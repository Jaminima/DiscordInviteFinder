using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkedClient
{
    static class Program
    {
        static void Main()
        {
            NetworkHandler.Start(Handler);
        }

        public static void Handler(String[] Data)
        {

        }
    }

}
