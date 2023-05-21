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
    public class OW_MPPasswordCheck : Window
    {
        private string windowTitle = "Password Input";
        private string passwordEntryText = "Password";
        private string passwordHideText = "";
        public string passwordText;
        public string passwordConfirmText;

        private int startAcceptingInputAtFrame;

        private float buttonX = 150f;
        private float buttonY = 38f;

        public string usernameText;

        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;
        private bool hidingPassword = true;


        public override Vector2 InitialSize => new Vector2(350f, 400f);

        public OW_MPPasswordCheck()
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

            float passwordEntryTextDif = Text.CalcSize(passwordEntryText).y + StandardMargin * 2;
            float passwordEntryDif = passwordEntryTextDif + 30f;


            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Text.Font = GameFont.Small;

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


            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Play"))
            {
                OnPasswordInput();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Back"))
            {
                OnBack();
            }

        }

        private void OnPasswordInput()
        {
            if (CheckForMissingInfo())
            {
                Find.WindowStack.Add(new OW_ErrorDialog("Password is missing or incorrect"));
                return;
            }

            else
            {
                string[] contents = new string[] { passwordText };
                Packet PasswordClientPacket = new Packet("PasswordCheckToServer", contents);
                Network.SendData(PasswordClientPacket);
                Close();
                Find.WindowStack.Add(new OW_WaitingDialog());
            }
        }

        private void OnBack()
        {
            Close();

            Find.WindowStack.Add(new OW_MPLoginType());
        }



        private bool CheckForMissingInfo()
        {
            bool isMissingInfo = false;

            if (string.IsNullOrWhiteSpace(passwordText)) isMissingInfo = true;

            return isMissingInfo;
        }
    }
}