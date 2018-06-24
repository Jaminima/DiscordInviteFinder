using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace NetworkedServer
{
    public static class NetworkHandler
    {
        static int Port = 6921;

        static TcpListener Receiver=new TcpListener(Port);

        public delegate void HandlerType(string[] Data);
        static HandlerType Handler;

        static Thread ListnerThread = new Thread(() => Listner());
        public static void Start(HandlerType LHandler)
        {
            Handler = LHandler;
            ListnerThread.Priority = ThreadPriority.AboveNormal;
            ListnerThread.Start();
        }

        static void Listner()
        {
            Receiver.Start();
            Console.WriteLine("Ready To Receive");
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
                        Data = Data + Convert.ToChar(Stream.Read()).ToString();
                    }
                    if (Data.Contains("|")) { Handler(Data.Split("|".ToCharArray())); }
                }
                System.Threading.Thread.Sleep(10);
            }
        }

        static string MessageLocation = "/var/www/html/Messages/";
        public static void SendMessage(string IP,List<String> Content)
        {
            try { System.IO.File.Delete(MessageLocation + IP + ".html"); } catch { }
            String FormattedContent = "";
            foreach (String Item in Content) { FormattedContent = FormattedContent + Item + "|"; }
            System.IO.File.WriteAllText(MessageLocation + IP + ".html",FormattedContent);
        }

    }
}
