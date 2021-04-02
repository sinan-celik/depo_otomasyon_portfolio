using BAT_Class_Library;
using DataAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using System.Reflection;
using log4net.Config;
using System.Runtime.Loader;
using PlcCommunication;

namespace SimulationMaker
{
    class Program
    {

        static List<Asrs> asrsList;
        static List<Shuttle> shuttleList;
        static List<ProductionNotification> pnList;
        static List<MachineTask> lstMachineTask;
        static IEnumerable<TimeParameter> lstTimeParameters;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {


            //PlcCommunicationWritedData plcCommunicationWritedData = new PlcCommunicationWritedData
            //{
            //    Id = 0,
            //    MachineCode = "",
            //    MW100 = 1,
            //    MW101 = 2,
            //    MW102_MD51 = 4,
            //    MW103_MD52 = 43,
            //    MW104 = 5,
            //    MW105 = 6,
            //    MW106 = 7,
            //    MW107 = 8,
            //    MW108 = 9,
            //    MW109 = 10,
            //    MW110_MD55 = 110,
            //    MW111_MD55 = 340,

            //};
            //PlcCommunicationReadedData plcCommunicationReadedData = new PlcCommunicationReadedData
            //{
            //    Id = 0,
            //    MachineCode = "",
            //    MW210_MD105 = 1,
            //    MW201 = 2,
            //    MW202_MD101 = 4,
            //    MW203_MD101 = 43,
            //    MW204 = 5,
            //    MW205 = 6,
            //    MW206 = 7,
            //    MW207 = 8,
            //    MW208 = 9,
            //    MW209 = 10,
            //    MW211_MD105 = 110,
            //    MW212_MD106= 340,

            //};


            //var arr = ObjectAssign.ObjectToIntArray<PlcCommunicationWritedData>(plcCommunicationWritedData);
            //var arr2 = ObjectAssign.ObjectToIntArray(plcCommunicationReadedData);

            //Server server = new Server(502);
            //server.Start();


            //progaram ayarlarını yapılandırır. connection string vs.
            //configuration = ConfigureServices();
            ConfigureServices();
            //Elimizdeki aktif makineleri çek ve setle. buna göre yönetici ve iletişim sağlayan threadler oluştur.
            GetMachineLists();

            GetMachineMovementDurations();
            //
            GetProductionNotifications();


            Console.WriteLine("Hazır! Enterla ...");
            Console.ReadLine();

            foreach (var item in pnList)
            {
                CreatingTasks(item);
                Console.WriteLine("Task Oluşturuldu..." + item.PaletteBarcode);
                //Console.ReadLine();

                // Log some things
                //log.Info("program.cs!");
                //log.Error("Error!");
                //log.Warn("Warn!");
            }

            Console.WriteLine("Hazır! Enterla ...");
            Console.ReadLine();

            GetWaitingTasks();

            //CreateTaskByBarcodeRead();

            //GetMachinePositionsAndActiveTasks();

            //MakeDesicions();


            MakeSimulation();
        }

