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
    public class OW_MPRegister : Window
    {
        private string windowTitle = "Registration Menu";
        private string usernameEntryText = "Username";
        private string passwordEntryText = "Password";
        private string passwordHideText = "";
        private string passwordConfirmEntryText = "Confirm Password";
        private string passwordConfirmHideText = "";

        public string passwordText;
        public string passwordConfirmText;

        private int startAcceptingInputAtFrame;

        private float buttonX = 150f;
        private float buttonY = 38f;

        public string usernameText;

        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;
        private bool hidingPassword = true;
        private bool spawnedWarningMessage;

        public override Vector2 InitialSize => new Vector2(350f, 400f);

        public OW_MPRegister()
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float usernameEntryTextDif = Text.CalcSize(usernameEntryText).y + StandardMargin * 2;
            float userEntryDif = usernameEntryTextDif + 30f;

            float passwordEntryTextDif = userEntryDif + Text.CalcSize(passwordEntryText).y + StandardMargin * 2;
            float passwordEntryDif = passwordEntryTextDif + 30f;

            float passwordConfirmEntryTextDif = passwordEntryDif + Text.CalcSize(passwordConfirmEntryText).y + StandardMargin * 2;
            float passwordConfirmEntryDif = passwordConfirmEntryTextDif + 30f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Text.Font = GameFont.Small;

            Widgets.Label(new Rect(centeredX - Text.CalcSize(usernameEntryText).x / 2, usernameEntryTextDif, Text.CalcSize(usernameEntryText).x, Text.CalcSize(usernameEntryText).y), usernameEntryText);
            string c = Widgets.TextField(new Rect(centeredX - (200f / 2), userEntryDif, 200f, 30f), usernameText);
            if (AcceptsInput && c.Length <= 24 && c.All(character => Char.IsLetterOrDigit(character) || character == '_' || character == '-')) usernameText = c;

            Widgets.Label(new Rect(centeredX - Text.CalcSize(passwordEntryText).x / 2, passwordEntryTextDif, Text.CalcSize(passwordEntryText).x, Text.CalcSize(passwordEntryText).y), passwordEntryText);
            string d = Widgets.TextField(new Rect(centeredX - (200f / 2), passwordEntryDif, 200f, 30f), passwordText);
            if (AcceptsInput && d.Length <= 24)
            {
                Text.Font = GameFont.Medium;
                if (hidingPassword) passwordHideText = new string('█', d.Length);
                else passwordHideText = "";
                Text.Font = GameFont.Small;

                passwordText = d;
            }
            string e = Widgets.TextField(new Rect(centeredX - (200f / 2), passwordEntryDif, 200f, 30f), passwordHideText);

            Widgets.Label(new Rect(centeredX - Text.CalcSize(passwordConfirmEntryText).x / 2, passwordConfirmEntryTextDif, Text.CalcSize(passwordConfirmEntryText).x, Text.CalcSize(passwordConfirmEntryText).y), passwordConfirmEntryText);
            string f = Widgets.TextField(new Rect(centeredX - (200f / 2), passwordConfirmEntryDif, 200f, 30f), passwordConfirmText);
            if (AcceptsInput && f.Length <= 24)
            {
                Text.Font = GameFont.Medium;
                if (hidingPassword) passwordConfirmHideText = new string('█', f.Length);
                else passwordConfirmHideText = "";
                Text.Font = GameFont.Small;

                passwordConfirmText = f;
            }
            string g = Widgets.TextField(new Rect(centeredX - (200f / 2), passwordConfirmEntryDif, 200f, 30f), passwordConfirmHideText);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Register"))
            {
                OnRegister();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Back"))
            {
                OnBack();
            }

            SpawnWarnMessage();
        }

        private void OnRegister()
        {
            if (CheckForMissingInfo())
            {
                Find.WindowStack.Add(new OW_ErrorDialog("Registration details are missing or incorrect"));
                return;
            }

            else
            {
                LoginDataFile newLoginData;
                if (File.Exists(FocusCache.loginDataFilePath)) newLoginData = Serializer.DeserializeFromFile<LoginDataFile>(FocusCache.loginDataFilePath);
                else newLoginData = new LoginDataFile();
                FocusCache.userName = usernameText; // Set Chat Name
                MPChat.cacheChatText.Clear(); // Clear Chat
                newLoginData.Username = usernameText;
                Serializer.SerializeToFile(FocusCache.loginDataFilePath, newLoginData);

                string[] contents = new string[] { usernameText, Hash.GetHashCode(passwordText) };
                Packet RegisterClientPacket = new Packet("RegisterClientPacket", contents);
                Network.SendData(RegisterClientPacket);
                Close();
                Find.WindowStack.Add(new OW_WaitingDialog());
            }
        }

        private void OnBack()
        {
            Close();

            Find.WindowStack.Add(new OW_MPLoginType());
        }

        private void SpawnWarnMessage()
        {
            if (!spawnedWarningMessage)
            {
                spawnedWarningMessage = true;

                string[] infoChain = new string[]
                {
                    "Accounts are directly linked with the game saves",
                    "Please take in mind that passwords are not retrievable",
                    "Make sure to remember them to avoid data loss"
                };
                Find.WindowStack.Add(new OW_ChainInfoDialog(infoChain));
            }
        }

        private bool CheckForMissingInfo()
        {
            bool isMissingInfo = false;

            if (string.IsNullOrWhiteSpace(usernameText)) isMissingInfo = true;
            if (string.IsNullOrWhiteSpace(passwordText)) isMissingInfo = true;
            if (string.IsNullOrWhiteSpace(passwordConfirmText)) isMissingInfo = true;
            if (passwordText != passwordConfirmText) isMissingInfo = true;

            return isMissingInfo;
        }
    }
}