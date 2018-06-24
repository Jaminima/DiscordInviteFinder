using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NetworkedClient
{
    static class Program
    {
        #region "Safe Closing"
        public enum ConsoleEvent
        {
            CTRL_C = 0,
            CTRL_BREAK = 1,
            CTRL_CLOSE = 2,
            CTRL_LOGOFF = 5,
            CTRL_SHUTDOWN = 6
        }
        public delegate void ControlEventHandler(ConsoleEvent consoleEvent);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ControlEventHandler e, bool add);
        public static void OnControlEvent(ConsoleEvent consoleEvent)
        {
            FinderHandler.Terminate(null,null);
        }
        #endregion
        static List<String> LBound = new List<string> { }, UBound = new List<string> { };
        static void Main()
        {
            SetConsoleCtrlHandler(new ControlEventHandler(OnControlEvent), true);
            ConfigHandler.Load();
            NetworkHandler.Start(Handler);
            Console.WriteLine("Running");
            while (true) { Console.ReadLine(); }
        }
        
        public static void Handler(String[] Data)
        {
            if (Data[0] == "Bounds")
            {
                foreach (Char C in Data[1]) { LBound.Add(C.ToString()); }
                foreach (Char C in Data[2]) { UBound.Add(C.ToString()); }
                FinderHandler.StartAt = LBound;
                FinderHandler.EndAt = UBound;
                LBound = new List<string> { };
                UBound = new List<string> { };
                NetworkHandler.SendMessage(new List<string> { "Hello" });
            }
            if (Data[0] == "ResetBounds")
            {
                FinderHandler.IsRunning = false;
                while (FinderHandler.T.IsAlive) { }
                foreach (Char C in Data[1]) { LBound.Add(C.ToString()); }
                foreach (Char C in Data[2]) { UBound.Add(C.ToString()); }
                FinderHandler.StartAt = LBound;
                FinderHandler.EndAt = UBound;
                LBound = new List<string> { };
                UBound = new List<string> { };
                FinderHandler.Start();
            }
            if (Data[0] == "Restart" || Data[0]=="Start")
            {
                FinderHandler.IsRunning = false;
                FinderHandler.Start();
            }
        }
    }

}
