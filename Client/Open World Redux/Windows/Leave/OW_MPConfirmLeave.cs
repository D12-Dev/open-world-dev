
using Multiplayer.Client;
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
using Verse.Sound;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using Multiplayer.Common;



namespace OpenWorldRedux
{
    public class OW_MPConfirmLeave : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 150f);

        private string windowTitle = "Warning!";
        private string windowDescription = "Are you sure you want to Leave with these items?";

        private float buttonX = 137f;
        private float buttonY = 38f;

        public OW_MPConfirmLeave()
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
            Log.Message("About to reset savedlastcaravan:"  + ColonistBar_CheckRecacheEntries.savedlastcaravan);
            ColonistBar_CheckRecacheEntries.savedlastcaravan = "";
            OnSendVisitRequest.visitrequeststringfinal = null;
            foreach (Tradeable tradeable in TradeCache.listToShowInTradesMenu)
            {
                Log.Message("Tradeable: " + tradeable.ToString());
                if (tradeable.AnyThing is Pawn pawn && tradeable.CountToTransfer != 0)
                {
                    ColonistBar_CheckRecacheEntries.savedlastcaravan += OnSendVisitRequest.pawntostring(pawn);
                }
                else if (tradeable.AnyThing is Thing item && tradeable.CountToTransfer != 0)
                {
                    ColonistBar_CheckRecacheEntries.savedlastcaravan += OnSendVisitRequest.itemtostring(item, tradeable.CountToTransfer);
                }
            }
            MemoryUtility.ClearAllMapsAndWorld();
            GenScene.GoToMainMenu();
            string packetType = "LeaveNotification";
            string focusedusername = OnRecievedVisitAccept.savedmultiplayerhost;
            string[] contents = new string[] { focusedusername + ":" + ColonistBar_CheckRecacheEntries.savedlastcaravan };
            Log.Message(contents[0]);
            Packet NewMsgPacket = new Packet(packetType, contents);
            Network.SendData(NewMsgPacket);

            Comingback.iscomingbackfromsettlement = true;
            Multiplayer.Client.Multiplayer.session = null;
            BooleanCache.ishostingrtseserver = false;
            string[] contents2 = new string[] { };
            Packet ClientSaveFilePacket = new Packet("RequestFullsave", contents2);

            Network.SendData(ClientSaveFilePacket);
            //GameDataSaveLoader.LoadGame("Open World Server Save " + FocusCache.userName);
            

        }

        private void OnCancel()
        {
            Close();
        }
    }

}
