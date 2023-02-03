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

        public string ipText;
        public string portText;

        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        public override Vector2 InitialSize => new Vector2(350f, 280f);

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

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float ipEntryTextDif = Text.CalcSize(ipEntryText).y + StandardMargin;
            float ipEntryDif = ipEntryTextDif + 30f;

            float portEntryTextDif = ipEntryDif + Text.CalcSize(portEntryText).y + StandardMargin * 2;
            float portEntryDif = portEntryTextDif + 30f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Text.Font = GameFont.Small;

            Widgets.Label(new Rect(centeredX - Text.CalcSize(ipEntryText).x / 2, ipEntryTextDif, Text.CalcSize(ipEntryText).x, Text.CalcSize(ipEntryText).y), ipEntryText);
            string a = Widgets.TextField(new Rect(centeredX - (200f / 2), ipEntryDif, 200f, 30f), ipText);
            if (AcceptsInput && a.Length <= 32) ipText = a;

            Widgets.Label(new Rect(centeredX - Text.CalcSize(portEntryText).x / 2, portEntryTextDif, Text.CalcSize(portEntryText).x, Text.CalcSize(portEntryText).y), portEntryText);
            string b = Widgets.TextField(new Rect(centeredX - (200f / 2), portEntryDif, 200f, 30f), portText);
            if (AcceptsInput && b.Length <= 5 && b.All(character => Char.IsDigit(character))) portText = b;

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Connect"))
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

                    Close();
                }
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                Close();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - 32f, rect.y), new Vector2(32f, 32f)), "E"))
            {
                DoFloatMenu();
            }
        }

        private void DoFloatMenu()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            List<Tuple<string, string, string>> savedServers = new List<Tuple<string, string, string>>()
            {
                Tuple.Create("Official Vanilla+ Server", "109.123.250.81", "25555"),
                Tuple.Create("Official Free For All Server", "109.123.250.81", "25556"),
                Tuple.Create("Local Host", "128.0.0.1", "25555")
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
        }
    }
}