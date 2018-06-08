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
        static void Main(string[] args)
        {
            List<String> Chars = new List<string> { };
            foreach (String C in "a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z|A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|0|1|2|3|4|5|6|7|8|9".Split("|".ToCharArray())) { Chars.Add(C); }

            List<Thread> Threads=new List<Thread> { };

            //List<String> CurCode = new List<string> { "M", "B", "B", "R", "z", "s" }; //Valid Code https://discord.gg/MBBRzs
            List<String> CurCode = new List<string> { "a","a","a","a","a","a" };
            while (CurCode != new List<string> { "9", "9", "9", "9", "9", "9" })
            {
                string Code = CurCode[0] + CurCode[1] + CurCode[2] + CurCode[3] + CurCode[4] + CurCode[5];
                Threads.Add(new Thread(()=>CheckCode(Code)));
                Threads[Threads.Count - 1].Start();
                CurCode = IterateCode(CurCode, Chars);
            }
            System.Console.ReadLine();
        }

        static void CheckCode(string Code)
        {
            string Response = GetResponse(Code);
            if (Response != null)
            {
                System.IO.File.AppendAllText("./Valid.dat", "\n" + Code);
                System.Console.WriteLine(Code);
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

    }

    /// curl 'https://discordapp.com/api/v6/invite/pZ49yu?with_counts=true'
    /// curl 'https://discordapp.com/api/v6/invite/pZ49yu?with_counts=true' -H 'x-fingerprint: 454374827680202765.4bIZHz0p-9ehg50j3yJijAdP0hs' -H 'accept-encoding: gzip, deflate, br' -H 'accept-language: en-GB' -H 'user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.108 Safari/537.36' -H 'accept: /' -H 'referer: https://discordapp.com/invite/pZ49yu' -H 'authority: discordapp.com' -H 'cookie: __cfduid=deaef43a25c69b76878433dc4dcdacd5e1528402014' --compressed
}
