using HarmonyLib;
using Ionic.Zlib;
using Multiplayer.Client.Networking;
using Multiplayer.Common;
using RimWorld;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Multiplayer.Client.Comp;
using UnityEngine;
using Verse;

namespace Multiplayer.Client
{

    public static class HostUtil
    {
        public static void HostServer(ServerSettings settings, bool fromReplay, bool hadSimulation, bool asyncTime)
        {
            Log.Message($"Starting the server");

            var session = new MultiplayerSession();
            if (Multiplayer.session != null) // This is the case when hosting from a replay
                session.dataSnapshot = Multiplayer.session.dataSnapshot;

            Multiplayer.session = session;

            session.myFactionId = Faction.OfPlayer.loadID;
            session.localServerSettings = settings;
            session.gameName = settings.gameName;
            // Server already pre-inited in HostWindow
            var localServer = Multiplayer.LocalServer;
            MultiplayerServer.instance = Multiplayer.LocalServer;

            if (hadSimulation)
            {
                localServer.savedGame = GZipStream.CompressBuffer(session.dataSnapshot.gameData);
                localServer.semiPersistent = GZipStream.CompressBuffer(session.dataSnapshot.semiPersistentData);
                localServer.mapData = session.dataSnapshot.mapData.ToDictionary(kv => kv.Key, kv => GZipStream.CompressBuffer(kv.Value));
                localServer.mapCmds = session.dataSnapshot.mapCmds.ToDictionary(kv => kv.Key, kv => kv.Value.Select(ScheduledCommand.Serialize).ToList());
            }

            localServer.commands.debugOnlySyncCmds = Sync.handlers.Where(h => h.debugOnly).Select(h => h.syncId).ToHashSet();
            localServer.commands.hostOnlySyncCmds = Sync.handlers.Where(h => h.hostOnly).Select(h => h.syncId).ToHashSet();
            localServer.hostUsername = Multiplayer.username;
            localServer.defaultFactionId = Faction.OfPlayer.loadID;

            localServer.rwVersion = VersionControl.CurrentVersionString;
            localServer.mpVersion = MpVersion.Version;
            localServer.defInfos = MultiplayerData.localDefInfos;
            localServer.serverData = JoinData.WriteServerData(settings.syncConfigs);

            if (settings.steam)
                localServer.TickEvent += SteamIntegration.ServerSteamNetTick;

            if (fromReplay)
                localServer.gameTimer = TickPatch.Timer;

            if (!fromReplay)
                SetupGameFromSingleplayer();

            foreach (var tickable in TickPatch.AllTickables)
                tickable.Cmds.Clear();

            Find.PlaySettings.usePlanetDayNightSystem = false;

            Multiplayer.game.ChangeRealPlayerFaction(Faction.OfPlayer);
            SetupLocalClient();

            Find.MainTabsRoot.EscapeCurrentTab(false);

            Multiplayer.session.AddMsg("If you are having any issues with the mod and would like some help resolving them, then please reach out to us on our Discord server:", false);
            Multiplayer.session.AddMsg(new ChatMsg_Url("https://discord.gg/S4bxXpv"), false);

            if (hadSimulation)
            {
                StartServerThread();
            }
            else
            {
                Multiplayer.WorldComp.TimeSpeed = TimeSpeed.Paused;
                foreach (var map in Find.Maps)
                    map.AsyncTime().TimeSpeed = TimeSpeed.Paused;

                Multiplayer.WorldComp.UpdateTimeSpeed();

                Multiplayer.GameComp.asyncTime = asyncTime;
                Multiplayer.GameComp.debugMode = settings.debugMode;
                Multiplayer.GameComp.logDesyncTraces = settings.desyncTraces;
                Multiplayer.GameComp.pauseOnLetter = settings.pauseOnLetter;
                Multiplayer.GameComp.timeControl = settings.timeControl;

                LongEventHandler.QueueLongEvent(() =>
                {
                    Multiplayer.session.dataSnapshot = SaveLoad.CreateGameDataSnapshot(SaveLoad.SaveAndReload());
                    SaveLoad.SendGameData(Multiplayer.session.dataSnapshot, false);

                    StartServerThread();
                }, "MpSaving", false, null);
            }

            void StartServerThread()
            {
                localServer.running = true;

                Multiplayer.LocalServer.serverThread = new Thread(localServer.Run)
                {
                    Name = "Local server thread"
                };
                Multiplayer.LocalServer.serverThread.Start();

                const string text = "Server started.";
                Messages.Message(text, MessageTypeDefOf.SilentInput, false);
                Log.Message(text);
            }
        }

