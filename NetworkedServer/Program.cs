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
        static void Main(string[] args)
        {
            NetworkHandler.Start(Handler);
            Console.WriteLine("Running");
            while (true) { Console.ReadLine(); }
        }

        static void Handler(string[] Content)
        {
            if (Content[1] == "ValidCode")
            {
                Console.WriteLine("Received Code");
                CheckCode(Content[2], Content[0]);
                NetworkHandler.SendMessage(IPAddress.Parse(Content[0]), new List<string> { "FuckOFf" });
            }
            if (Content[1] == "Hello")
            {
                NetworkHandler.SendMessage(IPAddress.Parse(Content[0]), new List<string> { "Start" });
            }
        }

        static List<String> ValidCodes = new List<string> { };
        static void CheckCode(string Code,string IP)
        {
            if (IsValidCode(Code))
            { ValidCodes.Add(Code); NetworkHandler.SendMessage(IPAddress.Parse(IP),new List<string> { "ValidCode", Code }); }
        }

        static WebClient wb = new WebClient();
        static Boolean IsValidCode(string Code)
        {
            try { wb.DownloadString("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true"); return true; }
            catch { return false; }
        }
    }
}
