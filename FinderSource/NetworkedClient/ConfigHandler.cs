using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedClient
{
    public static class ConfigHandler
    {
        public static Newtonsoft.Json.Linq.JObject Config;
        public static void Load()
        {
            Config = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText("./Config.json"));
        }
    }
}