        private static void SetupGameFromSingleplayer()
        {
            MultiplayerWorldComp comp = new MultiplayerWorldComp(Find.World);

            Faction NewFaction(int id, string name, FactionDef def)
            {
                Faction faction = Find.FactionManager.AllFactions.FirstOrDefault(f => f.loadID == id);

                if (faction == null)
                {
                    faction = new Faction() { loadID = id, def = def };

                    faction.ideos = new FactionIdeosTracker(faction);
                    faction.ideos.ChooseOrGenerateIdeo(new IdeoGenerationParms());

                    foreach (Faction other in Find.FactionManager.AllFactionsListForReading)
                        faction.TryMakeInitialRelationsWith(other);

                    Find.FactionManager.Add(faction);

                    comp.factionData[faction.loadID] = FactionWorldData.New(faction.loadID);
                }

                faction.Name = name;
                faction.def = def;

                return faction;
            }

            Faction.OfPlayer.Name = $"{Multiplayer.username}'s faction";
            //comp.factionData[Faction.OfPlayer.loadID] = FactionWorldData.FromCurrent();

            Multiplayer.game = new MultiplayerGame
            {
                gameComp = new MultiplayerGameComp(Current.Game)
                {
                    globalIdBlock = new IdBlock(GetMaxUniqueId(), 1_000_000_000)
                },
                worldComp = comp
            };

            var opponent = NewFaction(Multiplayer.GlobalIdBlock.NextId(), "Opponent", FactionDefOf.PlayerColony);
            opponent.hidden = true;
            opponent.SetRelation(new FactionRelation(Faction.OfPlayer, FactionRelationKind.Hostile));

            foreach (FactionWorldData data in comp.factionData.Values)
            {
                foreach (DrugPolicy p in data.drugPolicyDatabase.policies)
                    p.uniqueId = Multiplayer.GlobalIdBlock.NextId();

                foreach (Outfit o in data.outfitDatabase.outfits)
                    o.uniqueId = Multiplayer.GlobalIdBlock.NextId();

                foreach (FoodRestriction o in data.foodRestrictionDatabase.foodRestrictions)
                    o.id = Multiplayer.GlobalIdBlock.NextId();
            }

            foreach (Map map in Find.Maps)
            {
                //mapComp.mapIdBlock = localServer.NextIdBlock();

                BeforeMapGeneration.SetupMap(map);
                BeforeMapGeneration.InitNewMapFactionData(map, opponent);

                AsyncTimeComp async = map.AsyncTime();
                async.mapTicks = Find.TickManager.TicksGame;
                async.TimeSpeed = Find.TickManager.CurTimeSpeed;
            }

            Multiplayer.WorldComp.UpdateTimeSpeed();
        }

        private static void SetupLocalClient()
        {
            if (Multiplayer.session.localServerSettings.arbiter)
                StartArbiter();

            LocalClientConnection localClient = new LocalClientConnection(Multiplayer.username);
            LocalServerConnection localServerConn = new LocalServerConnection(Multiplayer.username);

            localServerConn.clientSide = localClient;
            localClient.serverSide = localServerConn;

            localClient.State = ConnectionStateEnum.ClientPlaying;
            localServerConn.State = ConnectionStateEnum.ServerPlaying;

            var serverPlayer = Multiplayer.LocalServer.playerManager.OnConnected(localServerConn);
            serverPlayer.color = PlayerManager.PlayerColors[0];
            PlayerManager.givenColors[serverPlayer.Username] = serverPlayer.color;
            serverPlayer.status = PlayerStatus.Playing;
            serverPlayer.FactionId = Faction.OfPlayer.loadID;
            serverPlayer.SendPlayerList();
            Multiplayer.LocalServer.playerManager.SendInitDataCommand(serverPlayer);

            Multiplayer.session.client = localClient;
            Multiplayer.session.ReapplyPrefs();
        }

        private static void StartArbiter()
        {
            Multiplayer.session.AddMsg("The Arbiter instance is starting...", false);

            Multiplayer.LocalServer.liteNet.SetupArbiterConnection();

            string args = $"-batchmode -nographics -arbiter -logfile arbiter_log.txt -connect=127.0.0.1:{Multiplayer.LocalServer.liteNet.ArbiterPort}";

            if (GenCommandLine.TryGetCommandLineArg("savedatafolder", out string saveDataFolder))
                args += $" \"-savedatafolder={saveDataFolder}\"";

            string arbiterInstancePath;
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                arbiterInstancePath = Application.dataPath + "/MacOS/" + Process.GetCurrentProcess().MainModule.ModuleName;
            }
            else
            {
                arbiterInstancePath = Process.GetCurrentProcess().MainModule.FileName;
            }

            try
            {
                Multiplayer.session.arbiter = Process.Start(
                    arbiterInstancePath,
                    args
                );
            }
            catch (Exception ex)
            {
                Multiplayer.session.AddMsg("Arbiter failed to start.", false);
                Log.Error("Arbiter failed to start.");
                Log.Error(ex.ToString());
                if (ex.InnerException is Win32Exception)
                {
                    Log.Error("Win32 Error Code: " + ((Win32Exception)ex).NativeErrorCode.ToString());
                }
            }
        }

        public static int GetMaxUniqueId()
        {
            return typeof(UniqueIDsManager)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.FieldType == typeof(int))
                .Select(f => (int)f.GetValue(Find.UniqueIDsManager))
                .Max();
        }

        public static void SetAllUniqueIds(int value)
        {
            typeof(UniqueIDsManager)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.FieldType == typeof(int))
                .Do(f => f.SetValue(Find.UniqueIDsManager, value));
        }
    }
}
