using RimWorld;
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
    public class OW_ChainErrorDialog : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 150f);

        private string windowTitle = "ERROR";
        private string[] windowDescription;
        private int chainCounter;

        private float buttonX = 150f;
        private float buttonY = 38f;

        public OW_ChainErrorDialog(string[] description)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = false;

            windowDescription = description;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float horizontalLineDif = Text.CalcSize(windowDescription[chainCounter]).y + StandardMargin / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription[chainCounter]).y + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription[chainCounter]).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription[chainCounter]).x, Text.CalcSize(windowDescription[chainCounter]).y), windowDescription[chainCounter]);

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "OK"))
            {
                if (chainCounter < windowDescription.Length - 1) chainCounter++;
                else Close();
            }
        }
    }
}
