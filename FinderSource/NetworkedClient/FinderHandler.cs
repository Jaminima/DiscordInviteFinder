using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace NetworkedClient
{
    public static class FinderHandler
    {
        static List<Thread> Threads = new List<Thread> { };

        public static Boolean IsRunning = false;
        static int Steps = 0;

        public static List<String>
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
            Code = StartAt;
            string EndCode = EndAt[0] + EndAt[1] + EndAt[2] + EndAt[3] + EndAt[4]+EndAt[5],StrCode = Code[0] + Code[1] + Code[2] + Code[3] + Code[4] + Code[5];
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Terminate);
            IsRunning = true;
            while (StrCode!=EndCode)
            {
                StrCode = Code[0] + Code[1] + Code[2] + Code[3] + Code[4] + Code[5];
                if (IsRunning)
                {
                    Threads.Add(new Thread(() => CheckCode(StrCode)));
                    Threads[Threads.Count - 1].Priority = ThreadPriority.BelowNormal;
                    Threads[Threads.Count - 1].Start();
                    Code = IterateCode(Code);
                }

                if (Threads.Count >= 1000) { for (int i = 0; i < Threads.Count; i++) { if (Threads[i].IsAlive == false) { Threads.RemoveAt(i); } } }
                if (DateTime.UtcNow.Ticks - StartTime >= 10000000 && IsRunning) { Console.Write("\rCodes Per Second: " + Steps + " Current Code: " + StrCode + "...."); StartTime = DateTime.UtcNow.Ticks; Steps = 0; }
            }
            NetworkHandler.SendMessage(new List<string> { "Goodbye" });
            NetworkHandler.SendMessage(new List<string> { "Hello" });
        }

        static List<String> ValidCodes=new List<string> { };
        static void CheckCode(string Code)
        {
            if (IsValidCode(Code))
            { ValidCodes.Add(Code); NetworkHandler.SendMessage(new List<string> { "ValidCode", Code }); }
            Steps++;
        }

        static WebClient wb = new WebClient();
        static Boolean IsValidCode(string Code)
        {
            try {wb.DownloadString("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true"); return true;  }
            catch { return false; }
        }

        static List<String> IterateCode(List<String> Code)
        {
            Boolean PreviousOverflowed = false;
            for (int i = 0; i < 6; i++)
            {
                if (PreviousOverflowed || i == 0) { 
                    try {
                        Code[i] = Chars[Chars.FindIndex(x => x.StartsWith(Code[i])) + 1];
                        PreviousOverflowed = false; }
                    catch {
                        Code[i] = Chars[0];
                        PreviousOverflowed = true; }
                }
            }
            return Code;
        }

        public static void Terminate(object sender, ConsoleCancelEventArgs args)
        {
            IsRunning = false;
            Console.Write("\nWaiting For Work To Finish...");
            while (Threads.Count > 0) { 
                for (int i = 0; i < Threads.Count; i++) { if (Threads[i].IsAlive == false) { Threads.RemoveAt(i); } } }
            NetworkHandler.SendMessage(new List<string> { "Goodbye" });
            System.Console.WriteLine("\nPress `Enter` To Exit!");
            System.Console.ReadLine();
        }

    }
}
