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
        static string DefaultServer = "51.68.46.67";
        static int DefaultPort = 6921;
        static WebClient WB = new WebClient();
        static String MyIpExternal = WB.DownloadString("http://checkip.dyndns.org/").Replace("\r", "").Replace("Current IP Address: ", "").Replace("<html><head><title>Current IP Check</title></head><body>","").Replace("</body></html>","").Replace("\n","");
        static String MyIPInternal = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
        static String MyIP = MyIpExternal + "-" + MyIPInternal;

        public delegate void HandlerType(string[] Data);
        static HandlerType Handler;

        static string TargetServer;
        static int TargetPort;

        public static Thread ListnerThread = new Thread(() => Listner());
        public static void Start(HandlerType LHandler)
        {
            TargetPort = DefaultPort;
            TargetServer = DefaultServer;
            Handler = LHandler;
            ListnerThread.Priority = ThreadPriority.AboveNormal;
            ListnerThread.Start();
        }
        public static void Start(HandlerType LHandler, string CustomServer,int CustomPort)
        {
            TargetServer = CustomServer;
            TargetPort = CustomPort;
            Handler = LHandler;
            ListnerThread.Priority = ThreadPriority.AboveNormal;
            ListnerThread.Start();
        }

        static void Listner()
        {
            SendMessage(new List<string> {"GetBounds"});
            Console.WriteLine("Requesting Start");
            string PreviousData = "";
            while (true)
            {
                try
                {
                    string CurData = WB.DownloadString("http://" + TargetServer + "/Messages/" + MyIpExternal + "-" + MyIPInternal + ".html");
                    if (CurData != PreviousData)
                    {
                        PreviousData = CurData;
                        SendMessage(new List<string> { "Understood" });
                        if (CurData.Contains("|")) { Handler(CurData.Split("|".ToCharArray())); }
                    }
                }
                catch { }
                System.Threading.Thread.Sleep(500);
            }
        }

        public static void SendMessage(List<String> Content)
        {
            try
            {
                StreamWriter Writer = new StreamWriter(new TcpClient(TargetServer, TargetPort).GetStream());
                String FormattedContent = MyIP+"|";
                foreach (String Item in Content) { FormattedContent = FormattedContent + Item + "|"; }
                Writer.Write(FormattedContent);
                Writer.Flush();
                Writer.Close();
            }catch { Console.WriteLine("Connection Issue"); }
        }

    }
}
