using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace NetworkedClient
{
    public static class NetworkHandler
    {
        static string DefaultServer = "18.236.181.128";
        static int DefaultPort = 6921;
        static WebClient WB = new WebClient();
        static String MyIpS = WB.DownloadString("http://checkip.dyndns.org/").Replace("\r", "").Replace("Current IP Address: ", "").Replace("<html><head><title>Current IP Check</title></head><body>","").Replace("</body></html>","").Replace("\n","");
        static IPAddress MyIP = IPAddress.Parse(MyIpS); //IPAddress.Parse("192.168.1.23"); 
        static TcpListener Receiver;
        static TcpClient Sender;

        public delegate void HandlerType(string[] Data);
        static HandlerType Handler;

        static string TargetServer;
        static int TargetPort;

        public static Thread ListnerThread = new Thread(() => Listner());
        public static void Start(HandlerType LHandler)
        {
            TargetPort = DefaultPort;
            TargetServer = DefaultServer;
            Receiver = new TcpListener(TargetPort);
            Handler = LHandler;
            ListnerThread.Priority = ThreadPriority.AboveNormal;
            Receiver.Start();
            ListnerThread.Start();
        }
        public static void Start(HandlerType LHandler, string CustomServer,int CustomPort)
        {
            TargetServer = CustomServer;
            TargetPort = CustomPort;
            Receiver = new TcpListener(TargetPort);
            Handler = LHandler;
            ListnerThread.Priority = ThreadPriority.AboveNormal;
            Receiver.Start();
            ListnerThread.Start();
        }

        static void Listner()
        {
            SendMessage(new List<string> {"Hello"});
            Console.WriteLine("Requesting Start");
            TcpClient LocalClient = new TcpClient { };
            string Data = "";
            while (true)
            {
                if (Receiver.Pending())
                {
                    Data = "";
                    LocalClient = Receiver.AcceptTcpClient();
                    StreamReader Stream = new StreamReader(LocalClient.GetStream());
                    while (Stream.Peek() > -1){
                        Data = Data + Convert.ToChar(Stream.Read()).ToString();}
                    if (Data.Contains("|")) { Handler(Data.Split("|".ToCharArray())); }
                }
                System.Threading.Thread.Sleep(10);
            }
        }

        public static void SendMessage(List<String> Content)
        {
            try
            {
                StreamWriter Writer = new StreamWriter(new TcpClient(TargetServer, TargetPort).GetStream());
                String FormattedContent = MyIP.ToString()+"|";
                foreach (String Item in Content) { FormattedContent = FormattedContent + Item + "|"; }
                Writer.Write(FormattedContent);
                Writer.Flush();
                Writer.Close();
            }catch { Console.WriteLine("Connection Issue"); }
        }

    }
}
