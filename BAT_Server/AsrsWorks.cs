using BAT_Class_Library;
using DataAccess;
using log4net;
using Microsoft.VisualBasic;
using PlcCommunication;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace BAT_Server
{
    public class AsrsWorks
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static PlcCommunicationReadedDataAcces readedDataAcces = new PlcCommunicationReadedDataAcces();
        private static PlcCommunicationWritedDataAcces writedDataAcces = new PlcCommunicationWritedDataAcces();

        static EasyModbusCommunication easyModbusCommunication;


        private static MachineTasksData machineTasksData = new MachineTasksData();
        private static PalletsAtAddressesData palletsAtAddressesData = new PalletsAtAddressesData();
        private static AddressData addressData = new AddressData();
        private static BuffersData buffersData = new BuffersData();
        private static OrderDetailPalletsData orderDetailPalletsData = new OrderDetailPalletsData();
        private static OrderDetailsData orderDetailsData = new OrderDetailsData();
        private static ShuttlesData shuttlesData = new ShuttlesData();
        private static readonly ProductionNotificationData productionNotificationData = new ProductionNotificationData();

        public static PlcCommunicationReadedData ReadAsrsPLC(MachinesDTO communicationMachine)
        {
            easyModbusCommunication = new EasyModbusCommunication(communicationMachine.IpAddress, 502);

            var readedData = easyModbusCommunication.ReadFromPLC(0, 40);

            var rd = ObjectAssign.IntArrayToObject_RD(readedData, communicationMachine);

            readedDataAcces.InsertReadedData(rd);

            return rd;
        }



        public static bool WriteAsrsPLC(MachinesDTO communicationMachine, TaskDTO taskDTO)
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




        internal static void DoEntrySideJobs(MachinesDTO senderMachine)
        {
            //simule ok
            easyModbusCommunication = new EasyModbusCommunication(senderMachine.IpAddress, 502);
            easyModbusCommunication.WriteToPLC(0, new int[] { 100 });//plcden ye done göndermesi için
            //


            var rd = ReadAsrsPLC(senderMachine);
            var id = easyModbusCommunication.ConvertRegistersToInt(new int[] { rd.MW202, rd.MW203 });


            if (id > 0 && rd.MW200 == (int)Plc_Task.DONESUCCESSFULLY)
            {

                log.Info(id);


                var incomingTask = machineTasksData.GetMachineTaskById(id);
                var prodNotif = productionNotificationData.GetProductionNotificationById(incomingTask.ProductNotificationId);

                if (!incomingTask.IsCompleted)
                {
                    if (incomingTask.TaskType == (int)TaskType.CTA)
                    {
                        palletsAtAddressesData.InsertPalettesAtAddresses(incomingTask, prodNotif);
                        addressData.AddressesChangeFirstRowInfo(incomingTask.TargetAddress, false, incomingTask.EndTime, Location.WH_IN);
                        machineTasksData.CreateTaskForFirstRowPalette(incomingTask);
                    }
                    else if (incomingTask.TaskType == (int)TaskType.CTB)
                    {
                        buffersData.UpdateBufferWithPalette(incomingTask); //buffer transaction logu tablosu oluşturup tutulmalı mı?
                        machineTasksData.CreateTaskForBufferPalette(incomingTask);
                    }
                    else if (incomingTask.TaskType == (int)TaskType.BTA) //TODO:check
                    {
                        buffersData.UpdateBufferWithOutPalette(incomingTask);
                        palletsAtAddressesData.InsertPalettesAtAddresses(incomingTask, prodNotif);
                        addressData.AddressesChangeFirstRowInfo(incomingTask.TargetAddress, false, incomingTask.EndTime, Location.WH_IN);
                        machineTasksData.CreateTaskForFirstRowPalette(incomingTask);
                    }

                    machineTasksData.UpdateMachineTaskWithPlcCompletedById(id);

                }


                //task ata
                AssignTask(senderMachine);
            }
            else if (rd.MW200 == (int)Plc_Task.DONEWITHERROR) // hata kontrolu
            {
                //yarım iş??
            }



        }

        internal static void DoExitSideJobs(MachinesDTO communicationMachine)
        {

            var rd = ReadAsrsPLC(communicationMachine);
            var id = easyModbusCommunication.ConvertRegistersToInt(new int[] { rd.MW202, rd.MW203 });


            var incomingTask = machineTasksData.GetMachineTaskById(id);

            if (incomingTask.TaskType == (int)TaskType.ATC)
            {
                //TODO:paletin hangi place e gittiğine göre fg4 ya da normal

                machineTasksData.UpdateMachineTaskWithPlcCompletedById(id);
                palletsAtAddressesData.UpdatePalletsAtAddressWithRelease(incomingTask.LoadInfo);

                orderDetailPalletsData.UpdateOrderDetailPalletsWithTaken(incomingTask.OrderDetailPalletId);
                var odp = orderDetailPalletsData.GetOrderDetailPalletById(incomingTask.OrderDetailPalletId);


                int remainPalletCount = orderDetailPalletsData.RemainCountByOrderDetailId(odp.OrderDetailId);

                if (remainPalletCount == 0)
                    orderDetailsData.UpdateOrderDetailsWithTaken(odp.OrderDetailId);

                UpdateProductQuantitiesPlaces();//trickery

                addressData.AddressesChangeFirstRowInfo(incomingTask.SourceAddress, true, null, Location.WH_OUT);

            }
            else if (incomingTask.TaskType == (int)TaskType.ShATA)
            {
                machineTasksData.UpdateMachineTaskWithPlcCompletedById(id);
                shuttlesData.UpdateShuttleLastAddressAndStatus(incomingTask);
                addressData.AddressesChangeFirstRowInfo(incomingTask.SourceAddress, true, null, Location.WH_OUT);
            }
            else if (incomingTask.TaskType == (int)TaskType.ShCHRG)
            {
                machineTasksData.UpdateMachineTaskWithPlcCompletedById(id);
                shuttlesData.UpdateShuttleLastAddressAndStatus(incomingTask);
                addressData.AddressesChangeFirstRowInfo(incomingTask.SourceAddress, true, null, Location.WH_OUT);
            }
            else if (incomingTask.TaskType == (int)TaskType.ATA)//olma ihtimali düşük olursa giriş işlemi gibi çalışması gerekebilir.
            {
                //değerlendir
            }
            else
            {
                //hata logu vs try catch
            }


            //tüp ağzındaki paleti aldı fg4 yada normal conveyore bıraktı
            //mekik aldı taşıdı
            //mekik aldı şarja götürdü
            //durumlarına göre güncellemeler yapacak
            //status update leri kritik olmalı


            //yaptığı işlerin kontrolünden sonra yeni iş atama:
            //orders check
            //mekik şarj task
            AssignTask(communicationMachine);


        }

        private static void UpdateProductQuantitiesPlaces()
        {
            //throw new NotImplementedException();
        }

        private static void AssignTask(MachinesDTO communicationMachine)
        {

            if (communicationMachine.Location == Location.WH_IN)
            {
                TaskDTO taskDTO = machineTasksData.GetTaskDataForEntrySide();

                if (taskDTO != null && WriteAsrsPLC(communicationMachine, taskDTO))
                {
                    log.Info("assign id: " + taskDTO.taskId);
                    machineTasksData.UpdateMachineTaskWithPlcSentById(taskDTO.taskId);
                }
            }
            else if (communicationMachine.Location == Location.WH_OUT)
            {
                //TODO: adres ağzı dolu olanın taskı atanmalı
                //shuttle getirdiğinde
                TaskDTO taskDTO = machineTasksData.GetTaskDataForExitSide();

                if (taskDTO != null && WriteAsrsPLC(communicationMachine, taskDTO))
                {
                    machineTasksData.UpdateMachineTaskWithPlcSentById(taskDTO.taskId);
                }
            }


        }





    }
}
