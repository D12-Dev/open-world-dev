using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OpenWorldRedux
{
    public class OW_MPConfirmBarter : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 150f);

        private string windowTitle = "Warning!";
        private string windowDescription = "Are you sure you want to barter these items?";

        private float buttonX = 137f;
        private float buttonY = 38f;

        private string invoker;

        private bool fromOtherSide;

        public OW_MPConfirmBarter(string invoker, bool fromOtherSide)
        {
            this.invoker = invoker;
            this.fromOtherSide = fromOtherSide;

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
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
            TradeHandler.TakeItemsForTrade(TradeHandler.TakeMode.Barter);

            if (!fromOtherSide)
            {
                TradeHandler.SendTrades();
            }

            else
            {
                List<string> contents = new List<string>();
                contents.Add(invoker);
                contents.Add(((int)TradeCache.takeMode).ToString());

                foreach (TradeItem thing in TradeCache.tradeItems)
                {
                    contents.Add(Serializer.SoftSerialize(thing));
                }

                Packet SendThingsPacket = new Packet("SendThingsPacket", contents.ToArray());
                Network.SendData(SendThingsPacket);

                Find.WindowStack.Add(new OW_WaitingDialog());

                FocusCache.barterMenuInstance.Close();
                FocusCache.barterRequestInstance.Close();
            }

            Close();
        }

        private void OnCancel()
        {
            Close();
        }
    }
}
