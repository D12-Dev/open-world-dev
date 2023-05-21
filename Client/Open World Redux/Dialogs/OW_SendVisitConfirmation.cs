using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;
using Steamworks;
using Multiplayer;
using System.Reflection;
using Multiplayer.Client;
using OpenWorldRedux.RTSE;

namespace OpenWorldRedux
{
    public class OW_SendVisitConfirmation : Window
    {
        public static string savedpeerstring;
        public override Vector2 InitialSize => new Vector2(400f, 150f);

        private string windowTitle = "Visit Confirmation";
        private string windowDescription = "";

        private float buttonX = 150f;
        private float buttonY = 38f;
        public string RequestFromUser;
        public string caravanitemsandpawnsfinal;
        public OW_SendVisitConfirmation(string Fromuser, string caravanitemsandpawns)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = true;
            closeOnAccept = false;
            closeOnCancel = false;
            string description = "Are you sure you want to accept? ";
            windowDescription = description;
            RequestFromUser = Fromuser;
            caravanitemsandpawnsfinal = caravanitemsandpawns;
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

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2 + 100, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Yes"))
            {
                Log.Message("yes called");
                string packetType = "VisitAccept";
                Log.Message("About to update tradeables");
                OnVisitAccept.UpdateTradeableItems(caravanitemsandpawnsfinal);
                Log.Message("Getting steamid");
                string Steamid = Steamworks.SteamUser.GetSteamID().ToString();
                Log.Message("Got steam id");
                BooleanCache.ishostingrtseserver = true;
                Log.Message("set is hosting");
                savedpeerstring = ColonistBar_CheckRecacheEntries.savedlastcaravan;
                Log.Message("setting saved last caravan");
                TryHost();
                Log.Message("Called try host");
                string[] contents = new string[] { RequestFromUser, Steamid + ":" + ColonistBar_CheckRecacheEntries.savedlastcaravan };
                Log.Message("Set contents");
                Log.Message("Sent: " + packetType + " " + contents[0]);
                Packet NewMsgPacket = new Packet(packetType, contents);
                Network.SendData(NewMsgPacket);
                
                Close();
            }
            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2 - 100, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "No"))
            {
                Close();
            }
        }
        public static void TryHost()
        {
            Log.Message("Starting Host Attempt. ");
            HostWindow hostWindowInstance = new HostWindow();
            HostWindowHelper hostWindowHelper = new HostWindowHelper();
            hostWindowHelper.CallTryHost(hostWindowInstance);
        }
    }
}
namespace Multiplayer.Client
{

    public class HostWindowHelper
    {

        public void CallTryHost(HostWindow hostWindowInstance)
        {
            Multiplayer.settings.serverSettings.steam = true;
            Multiplayer.settings.serverSettings.pauseOnJoin = false;
            Multiplayer.settings.serverSettings.syncConfigs = false;
            Multiplayer.settings.showCursors = false;
            Multiplayer.settings.autoAcceptSteam = true;
            MethodInfo privateMethod = typeof(HostWindow).GetMethod("TryHost", BindingFlags.NonPublic | BindingFlags.Instance);
            privateMethod.Invoke(hostWindowInstance, null);

        }
    }
}