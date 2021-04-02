using BAT_Class_Library;
using DataAccess;
using log4net;
using PlcCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BAT_Server
{
    public class ShuttleWorks
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static PlcCommunicationReadedDataAcces readedDataAcces = new PlcCommunicationReadedDataAcces();
        private static PlcCommunicationWritedDataAcces writedDataAcces = new PlcCommunicationWritedDataAcces();

        static EasyModbusCommunication easyModbusCommunication;

        private static MachineTasksData machineTasksData = new MachineTasksData();
        private static PalletsAtAddressesData palletsAtAddressesData = new PalletsAtAddressesData();
        private static AddressData addressData = new AddressData();
        private static ShuttlesData shuttlesData = new ShuttlesData();
        private static AsrsData asrsData = new AsrsData();




        public static PlcCommunicationReadedData ReadShuttlePLC(MachinesDTO communicationMachine)
        {
            easyModbusCommunication = new EasyModbusCommunication(communicationMachine.IpAddress, 502);

            var readedData = easyModbusCommunication.ReadFromPLC(0, 40);

            var rd = ObjectAssign.IntArrayToObject_RD(readedData, communicationMachine);

            readedDataAcces.InsertReadedData(rd);

            return rd;
        }

        public static bool WriteShuttlePLC(MachinesDTO communicationMachine, TaskDTO taskDTO)
        {
            var idReg = easyModbusCommunication.ConvertIntToRegisters(taskDTO.taskId);
            var xReg = easyModbusCommunication.ConvertIntToRegisters(taskDTO.X);
            var z1Reg = easyModbusCommunication.ConvertIntToRegisters(taskDTO.Z1);
            var z2Reg = easyModbusCommunication.ConvertIntToRegisters(taskDTO.Z2);
            var gReg = easyModbusCommunication.ConvertIntToRegisters(taskDTO.G);

            //task verisine göre plc write datası oluşturup yazılması gerekie.
            PlcCommunicationWritedData wd = new PlcCommunicationWritedData();
            wd.MW100 = 0; //tetik 

            wd.MW102 = idReg[0];
            wd.MW103 = idReg[1];

            wd.MW109 = (int)Plc_Asrs.GO;

            wd.MW110 = xReg[0];
            wd.MW111 = xReg[1];

            wd.MW112 = z1Reg[0];
            wd.MW113 = z1Reg[1];

            wd.MW114 = z2Reg[0];
            wd.MW115 = z2Reg[1];

            wd.MW116 = gReg[0];
            wd.MW117 = gReg[1];

            wd.MachineCode = communicationMachine.Code;
            wd.RecordDate = DateTime.Now;

            easyModbusCommunication = new EasyModbusCommunication(communicationMachine.IpAddress, 502);


            if (easyModbusCommunication.WriteToPLC(0, ArrayAssign.WritedDataToIntArray(wd)))//plc ye yazdıysa
            {
                writedDataAcces.InsertWritedData(wd);
                return true;
            }
            else
            {
                log.Error($"ASRS Plc Write Error. TaskId : {taskDTO.taskId}");
                return false;
            }


        }

        internal static void DoShuttleJobs(MachinesDTO senderMachine)
        {
            //simule ok
            easyModbusCommunication = new EasyModbusCommunication(senderMachine.IpAddress, 502);
            easyModbusCommunication.WriteToPLC(0, new int[] { 100, 0 });//plcden ye done göndermesi için
            //


            var rd = ReadShuttlePLC(senderMachine);
            var id = easyModbusCommunication.ConvertRegistersToInt(new int[] { rd.MW202, rd.MW203 });

            if (id > 0 && rd.MW200 == (int)Plc_Task.DONESUCCESSFULLY)
            {


                int distance = 0;

                var incomingTask = machineTasksData.GetMachineTaskById(id);

                if (incomingTask.IsCompleted == false)
                {

                    if (incomingTask.TaskType == (int)TaskType.ShOPT)
                    {
                        //Tüpteki paletlerin bilgileri distancetoexit bilgileri entry date e göre güncelle isinside true olanlar
                        palletsAtAddressesData.OptimizationUpdatePalletsAtAddresses(incomingTask.TargetAddress);

                        machineTasksData.UpdateMachineTaskWithPlcCompletedById(id);
                    }
                    else if (incomingTask.TaskType == (int)TaskType.ShINSERT) //asrs koyduktan sonra oluşacak
                    {
                        //asrs koyduğunda oluştu burda güncellenmeli
                        //Tüp ağzındaki paleti palletsataddressde güncelle(distance to exit),
                        palletsAtAddressesData.UpdatePalletsAtAddressWithShInsert(incomingTask, distance);

                        //adrsin tüp agzı bilgisini güncelle boş olarak
                        addressData.AddressesChangeFirstRowInfo(incomingTask.TargetAddress, true, null, Location.WH_IN);


                        machineTasksData.UpdateMachineTaskWithPlcCompletedById(id);
                    }
                    else if (incomingTask.TaskType == (int)TaskType.ShTAKEOUT) //orderdetail a göre veye asrs buraya mekik taşıdıktan sonra oluşacak??
                    {
                        //Tüp içindeki en öndeki paleti palletsataddressde güncelle(sadece distance bilgisi  ), 
                        palletsAtAddressesData.UpdatePalletsAtAddressWithShTakeOut(incomingTask);

                        //adresin tüp agzı bilgisini güncelle dolu olarak
                        addressData.AddressesChangeFirstRowInfo(incomingTask.TargetAddress, false, DateTime.Now, Location.WH_OUT);

                        machineTasksData.UpdateMachineTaskWithPlcCompletedById(id);
                    }
                    else if (incomingTask.TaskType == (int)TaskType.ShATA)
                    {
                        //asrs de bu adım asrs için var burada shuttle için yapması gereken birşey olup olmadığı kontrol edilebilir
                        //
                        machineTasksData.UpdateMachineTaskWithPlcCompletedById(id);
                    }
                    else
                    {
                        //hata logu vs try catch
                    }
                }

                AssignTask(senderMachine, rd);

            }
        }



        private static void AssignTask(MachinesDTO communicationMachine, PlcCommunicationReadedData rd)
        {
            var shuttle = shuttlesData.GetShuttleByCode(communicationMachine.Code);

            if (shuttle.Assignment == ShuttleAssign.OPTIMIZATION && shuttle.Status == Enum.GetName(typeof(SysStatus), SysStatus.READY))
            {
                //simule ok
                easyModbusCommunication = new EasyModbusCommunication(communicationMachine.IpAddress, 502);
                easyModbusCommunication.WriteToPLC(48, new int[] { 75 });//plcden ye done göndermesi için
                                                                         //

                //şarjı varsa başka tüpe taşı ve optimizasyon başlat
                if (rd.MW248 > 10)//TODO: make parametric
                {
                    CreateOptimizationTasks(communicationMachine, shuttle);
                }
                else
                {
                    //şarja gönder, başka mekiği optimizasyon için ata, taşı, optimizasyon başlat
                    CreateChargeTasks(communicationMachine, shuttle);
                    shuttlesData.SelectNewOptimizationShuttle(); //yeni shuttle döndürür yeni optimizasyon mekiği için iş atayabilir yada mekikten sinyal gelince normal işleyişe göre çalışır
                }

                //şarjı yoksa şarja taşı, başka mekiği optimizasyon olarak ata, bulunduğu yeri yada başka adresi optimize görevi yaz (önce taşı sonra yaz!)

            }


            //TODO: optimization task seçmede kontrolet
            TaskDTO taskDTO = machineTasksData.GetTaskDataForShuttle(communicationMachine);

            if (taskDTO != null && WriteShuttlePLC(communicationMachine, taskDTO))
            {
                log.Info("shuttle assign id: " + taskDTO.taskId);
                machineTasksData.UpdateMachineTaskWithPlcSentById(taskDTO.taskId);
                machineTasksData.UpdateMachineTaskWithMachineCodeById(taskDTO.taskId, communicationMachine.Code);
            }
            else
            {
                CheckNeedShuttleAddressesAndCreateTaskForIt(communicationMachine);
            }


        }

        private static void CreateChargeTasks(MachinesDTO communicationMachine, Shuttle shuttle)
        {
            var asrs = asrsData.GetAllAsrs().First(x => x.Location == Location.WH_OUT);
            var taskBatch = machineTasksData.GetNewTaskBatch();
            var chrgAddress = addressData.GetAvailableChargeAddress();

            MachineTask opt1 = new MachineTask //asrs shuttle taşıma
            {
                OrderDetailPalletId = 0,
                ProductNotificationId = 0,
                TaskType = (int)TaskType.ShCHRG,
                TaskBatch = taskBatch,
                Sequence = 1,
                MachineCode = asrs.Code,
                SourceType = AddressType.ADDRESS,
                SourceAddress = shuttle.LastAddress,
                LoadInfo = shuttle.Code,
                TargetType = AddressType.ADDRESS,
                TargetAddress = chrgAddress,
                AssignUser = "ShuttleWorks",
                AssignReason = "Charge",
                AssignTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                SentFlag = false,
                IsCompleted = false,
                ErrorCode = null
            };

            machineTasksData.InsertMachineTaskBatch(new List<MachineTask> { opt1 });

        }

        private static void CreateOptimizationTasks(MachinesDTO communicationMachine, Shuttle shuttle)
        {
            var asrs = asrsData.GetAllAsrs().First(x => x.Location == Location.WH_OUT);
            var taskBatch = machineTasksData.GetNewTaskBatch();

            string nextOptAddress = palletsAtAddressesData.GetNextOptimizationNeededAddresses()[0]; //TODO: seç

            MachineTask opt1 = new MachineTask //asrs shuttle taşıma
            {
                OrderDetailPalletId = 0,
                ProductNotificationId = 0,
                TaskType = (int)TaskType.ShOPT,
                TaskBatch = taskBatch,
                Sequence = 1,
                MachineCode = asrs.Code,
                SourceType = AddressType.ADDRESS,
                SourceAddress = shuttle.LastAddress,
                LoadInfo = shuttle.Code,
                TargetType = AddressType.ADDRESS,
                TargetAddress = nextOptAddress,
                AssignUser = "ShuttleWorks",
                AssignReason = "Optimization",
                AssignTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                SentFlag = false,
                IsCompleted = false,
                ErrorCode = null

            };

            MachineTask opt2 = new MachineTask //shuttle opt başla
            {
                OrderDetailPalletId = 0,
                ProductNotificationId = 0,
                TaskType = (int)TaskType.ShOPT,
                TaskBatch = taskBatch,
                Sequence = 2,
                MachineCode = communicationMachine.Code,
                SourceType = AddressType.ADDRESS,
                SourceAddress = nextOptAddress,
                LoadInfo = "",
                TargetType = AddressType.ADDRESS,
                TargetAddress = nextOptAddress,
                AssignUser = "ShuttleWorks",
                AssignReason = "Optimization",
                AssignTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                SentFlag = false,
                IsCompleted = false,
                ErrorCode = null

            };

            machineTasksData.InsertMachineTaskBatch(new List<MachineTask> { opt1, opt2 });

        }

        private static void CheckNeedShuttleAddressesAndCreateTaskForIt(MachinesDTO communicationMachine)
        {
            //sp_GetNeedShuttleAddresses_sel
            var list = addressData.GetNeedShuttleAddresses();

            //TODO: eğer atanmış iş varsa aynı adrese yenisini atamamlı


            if (list.Count > 0)
            {
                var shuttle = shuttlesData.GetShuttleByCode(communicationMachine.Code);
                var asrs = asrsData.GetAllAsrs().First(x => x.Location == Location.WH_OUT); //çıkış tarafındaki asrs taşıma işiyle ilgili

                var taskBatch = machineTasksData.GetNewTaskBatch();
                MachineTask machineTaskShuttle = new MachineTask
                {
                    OrderDetailPalletId = 0,
                    ProductNotificationId = 0,
                    TaskType = (int)TaskType.ShATA, //tüp başına gitmesi için gerekli komutu buradan alabilir?
                    TaskBatch = taskBatch,
                    Sequence = 1,
                    MachineCode = communicationMachine.Code,
                    SourceType = AddressType.ADDRESS,
                    SourceAddress = shuttle.LastAddress,
                    LoadInfo = "",
                    TargetType = AddressType.ADDRESS,
                    TargetAddress = shuttle.LastAddress, //list[0].Code,
                    AssignUser = "ShuttleWorks",
                    AssignReason = "ShuttleNeed",
                    AssignTime = DateTime.Now,
                    StartTime = null,
                    EndTime = null,
                    SentFlag = false,
                    IsCompleted = false,
                    ErrorCode = null

                };

                MachineTask machineTaskAsrs = new MachineTask
                {
                    OrderDetailPalletId = 0,
                    ProductNotificationId = 0,
                    TaskType = (int)TaskType.ShATA, //tüp başına gitmesi için gerekli komutu buradan alabilir?
                    TaskBatch = taskBatch,
                    Sequence = 2,
                    MachineCode = asrs.Code,
                    SourceType = AddressType.ADDRESS,
                    SourceAddress = shuttle.LastAddress,
                    LoadInfo = shuttle.Code,
                    TargetType = AddressType.ADDRESS,
                    TargetAddress = list[0].Code, //gideceği adres
                    AssignUser = "ShuttleWorks",
                    AssignReason = "ShuttleNeed",
                    AssignTime = DateTime.Now,
                    StartTime = null,
                    EndTime = null,
                    SentFlag = false,
                    IsCompleted = false,
                    ErrorCode = null

                };

                machineTasksData.InsertMachineTask(machineTaskShuttle);
                machineTasksData.InsertMachineTask(machineTaskAsrs);
            }

        }
    }
}
