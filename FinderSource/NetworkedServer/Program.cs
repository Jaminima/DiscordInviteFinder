using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace NetworkedServer
{
    class Program
    {
        public static int Steps = 0;
        static List<String> Chars = new List<string> {
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "0","1","2","3","4","5","6","7","8","9" };

        static long StartTime;
        static void Main(string[] args)
        {
            NetworkHandler.Start(Handler);
            Console.WriteLine("Running");
            while (true) {
                Console.ReadLine();
            }
        }

        static List<String> ClientIPs = new List<String> { };
        static void Handler(string[] Content)
        {
            if (Content[1] == "ValidCode")
            {
                CheckCode(Content[2], Content[0]);
            }
            if (Content[1] == "Hello")
            {
                ClientIPs.Add(Content[0]);
                string[] Bounds = CreateBounds(IPAddress.Parse(Content[0]));
                NetworkHandler.SendMessage(IPAddress.Parse(Content[0]), new List<string> { "Bounds", Bounds[0],Bounds[1] });
                NetworkHandler.SendMessage(IPAddress.Parse(Content[0]), new List<string> { "Start" });
            }
            if (Content[1] == "Goodbye")
            {
                ClientIPs.Remove(Content[0]);
                foreach (BoundsData BD in Bounds) { if (BD.IP == Content[0]) { Bounds.Remove(BD); break; } }
                if (ClientIPs.Count == 0) { Console.WriteLine("\rNo Clients                             "); }
            }
            if (Content[1] == "Steps")
            {
                Steps += int.Parse(Content[2]);
                if (DateTime.UtcNow.Ticks - StartTime >= 10000000) { Console.Write("\rCodes Per Second: " + Steps + " Clients: "+ClientIPs.Count+"......."); StartTime = DateTime.UtcNow.Ticks; Steps = 0; }
            }
        }

        static List<String> ValidCodes = new List<string> { };
        static void CheckCode(string Code, string IP)
        {
            if (IsValidCode(Code))
            { ValidCodes.Add(Code); System.IO.File.AppendAllText("./ValidCodes.dat", "\nhttps://discord.gg/" + Code); Console.WriteLine("Received Valid Code"); }
            else { Console.WriteLine("Received InValid Code"); }
        }

        static WebClient wb = new WebClient();
        static Boolean IsValidCode(string Code)
        {
            try { wb.DownloadString("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true"); return true; }
            catch { return false; }
        }

        static List<BoundsData> Bounds = new List<BoundsData> { };
        static string[] CreateBounds(IPAddress IP)
        {
            String StartPoint = "aaaaaa", EndPoint = "999999";
            int n = 0;
            while (true)
            {
                if (Math.Pow(2, n) >= Bounds.Count+1) { break; } else { n++; }
            }
            double BoundFraction = 1/Math.Pow(2,n);
            double BoundWidth = Math.Round(Math.Pow(62, 6) * BoundFraction,0);
            StartPoint = IntToCode(BoundWidth * ((Bounds.Count) * BoundFraction));
            EndPoint = IntToCode(BoundWidth * ((Bounds.Count+1) * BoundFraction)-1);
            Bounds.Add(new BoundsData(StartPoint,EndPoint,IP.ToString()));
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

    }

    class BoundsData {
        public String StartPoint, EndPoint, IP;
        public BoundsData(string SP,string EP,string lIP)
        {
            StartPoint = SP;EndPoint = EP;IP = lIP;
        }
    }

}
