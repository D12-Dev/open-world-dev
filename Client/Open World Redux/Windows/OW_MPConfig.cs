using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OpenWorldRedux
{
    public class OW_MPConfig : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 310f);

        private float buttonX = 150f;
        private float buttonY = 38f;

        string windowTitle = "Multiplayer Parameters";

        public OW_MPConfig()
        {
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float horizontalLineDif = Text.CalcSize(windowTitle).y + (StandardMargin / 4) + 3f;
            float horizontalLineDif2 = rect.y + 225f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - (Text.CalcSize(windowTitle).x / 2), rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Rect t1 = new Rect(new Vector2(rect.x, rect.y + 40f), new Vector2(InitialSize.x, 25f));
            Rect t2 = new Rect(new Vector2(rect.x, rect.y + 70f), new Vector2(InitialSize.x, 25f));

            Widgets.CheckboxLabeled(t1, "Auto Deny Trades", ref TradeCache.inTrade, placeCheckboxNearText: true);
            Widgets.CheckboxLabeled(t2, "Very Secret Stuff", ref BooleanCache.secretFlag, placeCheckboxNearText: true);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif2, rect.width);

            Rect buttonRect = new Rect(new Vector2(rect.x + (buttonX / 2), rect.y + 235f), new Vector2(buttonX, buttonY));
            if (Widgets.ButtonText(buttonRect, "Close")) Close();
        }
    }
}
