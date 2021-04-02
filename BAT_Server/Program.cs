using BAT_Class_Library;
using DataAccess;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BAT_Server
{
    public class Program
    {
        //data acces classes
        CommonData commonData = new CommonData();

        //data objects
        static List<MachinesDTO> lstMachinesCode_Ip;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        //LOG OBJECT
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Program()
        {
            // Load LOG4NET configuration
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));


            lstMachinesCode_Ip = commonData.GetAllMachinesIpAddress();
        }

        public static void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 502);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf(">") > -1)
                {
                    int start = content.IndexOf("<") + 1;
                    int end = content.IndexOf(">");

                    content = content.Substring(start, end - start);

                    log.Info(content);

                    if (content == "EMERGENCY")
                    {

                    }
                    else if (content == "MAINTENANCE")
                    {

                    }
                    else if (content == "IP=50 CODE=13")
                    //else if (content == "1")
                    {

                        var senderMachine = lstMachinesCode_Ip.First(x => x.IpAddress == ((IPEndPoint)handler.RemoteEndPoint).Address.ToString());

                        switch (senderMachine.Type)
                        {
                            case "ASRS":
                                if (senderMachine.Location == Location.WH_IN)
                                    AsrsWorks.DoEntrySideJobs(senderMachine);
                                else
                                    AsrsWorks.DoExitSideJobs(senderMachine);
                                break;

                            case "SHUTTLE":
                                ShuttleWorks.DoShuttleJobs(senderMachine);
                                break;

                            case "CONVEYOR":
                                if (senderMachine.Location == Location.WH_IN)
                                    ConveyorWorks.DoEntrySideJobs(senderMachine);
                                //else
                                    //ConveyorWorks.WriteConveyorPLC();//değişecek böyle kalmayacak
                                break;

                            case "OTHER":
                                PreConveyorWorks.ReadStrechMachinePLC(senderMachine);
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {


                    }

                    // Echo the data back to the client.  
                    Console.WriteLine("Connection closing: {0}.", handler.RemoteEndPoint);
                    //Send(handler, content);
                    //Herhangi birşey buradan send edilmiyor bağlantı burada kapatılıyor
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }



        /** * SEND * **/
        //private static void Send(Socket handler, String data)
        //{
        //    // Convert the string data to byte data using ASCII encoding.  
        //    byte[] byteData = Encoding.ASCII.GetBytes(data);

        //    // Begin sending the data to the remote device.  
        //    handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        //}

        //private static void SendCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        // Retrieve the socket from the state object.  
        //        Socket handler = (Socket)ar.AsyncState;

        //        // Complete sending the data to the remote device.  
        //        int bytesSent = handler.EndSend(ar);
        //        //Console.WriteLine("Sent {0} bytes to client.", bytesSent);
        //        //Console.WriteLine("client ip {0}.", handler.RemoteEndPoint);

        //        Console.WriteLine("Connection closing: {0}.", handler.RemoteEndPoint);
        //        handler.Shutdown(SocketShutdown.Both);
        //        handler.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}

        public static int Main(String[] args)
        {
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        string str = Console.ReadLine();
            //        Console.WriteLine($"yazdınız: {str}");
            //    }

            //});

            Program p = new Program();
            StartListening();

            Console.WriteLine("start listeningi geçti");
            return 0;
        }

    }
}
