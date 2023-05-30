using OpenWorldRedux.RTSE;
using RimWorld;
using RimWorld.Planet;
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
    public class OW_SendVisitRequest : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 150f);

        private string windowTitle = "Visit Request";
        private string windowDescription = "";

        private float buttonX = 150f;
        private float buttonY = 38f;
        public string settlement;
        public static Caravan lastrequest;
        public OW_SendVisitRequest(string description)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = false;

            windowDescription = description;
            settlement = description;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float horizontalLineDif = Text.CalcSize(windowDescription).y + StandardMargin / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2 + 100, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Send"))
            {
                string username = "";
                string firstusername = "";

                foreach (Settlement settlement in WorldCache.onlineSettlements)
                {
                    if (settlement.Tile == FocusCache.focusedSettlement.Tile && settlement.Name != null && settlement.Name != "" && settlement.Name != " ")
                    {
                        username = settlement.Name;
                    }
                }
                string packetType = "VisitRequest";
                    firstusername = username.Split('(')[0].Substring(0, username.Split('(')[0].Length - 13);
                Log.Message("User1: " + username);
                Log.Message("User2: " + firstusername);
                OnRecievedVisitAccept.savedmultiplayerhost = firstusername;
                string[] contents = new string[] { firstusername + ":" + OnSendVisitRequest.Pawnstostring(null, FocusCache.focusedCaravan) + OnSendVisitRequest.itemstostring(null, FocusCache.focusedCaravan) };
                Log.Message(contents[0]);
                Packet NewMsgPacket = new Packet(packetType, contents);
                lastrequest = FocusCache.focusedCaravan;
                Network.SendData(NewMsgPacket);
                Close();
            }
            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2 -100, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
            {
                Close();
            }
        }
    }
}
