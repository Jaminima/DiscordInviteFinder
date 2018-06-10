using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkedClient
{
    static class Program
    {
        static List<String> LBound = new List<string> { }, UBound = new List<string> { };
        static void Main()
        {
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
            }
            if (Data[0] == "Restart" || Data[0]=="Start")
            {
                FinderHandler.IsRunning = false;
                FinderHandler.Run();
            }
            if (Data[0] == "ValidCode")
            {
                Console.WriteLine("You Found A Code!!!\n");
            }
        }
    }

}
