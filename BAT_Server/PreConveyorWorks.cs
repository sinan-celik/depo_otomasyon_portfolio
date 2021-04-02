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
    //amacı
    //Konveyorden önce strechleme makinasından gelen bilgileri okuyup kayıt etmek 
    //uygun şartlar varsa mekik taşıma işi oluşturmak 
    public class PreConveyorWorks
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly PlcCommunicationReadedDataAcces readedDataAcces = new PlcCommunicationReadedDataAcces();
        private static readonly PlcCommunicationWritedDataAcces writedDataAcces = new PlcCommunicationWritedDataAcces();
        private static readonly AsrsData asrsData = new AsrsData();
        private static readonly ShuttlesData shuttlesData = new ShuttlesData();

        private static readonly AddressData addressData = new AddressData();
        private static readonly BuffersData buffersData = new BuffersData();

        private static readonly PreProductionNotificationsData preProductionNotificationsData = new PreProductionNotificationsData();
        //private static readonly PreProductionNotification preProductionNotification = new PreProductionNotification();

        private static readonly MachineTasksData tasksData = new MachineTasksData();

        static EasyModbusCommunication easyModbusCommunication;

        public static void ReadStrechMachinePLC(MachinesDTO communicationMachine)
        {

            easyModbusCommunication = new EasyModbusCommunication(communicationMachine.IpAddress, 502);
            var readedData = easyModbusCommunication.ReadFromPLC(0, 40);

            var obj = ObjectAssign.IntArrayToObject_RD(readedData, communicationMachine);
            readedDataAcces.InsertReadedData(obj);


            if (true/*obj.MW200 == 0*/)
            {
                int productId = 0;//plc okuma sonrası netleşecek alanlar belki barcoda göre gidip tekrar sorgu atması gerekecek

                InsertPreProductionNotification(new PreProductionNotification
                {
                    ProductId = 0,
                    PaletteBarcode = "",
                    BatchNo = "BT382020",
                    PreNotificationTime = DateTime.Now,
                    Lot = "",
                    ReadOnTheConveyor = false
                });

                //insert başarılı ve bufferda aynı üründen varsa adresinin tüp ağzında da aynı üründen varsa 
                //adrese mekik shuttle taşıma işi oluştur

                var address = addressData.GetFirstRowIsEmptyByProductId(productId);
                var buffer = buffersData.GetPaletteInfoByProductId(productId);

                if (!address.FirstRowIsEmpty && buffer.Id > 0)
                {
                    CreateTasksForShuttleRelocation(address.Code);

                }

            }

        }

        private static void CreateTasksForShuttleRelocation(string code)
        {
            var asrs = asrsData.GetAllAsrs().First(x => x.Location == Location.WH_OUT); //çıkış tarafındaki asrs taşıma işiyle ilgili
            Shuttle shuttle = shuttlesData.GetMoveableShuttleByAddress(code);

            //shuttle ın taşınma konumuna gitmesi için
            MachineTask machineTaskShuttle = new MachineTask
            {
                OrderDetailPalletId = 0,//boş kalamalı
                ProductNotificationId = 0,//boş kalamalı
                TaskType = (int)TaskType.ShATA,
                TaskBatch = 0,
                Sequence = 1,
                MachineCode = shuttle.Code,
                SourceType = AddressType.ADDRESS,
                SourceAddress = shuttle.LastAddress,
                LoadInfo = "",
                TargetType = AddressType.ADDRESS,
                TargetAddress = shuttle.LastAddress, //aynı adreste tüp ağzına gitmesi için
                AssignUser = "SYSTEM",
                AssignReason = "STANDART",
                AssignTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                SentFlag = false,
                IsCompleted = false,
                ErrorCode = null,
            };

            //asrs shuttle alması için
            MachineTask machineTaskASRS = new MachineTask
            {
                OrderDetailPalletId = 0,//boş kalamalı
                ProductNotificationId = 0,//boş kalamalı
                TaskType = (int)TaskType.ShATA,
                TaskBatch = 0,
                Sequence = 2,
                MachineCode = asrs.Code,
                SourceType = AddressType.ADDRESS,
                SourceAddress = "shuttle ın olduğu adress",
                LoadInfo = shuttle.Code,
                TargetType = AddressType.ADDRESS,
                TargetAddress = code,
                AssignUser = "SYSTEM",
                AssignReason = "STANDART",
                AssignTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                SentFlag = false,
                IsCompleted = false,
                ErrorCode = null,
            };

            //insert movement tasks
            tasksData.InsertMachineTask(machineTaskShuttle);
            tasksData.InsertMachineTask(machineTaskASRS);
        }

        private static void InsertPreProductionNotification(PreProductionNotification preProductionNotification)
        {
            preProductionNotificationsData.InsertPreProductionNotification(preProductionNotification);
        }
    }
}
