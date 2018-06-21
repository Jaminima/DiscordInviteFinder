using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Timers;

namespace NetworkedServer
{
    class Program
    {
        static double Combinations = Math.Pow(62, 6),C=Combinations;
        public static int Steps = 0;
        static List<String> Chars = new List<string> {
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "0","1","2","3","4","5","6","7","8","9" };

        static long StartTime;
        static Timer timer;
        static void Main(string[] args)
        {
            DiscordAPI.Events.Start();
            NetworkHandler.Start(Handler);
            Console.WriteLine("Running");
            timer = new System.Timers.Timer(6000);
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            while (true)
            {
                Console.ReadLine();
            }
        }

        static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            RemoveInvalidCodes();
            CheckAliveClients();
        }

        static void CheckAliveClients()
        {
            for (int i= 0;i<ClientIPs.Count;i++)
            {
                if (ClientIPs[i].TimeSinceLast >= 5) { for (int l = 0; l < Bounds.Count; l++) { if (Bounds[l].IP == ClientIPs[i].IP) { Bounds.RemoveAt(l); } } ClientIPs.RemoveAt(i); if (ClientIPs.Count == 0) { Console.WriteLine("\rNo Clients                             "); } }
                else { ClientIPs[i].TimeSinceLast++; }
            }
        }

        static string MessageLocation = "C:/Bitnami/wampstack-7.1.18-1/apache2/htdocs/Messages/";
        static List<ClientData> ClientIPs = new List<ClientData> { };
        static void Handler(string[] Content)
        {
            if (Content[1] == "ValidCode")
            {
                CheckCode(Content[2], Content[0]);
            }
            if (Content[1] == "Hello")
            {
                ClientIPs.Add(new ClientData(Content[0]));
                NetworkHandler.SendMessage(Content[0], new List<string> { "Start" });
            }
            if (Content[1] == "GetBounds")
            {
                string[] Bounds = CreateBounds(Content[0]);
                NetworkHandler.SendMessage(Content[0], new List<string> { "Bounds", Bounds[0], Bounds[1] });
            }
            if (Content[1] == "Goodbye")
            {
                ClientIPs.RemoveAt(ClientIPs.FindIndex(x =>x.IP==Content[0]));
                foreach (BoundsData BD in Bounds) { if (BD.IP == Content[0]) { Bounds.Remove(BD); break; } }
                if (ClientIPs.Count == 0) { Console.WriteLine("\rNo Clients                                 "); }
            }
            if (Content[1] == "Steps")
            {
                Steps += int.Parse(Content[2]);
                if (DateTime.UtcNow.Ticks - StartTime >= 10000000) { Console.Write("\rCodes Per Second: " + Steps + " Clients: "+ClientIPs.Count+"......."); StartTime = DateTime.UtcNow.Ticks; Steps = 0; }
                try { ClientIPs[ClientIPs.FindIndex(x => x.IP == Content[0])].TimeSinceLast = 0; }
                catch { ClientIPs.Add(new ClientData(Content[0])); }
            }
            if (Content[1] == "Understood")
            {
                System.IO.File.Delete(MessageLocation + Content[0] + ".html");
            }
        }

        static List<String> ValidCodes = new List<string> { };
        static void CheckCode(string Code, string IP)
        {
            if (IsValidCode(Code))
            {   ValidCodes.Add(Code); System.IO.File.AppendAllText("./ValidCodes.dat", "\nhttps://discord.gg/" + Code);
                Console.WriteLine("Received Valid Code");
                string GuildID = DiscordAPI.Events.JoinServer(Code);
                foreach (Newtonsoft.Json.Linq.JObject Room in DiscordAPI.Events.GetTextChannels(GuildID)) {
                    try { 
                        DiscordAPI.Events.SendMessage((string)Room["id"], "Your Discord Was Found Via The\\nDiscord Invite Finder\\nLearn More By Joining Our Discord\\nhttps://discord.gg/SAt84m3");
                        try
                        {
                            string NewInvite = DiscordAPI.Events.CreateInvite((string)Room["id"]);
                            System.IO.File.WriteAllText("./ValidCodes.dat", System.IO.File.ReadAllText("./ValidCodes.dat").Replace("https://discord.gg/" + Code, "https://discord.gg/" + NewInvite));
                        }
                        catch { Console.WriteLine("Unable To Create/Save Invite"); }
                        break;
                    } catch {}
                }
                DiscordAPI.Events.LeaveServer(GuildID);
            }
            else { Console.WriteLine("Received InValid Code"); }
        }

