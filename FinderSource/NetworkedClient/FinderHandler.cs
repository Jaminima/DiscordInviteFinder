using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

namespace NetworkedClient
{
    public static class FinderHandler
    {
        public static Boolean IsRunning = false;
        static int Steps = 0;
        static int InvitesFound = 0;
        public static List<String>
            StartAt = new List<string> { "a", "a", "a", "a", "a", "a" },
            EndAt = new List<string> { "9", "9", "9", "9", "9", "9" },
            Code;

        static List<String> Chars = new List<string> {
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "0","1","2","3","4","5","6","7","8","9" };

        public static Thread T = new Thread(() => Run());
        public static void Start()
        {
            if (!T.IsAlive) { T = new Thread(() => Run()); T.Start(); }
        }
        static List<Thread> Threads = new List<Thread> { };
        static long StartTime;
        public static async void Run()
        {
            Code = StartAt;
            string EndCode = EndAt[0] + EndAt[1] + EndAt[2] + EndAt[3] + EndAt[4]+EndAt[5],StrCode = Code[0] + Code[1] + Code[2] + Code[3] + Code[4] + Code[5];
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Terminate);
            StartCheckCode("nfa8Yk");
            IsRunning = true;
            ServicePointManager.ReusePort = true;
            client.DefaultRequestHeaders.ConnectionClose = true;
            while (StrCode != EndCode && IsRunning)
            {
                if (Threads.Count < 1)
                {
                    Threads.Add(new Thread(() => StartCheckCode(StrCode)));
                    Threads[Threads.Count - 1].Start();
                    //await CheckCode(StrCode);
                    Code = IterateCode(Code);
                    StrCode = Code[0] + Code[1] + Code[2] + Code[3] + Code[4] + Code[5];
                }
                if (Threads.Count > 0)
                {
                    for (int i = 0; i < Threads.Count; i++) { if (!Threads[i].IsAlive) { Threads.RemoveAt(i); } }
                }
                await UpdateDisplay(StrCode);
            }
            if (IsRunning) { NetworkHandler.SendMessage(new List<string> { "Goodbye" }); NetworkHandler.SendMessage(new List<string> { "Hello" }); } 
        }

        static async Task UpdateDisplay(string StrCode)
        {
            if (DateTime.UtcNow.Ticks - StartTime >= 10000000 && IsRunning)
            {
                Console.Write("\rInvites Found: " + InvitesFound + " Codes Per Second: " + Steps + " Current Code: " + StrCode + "...."+Threads.Count);
                NetworkHandler.SendMessage(new List<string> { "Steps", Steps.ToString() });
                StartTime = DateTime.UtcNow.Ticks; Steps = 0;
            }
        }

        static async void StartCheckCode(string Code)
        {
            await CheckCode(Code);
            System.Threading.Thread.CurrentThread.Abort();
        }

        static List<String> ValidCodes=new List<string> {  };
        static HttpClient client = new HttpClient() { MaxResponseContentBufferSize = 1000 };

        static async Task CheckCode(string Code)
        {
            Steps++;
            try { String Conn = await client.GetStringAsync("https://discordapp.com/api/v6/invite/" + Code); } catch (Exception E) { if (!E.Message.Contains("404")) { Console.WriteLine(E.Message); } return; }
            ValidCodes.Add(Code); NetworkHandler.SendMessage(new List<string> { "ValidCode", Code }); InvitesFound++;
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
            if (IsRunning)
            {
                IsRunning = false;
                Console.Write("\nWaiting For Work To Finish...");
                NetworkHandler.SendMessage(new List<string> { "Goodbye" });
                System.Console.WriteLine("\nPress `Enter` To Exit!");
                System.Console.ReadLine();
            }
        }

    }
}
