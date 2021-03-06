﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Timers;

namespace NetworkedServer
{
    class Program
    {
        static double Combinations = Math.Pow(62, 6),C=Combinations;
        public static int Steps = 0;
        static List<String> Chars = new List<string> {
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "0","1","2","3","4","5","6","7","8","9" };

        static string SpeedLocation = "";
        static string CodesLocation = "";
        static string DmsLocation = "";

        static long StartTime;
        static Timer timer;
        static void Main(string[] args)
        {
            ConfigHandler.Load();
            SpeedLocation = (string)ConfigHandler.Config["Speed"];
            CodesLocation = (string)ConfigHandler.Config["ValidCodes"];
            DmsLocation = (string)ConfigHandler.Config["Dms"];
            DiscordAPI.Events.Start();
            NetworkHandler.Start(Handler);
            Console.WriteLine("Running");
            timer = new System.Timers.Timer(6000);
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            while (true)
            {
                Console.ReadLine();
            }
        }

        static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            RemoveInvalidCodes();
            CheckAliveClients();
        }

        static void CheckAliveClients()
        {
            for (int i= 0;i<ClientIPs.Count;i++)
            {
                if (ClientIPs[i].TimeSinceLast >= 5) { for (int l = 0; l < Bounds.Count; l++) { if (Bounds[l].IP == ClientIPs[i].IP) { Bounds.RemoveAt(l); } } ClientIPs.RemoveAt(i); if (ClientIPs.Count == 0) { Console.WriteLine("\rNo Clients                             "); System.IO.File.WriteAllText(SpeedLocation, "0"); } }
                else { ClientIPs[i].TimeSinceLast++; }
            }
        }

        static string MessageLocation = (String)ConfigHandler.Config["MessageDir"];
        static List<ClientData> ClientIPs = new List<ClientData> { };
        static void Handler(string[] Content)
        {
            if (Content.Length == 0) { return; }
            if (Content[1] == "ValidCode")
            {
                CheckCode(Content[2], Content[0]);
            }
            if (Content[1] == "Hello")
            {
                ClientIPs.Add(new ClientData(Content[0]));
                NetworkHandler.SendMessage(Content[0], new List<string> { "Start" });
            }
            if (Content[1] == "GetBounds")
            {
                string[] Bounds = CreateBounds(Content[0]);
                NetworkHandler.SendMessage(Content[0], new List<string> { "Bounds", Bounds[0], Bounds[1] });
            }
            if (Content[1] == "Goodbye")
            {
                ClientIPs.RemoveAt(ClientIPs.FindIndex(x =>x.IP==Content[0]));
                foreach (BoundsData BD in Bounds) { if (BD.IP == Content[0]) { Bounds.Remove(BD); break; } }
                if (ClientIPs.Count == 0) {
                    Console.WriteLine("\rNo Clients                                 ");
                    System.IO.File.WriteAllText(SpeedLocation,"0");
                }
            }
            if (Content[1] == "Steps")
            {
                Steps += int.Parse(Content[2]);
                if (DateTime.UtcNow.Ticks - StartTime >= 10000000) { Console.Write("\rCodes Per Second: " + Steps + " Clients: "+ClientIPs.Count+".......");
                    System.IO.File.WriteAllText(SpeedLocation, Steps.ToString());
                    StartTime = DateTime.UtcNow.Ticks; Steps = 0;
                }
                try { ClientIPs[ClientIPs.FindIndex(x => x.IP == Content[0])].TimeSinceLast = 0; }
                catch { ClientIPs.Add(new ClientData(Content[0])); }
            }
            if (Content[1] == "Understood")
            {
                System.IO.File.Delete(MessageLocation + Content[0] + ".html");
            }
        }