        private static void MakeSimulation()
        {
            SimulationRecordsData simulationRecordsData = new SimulationRecordsData();
            SimulationRecord sr;
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            int accelerationSeconds = lstTimeParameters.First(x => x.MachineType == "ASRS" && x.MovementType == "ACCELERATION").TimeInSeconds;
            int decelerationSeconds = lstTimeParameters.First(x => x.MachineType == "ASRS" && x.MovementType == "DECELERATION").TimeInSeconds;

            foreach (var task in lstMachineTask)
            {
                //task batche göre düzenlenecek current batchno değişkeniyle düzenleme yapılabilir bi düşün
                //task assing time starttan küçük olamaz 
                start = lstMachineTask.IndexOf(task) == 0 || task.AssignTime > end ? task.AssignTime : end;//son tasktan gelsin.

                if (task.TargetAddress == "ASRS_WH_IN")
                {
                    end = start.AddSeconds(15);
                }
                else
                {
                    end = start
                        .AddSeconds(accelerationSeconds) //hızlanma
                        .Add(CalculateTime(task))//hareket metreyle orantılı çarpılıp hesaplanacak
                        .AddSeconds(decelerationSeconds); //yavaşlama
                }



                if (task.Sequence == 3)
                {
                    bool firstRowIsEmpty;
                    AddressData addressData = new AddressData();
                    firstRowIsEmpty = addressData.GetFirstRowIsEmpty(task.TargetAddress).LastLoadTime <= start.AddMinutes(-7);
                    if (!firstRowIsEmpty)
                    {
                        var bfr = SelectBufferAddress();
                        task.TargetType = "BUFFER";
                        task.TargetAddress = bfr.Code;//isnullllll  bufferda yoksa yeni tüp seç
                        end = start
                        .AddSeconds(accelerationSeconds) //hızlanma
                        .AddSeconds(20)//hesaplanacak
                        .AddSeconds(decelerationSeconds); //yavaşlama

                        Console.WriteLine("Buffera taşındı...");
                    }
                }

                //adresi güncelle
                //doluysa buffera

                task.StartTime = start;
                task.EndTime = end;
                task.IsCompleted = true;
                UpdateTaskData(task);

                UpdateAddressData(task);

                sr = new SimulationRecord
                {
                    ProductNotificationId = task.TaskBatch,
                    DependedTaskBatchNo = task.TaskBatch,
                    MachineCode = task.MachineCode,
                    WaitingPaletteBufferCount = 0, //çekilecek
                    WaitingPaletteConveyorQuee = SelectWaitingPaletteCount(start), //task başladığı anda üretim bildiriminde bekleyen (conveyor yada buffer) çekilecek
                    ExecutionDurationInSeconds = Convert.ToInt32((end - start).TotalSeconds),
                    ExecutionCompleteDateTime = end
                };

                long id = simulationRecordsData.InsertSimulationRecord(sr);
                Console.WriteLine("Simulasyon Oluştu... " + id.ToString());


            }



        }

        private static void UpdateAddressData(MachineTask task)
        {
            switch (task.TargetType)
            {
                case "ADDRESS":
                    AddressData addressData = new AddressData();
                    //addressData.AddressesChangeFirstRowInfo(task.TargetAddress, false, task.EndTime);
                    break;
                case "BUFFER":
                    BuffersData buffersData = new BuffersData();
                    buffersData.ChangeBufferLoadInfo(task.TargetAddress, false, task.LoadInfo);
                    break;

                default:
                    break;
            }

        }

        private static BAT_Class_Library.Buffer SelectBufferAddress()
        {
            BuffersData buffersData = new BuffersData();
            return buffersData.SelectAppropiriateBuffer();
        }

        private static TimeSpan CalculateTime(MachineTask task)
        {
            AddressData addressData = new AddressData();
            var dist = addressData.GetDistanceToRefPoint(task.SourceAddress, task.TargetAddress);
            var totalDistance =
                Math.Abs(dist[0] - dist[1])//source - target -
                - Convert.ToDouble(lstTimeParameters.First(x => x.MachineType == "ASRS" && x.MovementType == "ACCELERATION").MeasureValue)
                - Convert.ToDouble(lstTimeParameters.First(x => x.MachineType == "ASRS" && x.MovementType == "DECELERATION").MeasureValue);

            var velocity = lstTimeParameters.First(x => x.MachineType == "ASRS" && x.MovementType == "MOVEMENT").MeasureValue;//1 saniyede kat ettiği mesafe
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalDistance / Convert.ToDouble(velocity));

            return timeSpan;
        }

        private static int SelectWaitingPaletteCount(DateTime start)
        {
            //Buffer waiting count eklenecek
            ProductionNotificationData productionNotificationData = new ProductionNotificationData();
            return productionNotificationData.SelectWaitingPaletteCount(start);
        }

