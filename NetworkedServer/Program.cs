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

        static List<String> Chars = new List<string> {
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "0","1","2","3","4","5","6","7","8","9" };

        static void Main(string[] args)
        {
            NetworkHandler.Start(Handler);
            Console.WriteLine("Running");
            while (true) { Console.ReadLine(); }
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
                string[] Bounds = CreateBounds();
                NetworkHandler.SendMessage(IPAddress.Parse(Content[0]), new List<string> { "Bounds", Bounds[0],Bounds[1] });
                NetworkHandler.SendMessage(IPAddress.Parse(Content[0]), new List<string> { "Start" });
            }
            if (Content[1] == "Goodbye")
            {
                ClientIPs.Remove(Content[0]);
            }
        }

        static List<String> ValidCodes = new List<string> { };
        static void CheckCode(string Code, string IP)
        {
            if (IsValidCode(Code))
            { ValidCodes.Add(Code); System.IO.File.AppendAllText("./ValidCodes.dat", "https://discord.gg/" + Code + "\n"); Console.WriteLine("Received Valid Code"); }
            else { Console.WriteLine("Received InValid Code"); }
        }

        static WebClient wb = new WebClient();
        static Boolean IsValidCode(string Code)
        {
            try { wb.DownloadString("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true"); return true; }
            catch { return false; }
        }
        
        static string[] CreateBounds()
        {
            String StartPoint = "aaaaaa", EndPoint = "999999";
            return new string[] { StartPoint,EndPoint };
        }

    }
}
