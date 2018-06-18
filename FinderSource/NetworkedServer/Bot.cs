using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace NetworkedServer
{
    public static class Bot
    {

        private static string Email = "oscar.davies48@gmail.com", Password = "15PgO#V$9%!v";
        private static string Token = "";
        public static void Start()
        {
            string PostData = "{\"email\":\""+Email+"\",\"password\":\""+Password+"\",\"undelete\":false,\"captcha_key\":null}";
            Token = Post("https://discordapp.com/api/v6/auth/login", PostData).Replace("{\"token\": \"","").Replace("\"}","");
        }

        static string JoinMessage = @"Your Discord Was Found Via The\rDiscord Invite Finder\rLearn More By Joining Our Discord\rhttps://discord.gg/SAt84m3";
        public static void JoinServer(string Code)
        {
            string Data=Post("https://discordapp.com/api/v6/invite/"+Code,"");
            Newtonsoft.Json.Linq.JObject P = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(Data);
            string Join=Post("https://discordapp.com/api/v6/channels/" + (string)P["channel"]["id"] + "/messages", "{\"content\":\""+JoinMessage+"\",\"nonce\":\"618169420211337420\",\"tts\":false}");
            P = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(Post("https://discordapp.com/api/v6/channels/"+(string)P["channel"]["id"]+"/invites", "{\"max_age\":null,\"max_uses\":0,\"temporary\":false}"));
            System.IO.File.WriteAllText("./ValidCodes.dat",System.IO.File.ReadAllText("./ValidCodes.dat").Replace("https://discord.gg/" + Code, "https://discord.gg/" + P["code"]));
        }

        static string Post(string URL,string Data)
        {
            Console.WriteLine(URL);
            byte[] bData = Encoding.ASCII.GetBytes(Data);
            WebRequest Req = WebRequest.Create(URL);
            Req.Method = "POST";
            Req.ContentType = "application/json";
            if (Token != "") { Req.Headers.Add("authorization", Token); }
            Stream s = Req.GetRequestStream();
            s.Write(bData, 0, Data.Length);
            s.Close();

            WebResponse Resp = Req.GetResponse();
            s = Resp.GetResponseStream();
            StreamReader R = new StreamReader(s);
            return R.ReadToEnd();
        }
    }
}
