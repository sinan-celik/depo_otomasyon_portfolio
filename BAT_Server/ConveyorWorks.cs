using BAT_Class_Library;
using DataAccess;
using log4net;
using PlcCommunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BAT_Server
{
    public class ConveyorWorks
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly PlcCommunicationReadedDataAcces readedDataAcces = new PlcCommunicationReadedDataAcces();
        private static readonly PlcCommunicationWritedDataAcces writedDataAcces = new PlcCommunicationWritedDataAcces();

        private static readonly AsrsData asrsData = new AsrsData();
        private static readonly AddressData addressData = new AddressData();
        private static readonly BuffersData buffersData = new BuffersData();
        private static readonly MachineTasksData tasksData = new MachineTasksData();
        private static readonly ProductsData productsData = new ProductsData();
        private static readonly ProductionNotificationData productionNotificationData = new ProductionNotificationData();

        static EasyModbusCommunication easyModbusCommunication;


        static Random random = new Random();

        static int productId = random.Next(1, 30);
        static int productionlineId = random.Next(1, 22);
        static string paletteBarcode = "palettebarcode" + productId.ToString();
        static int boxquantity = random.Next(50, 100);
        static string lot = "lot" + productId.ToString();
        static string batch = "BT382020";


        public static PlcCommunicationReadedData ReadConveyorPLC(MachinesDTO senderMachine)
        {
            easyModbusCommunication = new EasyModbusCommunication(senderMachine.IpAddress, 502);

            var readedData = easyModbusCommunication.ReadFromPLC(0, 40);

            var rd = ObjectAssign.IntArrayToObject_RD(readedData, senderMachine);

            readedDataAcces.InsertReadedData(rd);

            return rd;
        }

        public static void WriteConveyorPLC()
        {
            //conveyore seni okudum diye yazılabilir durumları setlenebilir.
            throw new NotImplementedException();

        }

        public static void DoEntrySideJobs(MachinesDTO senderMachine)
        {


            productId = random.Next(1, 30);
            productionlineId = random.Next(1, 22);
            paletteBarcode = "palettebarcode" + productId.ToString();
            boxquantity = random.Next(50, 100);
            lot = "lot" + productId.ToString();
            batch = "BT382020";

            easyModbusCommunication = new EasyModbusCommunication(senderMachine.IpAddress, 502);
            easyModbusCommunication.WriteToPLC(0, new int[] { 100 });//plcden ye done göndermesi için

            //Console.SetOut(TextWriter.Null);
            var rd = ReadConveyorPLC(senderMachine);
            //Console.SetOut(TextWriter.Synchronized(writer));

            var id = easyModbusCommunication.ConvertRegistersToInt(new int[] { rd.MW202, rd.MW203 });

            if (rd.MW200 == (int)Plc_Task.DONESUCCESSFULLY)
            {
                var pnId = InsertProductionNotification();//plc den gelen datayla beslenecek gibi düşün
                CreateTaskForPalette(paletteBarcode, senderMachine, pnId /*obj den gelen bir data veri barkod üzerine sistem kayıtlarını oluştur.*/);
            }

            //WriteDataToPlc();

        }

        private static long InsertProductionNotification()
        {

            return productionNotificationData.InsertProductionNotification(new ProductionNotification
            {

                ProductId = productId,
                ProductionLineId = productionlineId,
                PaletteBarcode = paletteBarcode,//TODO:oku
                BatchNo = batch,
                NotificationTime = DateTime.Now, //üretim zamanı gelirse ordan yazılabilir
                WeightUnit = "KG",
                Weight = 0,
                BoxQuantity = boxquantity, //en önemli değişken bu oluyor mutlaka olmalı ve sıfırdan büyük olmalı
                Lot = lot,
                CreateDate = DateTime.Now, // kayıt oluşma zamanı
                CreateUser = "SYSTEM",
                LastUpdateDate = null,
                LastUptadeUser = null

            });
        }

        public static void CreateTaskForPalette(string barcode, MachinesDTO communicationMachine, long pnId)
        {
            var asrs = asrsData.GetAllAsrs().First(x => x.Location == Location.WH_IN);

            Product product = productsData.GetProductByBarcode(barcode);
            var target = SelectTarget(product.Id);
            var taskBatch = tasksData.GetNewTaskBatch();

            MachineTask machineTask = new MachineTask
            {
                OrderDetailPalletId = 0,//boş kalamalı
                ProductNotificationId = (int)pnId,
                TaskType = target.Item1 == AddressType.ADDRESS ? (int)TaskType.CTA :
                           target.Item1 == AddressType.BUFFER ? (int)TaskType.CTB :
                           0, //address type göre task type belirlendi 0 olma durumunda hata var demektir
                TaskBatch = taskBatch,
                Sequence = 1,
                MachineCode = asrs.Code,
                SourceType = communicationMachine.Type,
                SourceAddress = communicationMachine.Code, //conveyorden alıyor conveyore konum bigileri eklemek gerekebilir
                LoadInfo = barcode,
                TargetType = target.Item1,
                TargetAddress = target.Item2,
                AssignUser = "SYSTEM",
                AssignReason = "STANDART",
                AssignTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                SentFlag = false,
                IsCompleted = false,
                ErrorCode = null,

            };

            //insert task
            tasksData.InsertMachineTask(machineTask);


            //daha sonra icra edilmek üzere palet buffera taşındıysa batch ve sequence ayarlayarak
            //buffer dan address e taşıma task ı oluştur
            if (target.Item1 == AddressType.BUFFER)
            {

                MachineTask machineTaskBTA = new MachineTask
                {
                    OrderDetailPalletId = 0,//boş kalamalı
                    ProductNotificationId = (int)pnId,
                    TaskType = (int)TaskType.BTA,
                    TaskBatch = taskBatch,
                    Sequence = 2,
                    MachineCode = asrs.Code,
                    SourceType = AddressType.BUFFER,
                    SourceAddress = target.Item2,
                    LoadInfo = barcode,
                    TargetType = AddressType.ADDRESS,
                    TargetAddress = addressData.SelectRelatedAppropiriateAddress(product.Id),  //TODO: mutlaka adres seçilmesi sağlanacak. yeni method yazılmalı
                    AssignUser = "SYSTEM",
                    AssignReason = "STANDART",
                    AssignTime = DateTime.Now,
                    StartTime = null,
                    EndTime = null,
                    SentFlag = false,
                    IsCompleted = false,
                    ErrorCode = null,

                };
                tasksData.InsertMachineTask(machineTaskBTA);

            }
        }



        private static Tuple<string, string> SelectTarget(int productId)
        {
            string type = "";
            string address = "";


            address = addressData.SelectRelatedAppropiriateAddress(productId);
            if (!string.IsNullOrEmpty(address))
            {
                type = AddressType.ADDRESS;
                return Tuple.Create(type, address);
            }
            else
            {
                address = buffersData.SelectAppropiriateBuffer().Code;
                type = AddressType.BUFFER;
                return Tuple.Create(type, address);
            }

            //adress ve bufer dolu olma durumu tartışılacak
        }


    }
}
