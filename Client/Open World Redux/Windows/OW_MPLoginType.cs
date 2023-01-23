﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux
{
    public class OW_MPLoginType : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 250f);

        private string windowTitle = "Multiplayer";
        private string windowDescription = "Please choose your login mode";

        private float buttonX = 200f;
        private float buttonY = 38f;

        public OW_MPLoginType()
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = false;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2, rect.yMax - buttonY * 3 - 20f), new Vector2(buttonX, buttonY)), "Login"))
            {
                Find.WindowStack.Add(new OW_MPLogin());
                Close();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2, rect.yMax - buttonY * 2 - 10f), new Vector2(buttonX, buttonY)), "Register"))
            {
                Find.WindowStack.Add(new OW_MPRegister());
                Close();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                Close();

                Network.DisconnectFromServer();
            }
        }
    }
}
