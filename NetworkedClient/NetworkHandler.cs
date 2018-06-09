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
        static string DefaultServer = "192.168.1.4";
        static int DefaultPort = 6921;
        static IPAddress MyIP = IPAddress.Parse("192.168.1.23");
        static TcpListener Receiver;
        static TcpClient Sender;

        public delegate void HandlerType(string[] Data);
        static HandlerType Handler;

        static string TargetServer;
        static int TargetPort;

        static Thread ListnerThread = new Thread(() => Listner());
        public static void Start(HandlerType LHandler)
        {
            TargetPort = DefaultPort;
            TargetServer = DefaultServer;
            Handler = LHandler;
        }
        public static void Start(HandlerType LHandler, string CustomServer,int CustomPort)
        {
            TargetServer = CustomServer;
            TargetPort = CustomPort;
            Handler = LHandler;
            ListnerThread.Start();
        }

        static void Listner()
        {
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
                    Handler(Data.Split("|".ToCharArray()));
                }
                System.Threading.Thread.Sleep(10);
            }
        }

        public static void SendMessage(List<String> Content)
        {
            try
            {
                StreamWriter Writer = new StreamWriter(new TcpClient(TargetServer, TargetPort).GetStream());
                String FormattedContent = MyIP.ToString();
                foreach (String Item in Content) { FormattedContent = "|"+FormattedContent + Item; }
                Writer.Write(FormattedContent);
                Writer.Flush();
                Writer.Close();
            }catch { }
        }

    }
}