        static WebClient wb = new WebClient();
        static Boolean IsValidCode(string Code)
        {
            try { wb.DownloadString("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true"); return true; }
            catch { return false; }
        }

        static List<BoundsData> Bounds = new List<BoundsData> { };
        static double BoundCapacity = 1;
        static string[] CreateBounds(string IP)
        {
            String StartPoint = "aaaaaa", EndPoint = "999999";
            double n = 0;
            while (true)
            {
                if (Math.Pow(2, n) >= Bounds.Count+1) { break; } else { n++; }
            }
            double BoundFraction = 1 / Math.Pow(2, n);
            if (BoundFraction < BoundCapacity)
            {
                BoundCapacity = BoundFraction;
                for (int i = 0; i < Math.Pow(2, n)/2; i++) {
                    Bounds[i].SPi = Math.Floor(0.5 * Bounds[i].SPi);
                    Bounds[i].EPi = Math.Ceiling(0.5 * Bounds[i].EPi);
                    Bounds[i].StartPoint = IntToCode(Bounds[i].SPi);
                    Bounds[i].EndPoint = IntToCode(Bounds[i].EPi);
                    NetworkHandler.SendMessage(Bounds[i].IP, new List<string> { "ResetBounds", Bounds[i].StartPoint, Bounds[i].EndPoint });
                }
            }
            double BoundWidth = Math.Ceiling(C * BoundFraction);
            double SI = C * ((Bounds.Count) * BoundFraction),
                EI = C * ((Bounds.Count + 1) * BoundFraction)-1;
            StartPoint = IntToCode(SI);
            EndPoint = IntToCode(EI);
            Bounds.Add(new BoundsData(StartPoint,EndPoint,IP,SI,EI));
            return new string[] { StartPoint,EndPoint };
        }

        static string IntToCode(double Int)
        {
            List<Double> IntegeralPart = new List<Double> { };
            int Loops = 0;
            while (Int > 0)
            {
                IntegeralPart.Add((Int % 62));
                Int = (Math.Floor(Int / 62));
                Loops++;
            }
            string Code = "";
            foreach (double i in IntegeralPart) { Code = Code + Chars[int.Parse(i.ToString())]; }
            for (int i = Loops; i < 6; i++) { Code =  Code + "a"; }
            return Code;
        }

        static void RemoveInvalidCodes()
        {
            List<string> oldLines = new List<string>();
            List<string> newLines = new List<string>();
            oldLines = System.IO.File.ReadAllLines("./ValidCodes.dat").ToList();

            foreach (string link in oldLines)
            {
                if (IsValidCode(link.Replace("https://discord.gg/", ""))) { newLines.Add(link); }
            }
            System.IO.File.WriteAllLines("./ValidCodes.dat", newLines);
        }
    }

    class BoundsData {
        public String StartPoint, EndPoint, IP;
        public double SPi, EPi;
        public BoundsData(string SP,string EP,string lIP,double lSPi,double lEPi)
        {
            StartPoint = SP;EndPoint = EP;IP = lIP;
            SPi = lSPi;EPi = lEPi;
        }
    }

    class ClientData
    {
        public string IP;
        public int TimeSinceLast;
        public ClientData(string lIP)
        {
            IP = lIP;TimeSinceLast = 0;
        }
    }

}
