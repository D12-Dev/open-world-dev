using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public class OW_MPAidMenu : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 175f);

        private string windowTitle = "Aid Menu";
        private string windowDescription = "Pay to send colonists on aid mission";
        private string windowSubdescription = $"Aid price in silver: {AidHandler.aidUsageCost}";

        private float buttonX = 150f;
        private float buttonY = 38f;

        public OW_MPAidMenu()
        {
            FocusCache.aidMenuInstance = this;

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            float descriptionLineDif = windowDescriptionDif + Text.CalcSize(windowDescription).y;

            float windowSubdescriptionDif = descriptionLineDif + (Text.CalcSize(windowSubdescription).y / 2);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.xMin, (rect.y + Text.CalcSize(windowTitle).y + 10), rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
            Text.Font = GameFont.Medium;

            Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowSubdescription).x / 2, windowSubdescriptionDif, Text.CalcSize(windowSubdescription).x, Text.CalcSize(windowSubdescription).y), windowSubdescription);
            Text.Font = GameFont.Medium;

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMin, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Accept"))
            {
                OnAccept();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                OnCancel();
            }
        }

        private void OnAccept()
        {
            Find.WindowStack.Add(new OW_MPConfirmAid());
        }

        private void OnCancel()
        {
            Close();
        }
    }
}