        static WebClient wb = new WebClient();
        static List<String> ValidCodes = new List<string> { };
        static String JoinMessage = "Your Discord Was Found Via The\\nDiscord Invite Finder\\nLearn More By Joining Our Discord\\nhttps://discord.gg/SAt84m3\\nIf you dont want your discord to be listed,\\nPlease contact an Admin on our Discord.";
        static void CheckCode(string Code, string IP)
        {
            ServicePointManager.ServerCertificateValidationCallback +=(sender, cert, chain, sslPolicyErrors) => true;
            try
            {
                if ((string)DiscordAPI.DiscordInterface.PostRequest("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true", false, "GET")["code"] == Code)
                {
                    if (DiscordAPI.DiscordInterface.PostRequest("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true", false, "GET").ContainsKey("guild"))
                    {
                        ValidCodes.Add(Code); System.IO.File.AppendAllText(CodesLocation, "\nhttps://discord.gg/" + Code);
                        string GuildID = DiscordAPI.Events.JoinServer(Code);
                        foreach (Newtonsoft.Json.Linq.JObject Room in DiscordAPI.Events.GetTextChannels(GuildID))
                        {
                            try
                            {
                                //DiscordAPI.Events.SendMessage((string)Room["id"], JoinMessage);
                                try
                                {
                                    string NewInvite = DiscordAPI.Events.CreateInvite((string)Room["id"]);
                                    System.IO.File.WriteAllText(CodesLocation, System.IO.File.ReadAllText(CodesLocation).Replace("https://discord.gg/" + Code, "https://discord.gg/" + NewInvite));
                                }
                                catch (Exception e) { Console.WriteLine("Unable To Create/Save Invite\n", e.Message); }
                                break;
                            }
                            catch { }
                        }
                        DiscordAPI.Events.LeaveServer(GuildID);
                        Console.WriteLine("Received Valid Code");
                    }
                    else
                    {
                        System.IO.File.AppendAllText(DmsLocation, "\nhttps://discord.gg/" + Code);
                        string DMID = DiscordAPI.Events.JoinDM(Code);
                        //try { DiscordAPI.Events.SendMessage(DMID, JoinMessage); } catch { }
                        string NewInvite = DiscordAPI.Events.CreateDMInvite(DMID);
                        System.IO.File.WriteAllText(DmsLocation, System.IO.File.ReadAllText(DmsLocation).Replace("https://discord.gg/" + Code, "https://discord.gg/" + NewInvite));
                        Console.WriteLine("Received Valid Dm");
                    }
                }
                else { Console.WriteLine("Received InValid Code"); }
            }catch (Exception e) { if (e.Message.Contains("404")) { Console.WriteLine("Received InValid Code"); } else { Console.WriteLine(e.Message); } }
        }

        static List<BoundsData> Bounds = new List<BoundsData> { };
        static double BoundCapacity = 1;
        static string[] CreateBounds(string IP)
        {
            String StartPoint = "aaaaaa", EndPoint = "999999";
            double n = 0;
            while (true)
            {
                if (Math.Pow(2, n) >= Bounds.Count+1) { break; } else { n++; }
            }
            double BoundFraction = 1 / Math.Pow(2, n);
            if (BoundFraction < BoundCapacity)
            {
                BoundCapacity = BoundFraction;
                for (int i = 0; i < Math.Pow(2, n)/2; i++) {
                    Bounds[i].SPi = Math.Floor(0.5 * Bounds[i].SPi);
                    Bounds[i].EPi = Math.Ceiling(0.5 * Bounds[i].EPi);
                    Bounds[i].StartPoint = IntToCode(Bounds[i].SPi);
                    Bounds[i].EndPoint = IntToCode(Bounds[i].EPi);
                    NetworkHandler.SendMessage(Bounds[i].IP, new List<string> { "ResetBounds", Bounds[i].StartPoint, Bounds[i].EndPoint });
                }
            }
            double BoundWidth = Math.Ceiling(C * BoundFraction);
            double SI = C * ((Bounds.Count) * BoundFraction),
                EI = C * ((Bounds.Count + 1) * BoundFraction)-1;
            StartPoint = IntToCode(SI);
            EndPoint = IntToCode(EI);
            Bounds.Add(new BoundsData(StartPoint,EndPoint,IP,SI,EI));
            return new string[] { StartPoint,EndPoint };
        }

        static string IntToCode(double Int)
        {
            List<Double> IntegeralPart = new List<Double> { };
            int Loops = 0;
            while (Int > 0)
            {
                IntegeralPart.Add((Int % 62));
                Int = (Math.Floor(Int / 62));
                Loops++;
            }
            string Code = "";
            foreach (double i in IntegeralPart) { Code = Code + Chars[int.Parse(i.ToString())]; }
            for (int i = Loops; i < 6; i++) { Code =  Code + "a"; }
            return Code;
        }
        static void RemoveInvalidCodes()
        {
            RemoveInvalidCodes(CodesLocation);
            RemoveInvalidCodes(DmsLocation);
        }
        static void RemoveInvalidCodes(string Location)
        {
            List<string> oldLines = new List<string>();
            List<string> newLines = new List<string>();
            oldLines = System.IO.File.ReadAllLines(Location).ToList();

            foreach (string link in oldLines)
            {
                if (IsValidCode(link.Replace("https://discord.gg/", "")))
                { if (!newLines.Contains(link)) { newLines.Add(link); } }
            }
            System.IO.File.WriteAllLines(Location, newLines);
        }
        static Boolean IsValidCode(string Code)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            try { DiscordAPI.DiscordInterface.PostRequest("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true", false, "GET"); return true; } //wb.DownloadString("https://discordapp.com/api/v6/invite/" + Code + "?with_counts=true")
            catch (Exception e) { if (e.Message.Contains("404")) { return false; } else { return true; } }
        }

    }

    class BoundsData {
        public String StartPoint, EndPoint, IP;
        public double SPi, EPi;
        public BoundsData(string SP,string EP,string lIP,double lSPi,double lEPi)
        {
            StartPoint = SP;EndPoint = EP;IP = lIP;
            SPi = lSPi;EPi = lEPi;
        }
    }

    class ClientData
    {
        public string IP;
        public int TimeSinceLast;
        public ClientData(string lIP)
        {
            IP = lIP;TimeSinceLast = 0;
        }
    }

}
