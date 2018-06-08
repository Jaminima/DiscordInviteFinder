using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace DiscordInviteFinder
{
    static class Program
    {
        static List<String> CurCode = new List<string> { "a", "a", "a", "a", "a", "a" };
        static List<Thread> Threads = new List<Thread> { };
        static Boolean Exiting = false;
        static void Main(string[] args)
        {
            CurCode = Load();
            System.Console.WriteLine("Press `Ctrl+C` to Exit and Save.");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Save);
            List<String> Chars = new List<string> { };
            foreach (String C in "a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z|A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|0|1|2|3|4|5|6|7|8|9".Split("|".ToCharArray())) { Chars.Add(C); }
            
            while (CurCode != new List<string> { "9", "9", "9", "9", "9", "9" } && !Exiting)
            {
                string Code = CurCode[0] + CurCode[1] + CurCode[2] + CurCode[3] + CurCode[4] + CurCode[5];
                Threads.Add(new Thread(()=>CheckCode(Code)));
                Threads[Threads.Count - 1].Start();
                CurCode = IterateCode(CurCode, Chars);
                if (Threads.Count >= 1000 && !Exiting) { for (int i = 0; i < Threads.Count; i++) { if (Threads[i].IsAlive == false && !Exiting) { Threads.RemoveAt(i); } } }
            }
            System.Console.ReadLine();
        }

        static void CheckCode(string Code)
        {
            string Response = GetResponse(Code);
            if (Response != null)
            {
                System.IO.File.AppendAllText("./Valid.dat", "\n" + Code);
                System.Console.WriteLine("Found: https://discord.gg/" + Code);
            }
        }

        static WebClient wb = new WebClient();
        static string GetResponse(string Code)
        {
            try
            {
                string response = wb.DownloadString("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true");
                return response;
            }
            catch { return null; }
        }

        static List<String> IterateCode(List<String> Code,List<String> Chars)
        {
            Boolean PreviousOverflow = false;
            for (int i = 0; i < 6; i++)
            {
                if (PreviousOverflow || i == 0)
                {
                    try
                    {
                        Code[i] = Chars[Chars.FindIndex(x => x.StartsWith(Code[i])) + 1];
                        PreviousOverflow = false;
                    }
                    catch
                    {
                        Code[i] = Chars[0];
                        PreviousOverflow = true;
                    }
                }
            }
            return Code;
        }

        static List<String> Load()
        {
            string Stored;
            try { Stored = System.IO.File.ReadAllText("./Last.dat"); }
            catch { Stored = "aaaaaa"; }
            List<String> Out = new List<string> { };
            foreach (char C in Stored) { Out.Add(C.ToString()); }
            System.Console.WriteLine("Loaded " + Stored);
            return Out;
        }

        static void Save(object sender, ConsoleCancelEventArgs args)
        {
            Exiting = true;
            System.Console.Clear();
            string Code = CurCode[0] + CurCode[1] + CurCode[2] + CurCode[3] + CurCode[4] + CurCode[5];
            try { System.IO.File.Delete("./Last.dat"); } catch { }
            System.IO.File.WriteAllText("./Last.dat", Code);
            System.Console.WriteLine("Saved " + Code + "\nWaiting for all Requests to Close.\nPlease Wait!");
            //while (true)
            //{
            //    Boolean RunningThread = false;
            //    for (int i= 0;i<Threads.Count;i++)
            //    { if (Threads[i].IsAlive) { RunningThread = true; } else { Threads.RemoveAt(i); } }
            //    if (!RunningThread) { break; }
            //}
            System.Console.WriteLine("Press Enter To Exit!");
            System.Console.ReadLine();
            System.Console.WriteLine("Goodbye!");
        }

    }

    /// curl 'https://discordapp.com/api/v6/invite/pZ49yu?with_counts=true'
    /// curl 'https://discordapp.com/api/v6/invite/pZ49yu?with_counts=true' -H 'x-fingerprint: 454374827680202765.4bIZHz0p-9ehg50j3yJijAdP0hs' -H 'accept-encoding: gzip, deflate, br' -H 'accept-language: en-GB' -H 'user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.108 Safari/537.36' -H 'accept: /' -H 'referer: https://discordapp.com/invite/pZ49yu' -H 'authority: discordapp.com' -H 'cookie: __cfduid=deaef43a25c69b76878433dc4dcdacd5e1528402014' --compressed
}
