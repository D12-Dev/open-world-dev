using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Multiplayer.Client;
using Multiplayer.Client.Networking;
using Multiplayer.Common;
using OpenWorldRedux.Handlers;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux.RTSE
{
    public static class OnRecievedVisitAccept
    {
        public static string savedmultiplayerhost;
        public static void OnVisitAccept(string steamid)
        {
            
            Log.Message("SteamID:" + steamid);
            ulong steamIdUlong;
            if (ulong.TryParse(steamid, out steamIdUlong))
            {
                //may cause item deletion
                Log.Message("deleting caravan: " + OW_SendVisitRequest.lastrequest.AllThings.Count() + " " + OW_SendVisitRequest.lastrequest.Name);
                OW_SendVisitRequest.lastrequest.Destroy();
                Steamworks.CSteamID steamId = new Steamworks.CSteamID(steamIdUlong);
                Find.TickManager.Pause();
                ServerHandlers.SaveAndSendToSever();
                string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                
                //GameDataSaveLoader.SaveGame("Open World Server Save-" + dateTime + "-" + FocusCache.userName);
                BooleanCache.ishostingrtseserver = false;
                Multiplayer.Client.Multiplayer.session = null;
                MemoryUtility.ClearAllMapsAndWorld();
                GenScene.GoToMainMenu();
                Comingback.iscomingbackfromsettlement = true;
                Log.Message("Connecting through Steam: " + steamId);
                Thread.Sleep(1000);
                try
                {
                    Multiplayer.Client.Multiplayer.session = new MultiplayerSession
                    {
                        client = new SteamClientConn(steamId) { username = Multiplayer.Client.Multiplayer.username },
                        steamHost = steamId
                    };
                    Find.WindowStack.Add(new SteamConnectingWindow(steamId) { returnToServerBrowser = false });
                    Thread.Sleep(1500);
                    Multiplayer.Client.Multiplayer.session.ReapplyPrefs();
                    Multiplayer.Client.Multiplayer.Client.State = ConnectionStateEnum.ClientSteam;
                }
                
                catch (Exception ex)
                {
                    Log.Message("Error in joining 2: " + ex.ToString());
                }
            }

        }
    }
}


