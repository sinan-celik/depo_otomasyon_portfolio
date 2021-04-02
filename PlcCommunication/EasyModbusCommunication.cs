using EasyModbus;
using log4net;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PlcCommunication
{

    public class EasyModbusCommunication
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string ip { get; set; }
        public int port { get; set; }

        //public int startingAddress { get; set; }
        //public int[] values { get; set; }
        //public int quantity { get; set; }

        public EasyModbusCommunication(string _ip, int _port)
        {
            ip = _ip;
            port = _port;
        }

        private ModbusClient _client;
        public ModbusClient client
        {
            get
            {
                try
                {
                    if (_client == null)
                    {
                        _client = new ModbusClient(ip, port);
                    }
                    else
                    {
                        return _client;
                    }
                }
                catch (Exception e)
                {
                    log.Error("An error occurred while creating ModbusClient.", e);
                }

                return _client;
            }
        }

        int counter = 0;
        public void Connect()
        {
            try
            {
                counter++;
                client.Connect();
                counter = 0;
            }
            catch (Exception ex)
            {
                log.Error($"COUNTER{counter}. PLC Connecion ex: {ex} ");
                client.Disconnect();
                Thread.Sleep(1000);
                if (counter <= 5)
                {

                    Connect();

                }
            }
        }

        public bool WriteToPLC(int startingAddress, int[] values)
        {
            if (!client.Connected)
            {
                try
                {
                    //client.ConnectionTimeout = 5000;
                    //client.Connect();
                    Connect();
                }
                catch (Exception ex)
                {
                    log.Error($"PLC Connecion ex: {ex}");
                    //Thread.Sleep(100);
                    //client.Disconnect();

                    //client.Connect();
                }
            }
            //yazdı
            client.WriteMultipleRegisters(startingAddress, values);

            //yazdıklarını okudu
            var arr = client.ReadHoldingRegisters(startingAddress, values.Length);

            //ikisini karşılaştırdı doğru yazıp yazmadığını kontrol etti
            IStructuralEquatable se1 = values;
            bool isEqual = se1.Equals(arr, StructuralComparisons.StructuralEqualityComparer);

            client.Disconnect();

            return isEqual;
        }

        public bool WriteToPlcSingle(int startIndex, int value)
        {
            if (!client.Connected)
            {
                client.Connect();
            }
            client.WriteSingleRegister(startIndex, value);
            var readed = client.ReadHoldingRegisters(startIndex, 1);

            client.Disconnect();
            return (readed[0] == value);



        }

        public int[] ReadFromPLC(int startingAddress, int quantity)
        {
            if (!client.Connected)
            {
                try
                {
                    //client.ConnectionTimeout = 500;
                    client.Connect();
                }
                catch (Exception ex)
                {
                    log.Error($"PLC Connecion ex: {ex}");
                    Thread.Sleep(100);
                    client.Disconnect();

                    client.Connect();
                }
            }

            var arr = client.ReadHoldingRegisters(startingAddress, quantity);

            client.Disconnect();

            return arr;
        }

        public int ConvertRegistersToInt(int[] registers)
        {
            return ModbusClient.ConvertRegistersToInt(registers);
        }

        public int[] ConvertIntToRegisters(int value)
        {
            return ModbusClient.ConvertIntToRegisters(value);
        }

        // 1. Run the Write - Part on a Threadpool Thread ...
        private Task WriteRegAsync(float variable, ModbusClient client)
        {
            return Task.Run(() =>
            {
                client.WriteMultipleRegisters(
                             2,
                             ModbusClient.ConvertFloatToRegisters(variable,
                                                                   ModbusClient.RegisterOrder.HighLow)
                 );
            });
        }

        // 2. Run the Read-Part on a Threadpool Thread
        private Task<string> ReadRegAsync(int address, ModbusClient client)
        {
            return Task.Run(() =>
            {
                return ModbusClient.ConvertRegistersToFloat(
                                          client.ReadHoldingRegisters(address, 2),
                                          ModbusClient.RegisterOrder.HighLow)
                                   .ToString();
            });
        }



    }
}
