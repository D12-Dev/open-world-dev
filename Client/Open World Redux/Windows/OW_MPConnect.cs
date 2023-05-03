using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;
using Verse.Profile;
using System.IO;

namespace OpenWorldRedux
{
    public class OW_MPConnect : Window
    {
        private string windowTitle = "Multiplayer Menu";
        private string ipEntryText = "Server IP";
        private string portEntryText = "Server Port";

        private int startAcceptingInputAtFrame;

        private float buttonX = 150f;
        private float buttonY = 38f;
        public string PageSelected = "Official";
        public string ipText;
        public string portText;
        public static Texture2D ButtonTexture;
        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        public override Vector2 InitialSize => new Vector2(1000f, 500f);
        [StaticConstructorOnStartup]
        static class UIHelper
        {
            static UIHelper()
            {
                ButtonTexture = ContentFinder<Texture2D>.Get("UI/ServerDisplayBar");
            }
        }
        public OW_MPConnect()
        {
            if (File.Exists(FocusCache.loginDataFilePath))
            {
                LoginDataFile previousLoginData = Serializer.DeserializeFromFile<LoginDataFile>(FocusCache.loginDataFilePath);

                ipText = previousLoginData.ServerIP;
                portText = previousLoginData.ServerPort;
            }

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }


        public void CreateNavBar(Rect rect) {
            float NavBarOffset = 225f;
            if (Widgets.ButtonText(new Rect(new Vector2((rect.xMin + 256f) + NavBarOffset, rect.y), new Vector2(128f, 32f)), "Direct Connect"))
            {
                PageSelected = "DirectConnect";
            }
            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMin + NavBarOffset, rect.y), new Vector2(128f, 32f)), "Offical Servers"))
            {
                PageSelected = "Official";
            }
            if (Widgets.ButtonText(new Rect(new Vector2((rect.xMin + 128f) + NavBarOffset, rect.y), new Vector2(128f, 32f)), "Unoffical Servers"))
            {
                PageSelected = "Unofficial";
            }
            if (Widgets.ButtonText(new Rect(new Vector2((rect.xMin + 384f) + NavBarOffset, rect.y), new Vector2(128f, 32f)), "Local Host"))
            {
                PageSelected = "Local";
            }

        }
        public void CreateCurrentTab(Rect rect) {
            float centeredX = rect.width / 2;

            float ipEntryTextDif = Text.CalcSize(ipEntryText).y + StandardMargin;
            float ipEntryDif = ipEntryTextDif + 30f;

            float portEntryTextDif = ipEntryDif + Text.CalcSize(portEntryText).y + StandardMargin * 2;
            float portEntryDif = portEntryTextDif + 30f;
            if (PageSelected == "Official")
            {
                CreateOfficalServersListTab(rect);
            }

            if (PageSelected == "DirectConnect") {
                CreateDirectConnectTab(rect, ipText, portText);
                Widgets.Label(new Rect(centeredX - Text.CalcSize(ipEntryText).x / 2, ipEntryTextDif, Text.CalcSize(ipEntryText).x, Text.CalcSize(ipEntryText).y), ipEntryText);
                string a = Widgets.TextField(new Rect(centeredX - (200f / 2), ipEntryDif, 200f, 30f), ipText);
                if (AcceptsInput && a.Length <= 32) ipText = a;

                Widgets.Label(new Rect(centeredX - Text.CalcSize(portEntryText).x / 2, portEntryTextDif, Text.CalcSize(portEntryText).x, Text.CalcSize(portEntryText).y), portEntryText);
                string b = Widgets.TextField(new Rect(centeredX - (200f / 2), portEntryDif, 200f, 30f), portText);
                if (AcceptsInput && b.Length <= 5 && b.All(character => Char.IsDigit(character))) portText = b;


            }
            if (PageSelected == "Local")
            {

                CreateDirectConnectTab(rect, ipText, portText);
                Widgets.Label(new Rect(centeredX - Text.CalcSize(ipEntryText).x / 2, ipEntryTextDif, Text.CalcSize(ipEntryText).x, Text.CalcSize(ipEntryText).y), ipEntryText);
                string a = Widgets.TextField(new Rect(centeredX - (200f / 2), ipEntryDif, 200f, 30f), "127.0.0.1");
                if (AcceptsInput && a.Length <= 32) ipText = a;

                Widgets.Label(new Rect(centeredX - Text.CalcSize(portEntryText).x / 2, portEntryTextDif, Text.CalcSize(portEntryText).x, Text.CalcSize(portEntryText).y), portEntryText);
                string b = Widgets.TextField(new Rect(centeredX - (200f / 2), portEntryDif, 200f, 30f), "25555");
                if (AcceptsInput && b.Length <= 5 && b.All(character => Char.IsDigit(character))) portText = b;
            }
            if (PageSelected == "Unofficial")
            {
                Widgets.Label(new Rect((float)(rect.width / 2.75), rect.height / 2,rect.width, rect.height), "This Feature Is Currently Work in Progress!");

            }
        }
        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float ipEntryTextDif = Text.CalcSize(ipEntryText).y + StandardMargin;
            float ipEntryDif = ipEntryTextDif + 30f;

            float portEntryTextDif = ipEntryDif + Text.CalcSize(portEntryText).y + StandardMargin * 2;
            float portEntryDif = portEntryTextDif + 30f;

            Text.Font = GameFont.Medium;
          //  Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Text.Font = GameFont.Small;
            Log.Message(PageSelected);
            CreateNavBar(rect);
            CreateCurrentTab(rect);




        }
        // Used to create an option menu
        /*        private void DoFloatMenu()
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    List<Tuple<string, string, string>> savedServers = new List<Tuple<string, string, string>>()
                    {
                        Tuple.Create("Official Vanilla+ Server", "173.212.246.147", "3333"),
                        Tuple.Create("Official Free For All Server", "173.212.246.147", "3334"),
                        Tuple.Create("Local Host", "127.0.0.1", "25555"),
                        Tuple.Create("Local Host2", "128.0.0.1", "25555")
                    };

                    foreach (Tuple<string, string, string> tuple in savedServers)
                    {
                        FloatMenuOption item = new FloatMenuOption(tuple.Item1, delegate
                        {
                            ipText = tuple.Item2;
                            portText= tuple.Item3;
                        });

                        list.Add(item);
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }*/
        public void CreateOfficalServersListTab(Rect rect) {
            float IncreX = 0f; 
            float IncreY = rect.yMax / 7;
            float ButtonWidth = (float)(rect.width * 0.975);
            float ButtonHeight = rect.height / 10;
            Log.Message(ButtonWidth.ToString() + "   " +  ButtonHeight.ToString());
            List<Tuple<string, string, string, string >> ServerList = GetServersFromMasterServer();

            foreach (Tuple<string, string, string, string > Server in ServerList) {
                if (Widgets.ButtonImage(new Rect(new Vector2((float)IncreX, IncreY), new Vector2(ButtonWidth, ButtonHeight)), ButtonTexture)) 
                {
                    if (BooleanCache.isTryingToConnect) return;
                    Network.ip = Server.Item2;
                    Network.port = Server.Item3;
                    Threading.GenerateThreads(0);
                    Find.WindowStack.currentlyDrawnWindow.Close();


                }
                Widgets.Label(new Rect(new Vector2((float)(rect.width *0.05), IncreY + (ButtonHeight/4)), new Vector2((float)(ButtonWidth * 0.2), ButtonHeight)) , Server.Item1);
                Widgets.Label(new Rect(new Vector2((float)(rect.width * 0.05) + (float)(ButtonWidth *0.2), IncreY + (ButtonHeight / 4)), new Vector2((float)(ButtonWidth * 0.8), ButtonHeight)), Server.Item4);
                IncreY += ButtonHeight;

            }
        
        
        }
        public List<Tuple<string, string, string, string>> GetServersFromMasterServer() {
            List<Tuple<string, string, string, string>> ServerList = new List<Tuple<string, string, string, string>>()
            {
                Tuple.Create("Official Vanilla+ Server", "173.212.246.147", "3333", "Offical Server with a set mod list found in the discord."),
                Tuple.Create("Official Free For All Server", "173.212.246.147", "3334", "Offical Server which allows any mods."),

            };

            return ServerList;
        }
        

 
        public void CreateDirectConnectTab(Rect rect, string ipText, string portText)
        {
            if (Widgets.ButtonText(new Rect(new Vector2((float)(rect.xMax / 2.4), ((float)(rect.yMax / 2)) - buttonY), new Vector2(buttonX, buttonY)), "Connect"))
            {
                if (BooleanCache.isTryingToConnect) return;

                bool isMissingInfo = false;

                if (string.IsNullOrWhiteSpace(ipText)) isMissingInfo = true;
                if (string.IsNullOrWhiteSpace(portText)) isMissingInfo = true;

                if (isMissingInfo)
                {
                    Find.WindowStack.Add(new OW_ErrorDialog("Login details are missing or incorrect"));
                    return;
                }

                else
                {
                    Network.ip = ipText;
                    Network.port = portText;

                    LoginDataFile newLoginData;
                    if (File.Exists(FocusCache.loginDataFilePath)) newLoginData = Serializer.DeserializeFromFile<LoginDataFile>(FocusCache.loginDataFilePath);
                    else newLoginData = new LoginDataFile();

                    newLoginData.ServerIP = ipText;
                    newLoginData.ServerPort = portText;
                    Serializer.SerializeToFile(FocusCache.loginDataFilePath, newLoginData);

                    Threading.GenerateThreads(0);
                    Find.WindowStack.currentlyDrawnWindow.Close();
                    // Close();
                }
            }
        }
    }


}

