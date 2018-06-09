using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace DiscordInviteFinder
{
    public static class FinderHandler
    {
        static List<Thread> Threads = new List<Thread> { };

        static Boolean IsRunning = false;
        static int Steps = 0;

        static List<String>
            StartAt = new List<string> { "a", "a", "a", "a", "a", "a" },
            EndAt = new List<string> { "9", "9", "9", "9", "9", "9" },
            Code;

        static List<String> Chars = new List<string> {
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "0","1","2","3","4","5","6","7","8","9" };

        static long StartTime;
        public static void Run()
        {
            IsRunning = true;
            Code = StartAt;
            while (Code != EndAt)
            {
                string StrCode = Code[0] + Code[1] + Code[2] + Code[3] + Code[4] + Code[5];
                Threads.Add(new Thread(() => CheckCode(StrCode)));
                Threads[Threads.Count - 1].Start();
                Code = IterateCode(Code);

                if (Threads.Count >= 1000 && !IsRunning) { for (int i = 0; i < Threads.Count; i++) { if (Threads[i].IsAlive == false && !IsRunning) { Threads.RemoveAt(i); } } }
                if (DateTime.UtcNow.Ticks - StartTime >= 10000000) { Console.WriteLine("\rCodes Per Second: " + Steps + " Current Code: " + Code + "...."); StartTime = DateTime.UtcNow.Ticks; Steps = 0; }
            }
        }

        static List<String> ValidCodes=new List<string> { };
        static void CheckCode(string Code)
        {
            if (IsValidCode(Code))
            {ValidCodes.Add(Code);}
            Steps++;
        }

        static WebClient wb = new WebClient();
        static Boolean IsValidCode(string Code)
        {
            Boolean Response = false;
            string RequestData="";
            try { RequestData= System.Text.Encoding.UTF8.GetString(wb.DownloadData("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true"));  }
            catch { Response = false; }
            if (RequestData.Contains(Code)) { Response = true; }
            return Response;
        }

        static List<String> IterateCode(List<String> Code)
        {
            Boolean PreviousOverflowed = false;
            for (int i = 0; i < 6; i++)
            {
                try {
                    Code[i] = Chars[Chars.FindIndex(x => x.StartsWith(Code[i])) + 1];
                    PreviousOverflowed = false; }
                catch{
                    Code[i] = Chars[0];
                    PreviousOverflowed = true;}
            }
            return Code;
        }

    }
}
