
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public class OW_MPConfirmAid : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 150f);

        private string windowTitle = "WARNING";
        private string windowDescription = $"Using this costs {AidHandler.aidUsageCost} silver. Continue?";

        private float buttonX = 137f;
        private float buttonY = 38f;

        public OW_MPConfirmAid()
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

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + (StandardMargin / 2);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "OK"))
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
            if (CaravanInventoryUtility.HasThings(FocusCache.focusedCaravan, ThingDefOf.Silver, AidHandler.aidUsageCost))
            {
                string[] contents = new string[] { FocusCache.focusedCaravan.Tile.ToString() };
                Packet SendAidPacket = new Packet("SendAidPacket", contents);
                Network.SendData(SendAidPacket);

                Find.WindowStack.Add(new OW_WaitingDialog());
                FocusCache.aidMenuInstance.Close();
                Close();
            }

            else
            {
                Find.WindowStack.Add(new OW_ErrorDialog("You don't have enough funds for this action"));
                FocusCache.aidMenuInstance.Close();
                Close();
            }
        }

        private void OnCancel()
        {
            Close();
        }
    }
}