        private static void CreatingTasks(ProductionNotification item)
        {

            List<MachineTask> machineTasks = new List<MachineTask>();

            machineTasks.Add(new MachineTask
            {
                TaskBatch = item.Id,
                Sequence = 1,
                MachineCode = "ASRS_WH_IN",
                SourceType = "ADDRESS",
                SourceAddress = asrsList.First(x => x.Location == "WH_IN").LastAddress,  //adresi çek ve güncelle
                LoadInfo = "", //üzerinde yük yok sadece yer değiştirecek, ilk adım, yinede barkod bilgisi yazılabilir
                TargetType = "CONVEYOR",
                TargetAddress = "WH_IN_1", //ilk adım için konveyor kodu.
                AssignUser = "SYSTEM",
                AssignReason = "STANDART",
                AssignTime = item.NotificationTime,
                StartTime = null,
                EndTime = null,
                IsCompleted = false,
                ErrorCode = ""
            });

            machineTasks.Add(new MachineTask
            {
                TaskBatch = item.Id,
                Sequence = 2,
                MachineCode = "ASRS_WH_IN",
                SourceType = "CONVEYOR",
                SourceAddress = "WH_IN_1",
                LoadInfo = item.PaletteBarcode, //Üzerine alma işlemi
                TargetType = "ASRS",
                TargetAddress = "ASRS_WH_IN",
                AssignUser = "SYSTEM",
                AssignReason = "STANDART",
                AssignTime = item.NotificationTime,
                StartTime = null,
                EndTime = null,
                IsCompleted = false,
                ErrorCode = ""
            });

            machineTasks.Add(new MachineTask
            {
                TaskBatch = item.Id,
                Sequence = 3,
                MachineCode = "ASRS_WH_IN",
                SourceType = "ASRS",
                SourceAddress = "ASRS_WH_IN",
                LoadInfo = item.PaletteBarcode,
                TargetType = "ADDRESS",
                TargetAddress = SelectRelatedAppropiriateAddress(item),
                AssignUser = "SYSTEM",
                AssignReason = "STANDART",
                AssignTime = item.NotificationTime,
                StartTime = null,
                EndTime = null,
                IsCompleted = false,
                ErrorCode = ""
            });

            //task insert
            MachineTasksData machineTasksData = new MachineTasksData();
            machineTasksData.InsertMachineTaskBatch(machineTasks);


            //asrs update
            Asrs asrsForUpdt = asrsList.First(x => x.Location == "WH_IN");
            asrsForUpdt.LastAddress = machineTasks.LastOrDefault().TargetAddress;
            log.Info("asrs son adresi  :  " + asrsForUpdt.LastAddress);
            UpdateAsrsLastAddress(asrsForUpdt);

        }

        private static void UpdateAsrsLastAddress(Asrs asrs)
        {
            AsrsData asrsData = new AsrsData();
            asrsData.UpdateLastAddress(asrs);
        }
        private static void UpdateTaskData(MachineTask task)
        {
            MachineTasksData td = new MachineTasksData();
            td.UpdateTask(task);
        }

        private static string SelectRelatedAppropiriateAddress(ProductionNotification item)
        {
            AddressData addressData = new AddressData();

            return addressData.SelectRelatedAppropiriateAddress(item.ProductId);
        }

        private static void GetProductionNotifications()
        {
            ProductionNotificationData data = new ProductionNotificationData();
            pnList = data.GetProductionNotifications().ToList();
        }

        private static void GetMachinePositionsAndActiveTasks()
        {
            //throw new NotImplementedException();
        }

        private static void MakeDesicions()
        {
            //throw new NotImplementedException();
        }

        private static void GetMachineMovementDurations()
        {
            Console.WriteLine("Parametreler Alınıyor...");

            TimeParametersData timeParametersData = new TimeParametersData();
            lstTimeParameters = timeParametersData.GetTimeParameters();

        }

        private static void CreateTaskByBarcodeRead()
        {
            Console.WriteLine("Yeni iş emirleri oluşturuluyor...");
        }

        private static void GetWaitingTasks()
        {
            Console.WriteLine("Bekleyen İşler Alınıyor...");
            MachineTasksData machineTasksData = new MachineTasksData();
            lstMachineTask = machineTasksData.GetWaitingTasks();
        }

        private static void GetMachineLists()
        {
            AsrsData asrs = new AsrsData();
            asrsList = asrs.GetAllAsrs().ToList();

            ShuttlesData shuttleData = new ShuttlesData();
            shuttleList = shuttleData.GetAllShuttles().ToList();
        }

        private static void ConfigureServices()
        {

            // Load LOG4NET configuration
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));



            ////Configuration for db connection
            //var builder = new ConfigurationBuilder()
            //   .SetBasePath(Directory.GetCurrentDirectory())
            //   .AddJsonFile("appsettings.json");

            //var configuration = builder.Build();

            //return configuration;
        }
    }
}
