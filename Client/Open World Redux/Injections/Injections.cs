using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using HugsLib;
using HarmonyLib;
using UnityEngine;
using System.IO;
using Verse.Profile;
using OpenWorldRedux.RTSE;

namespace OpenWorldRedux
{
    public class Injections : ModBase
    {
        public override string ModIdentifier => "OpenWorld";

        public static List<Action> thingsToDoInUpdate = new List<Action>();
    }

    //Inject Orders To ModBase
    [HarmonyPatch(typeof(ModBase), "OnGUI")]
    public static class InjectOrdersToRoot
    {
        [HarmonyPostfix]
        public static void InjectToRoot()
        {
            if (Injections.thingsToDoInUpdate.Count > 0)
            {
                Action[] toDo = Injections.thingsToDoInUpdate.ToArray();
                foreach (Action action in toDo)
                {
                    action.Invoke();
                    Injections.thingsToDoInUpdate.Remove(action);
                }
            }
        }
    }
    
    //Inject when saving
    [HarmonyPatch(typeof(GameDataSaveLoader), "SaveGame", typeof(string))]
    public static class SaveOnlineGame
    {
        [HarmonyPostfix]
        public static void HarmonyPost(ref string fileName)
        {
            if (!BooleanCache.isConnectedToServer) return;
            else
            {
                SaveHandler.SendSaveToServer(fileName);
                return;
            }
        }
    }

    //Get tile id when settling
    [HarmonyPatch(typeof(SettleInEmptyTileUtility), "Settle")]
    public static class GetTileIDWhenSettling
    {
        [HarmonyPostfix]
        public static void ModifyPost(Caravan caravan)
        {
            if (!BooleanCache.isConnectedToServer) return;
            else MPGame.SendPlayerSettlementDataFromCaravan(caravan);
        }
    }

    //Get tile id when abandoning settlement
    [HarmonyPatch(typeof(SettlementAbandonUtility), "Abandon")]
    public static class GetTileIDWhenAbandoning
    {
        [HarmonyPostfix]
        public static void ModifyPost(Settlement settlement)
        {
            if (!BooleanCache.isConnectedToServer) return;
            else MPGame.SendPlayerSettlementAbandonData(settlement);
        }
    }

    //Get tile id when settling first time
    [HarmonyPatch(typeof(Game), "InitNewGame")]
    public static class GetTileIDAtStart
    {
        [HarmonyPostfix]
        public static void ModifyPost(Game __instance)
        {
            if (!BooleanCache.isConnectedToServer) return;
            else
            {
                Log.Message("New game started");
                FactionsCache.FindOnlineFactionsInWorld();

                WorldHandler.PrepareWorld();

               WorldHandler.TryPatchOldWorlds();

                RimworldHandler.ToggleDevOptions();

                RimworldHandler.EnforceDificultyTweaks();

                MPGame.SendPlayerSettlementData(__instance);

                BooleanCache.hasLoadedCorrectly = true;
            }
        }
    }

    //Get Tile ID When Loading Existing Game
    [HarmonyPatch(typeof(Game), "LoadGame")]
    public static class GetTileIDWhenLoadingGame
    {
        [HarmonyPostfix]
        public static void GetIDFromExistingGame()
        {
            Log.Message("Might call loadgame");
            if (!BooleanCache.isConnectedToServer && !BooleanCache.ishostingrtseserver && !Comingback.iscomingbackfromsettlement) return;
            else
            {
                Log.Message("Called loadgame");
                FactionsCache.FindOnlineFactionsInWorld();

                WorldHandler.PrepareWorld();

                WorldHandler.TryPatchOldWorlds();

                RimworldHandler.ToggleDevOptions();

                RimworldHandler.EnforceDificultyTweaks();

                


                if (WorldRendererUtility.WorldRenderedNow == true)
                {
                    Find.WindowStack.Add(new Page_CustomSelectScenario());

                    string[] chainInfo = new string[]
                    {
                    "Welcome to the multiplayer world generation screen",
                    "Configure the settings you will use for this save",
                    "Locked variables are automatically handled by the server"
                    };
                    Find.WindowStack.Add(new OW_ChainInfoDialog(chainInfo));
                    BooleanCache.isGeneratingWorldFromPacket = false;


                }

                BooleanCache.hasLoadedCorrectly = true;

                if (Current.ProgramState == ProgramState.Playing && ColonistBar_CheckRecacheEntries.savedlastcaravan != null && ColonistBar_CheckRecacheEntries.savedcaravan != null && Comingback.iscomingbackfromsettlement && Multiplayer.Client.Multiplayer.session == null)
                {
                    Log.Message("CALLLLED COMING BACK RFOM PING");
                    Comingback.Comingbackfromsettlement();
                }
                
            }
        }
    }

    /*//Spawn All Online Materials Before Starting Site
	[HarmonyPatch(typeof(Page_SelectStartingSite), "PreOpen")]
	public static class SpawnOnlineMaterials
    {
		[HarmonyPostfix]
		public static void SpawnMaterials()
        {
			if (!BooleanCache.isConnectedToServer) return;
			else
            {
                FactionsCache.FindOnlineFactionsInWorld();

                WorldHandler.PrepareWorld();

                RimworldHandler.ToggleDevOptions();

                RimworldHandler.EnforceDificultyTweaks();

                BooleanCache.hasLoadedCorrectly = true;
            }
		}
    }*/

    //Get items traded
    [HarmonyPatch(typeof(Tradeable), "ResolveTrade")]
    public static class GetItemsTraded
    {
        [HarmonyPrefix]
        public static bool ModifyPre(List<Thing> ___thingsColony, int ___countToTransfer)
        {
            if (!BooleanCache.isConnectedToServer) return true;
            else
            {
                if (!FactionsCache.allOnlineFactions.Contains(TradeSession.trader.Faction)) return true;
                else
                {
                    TradeItem newTradeItem = new TradeItem();
                    newTradeItem.defName = ___thingsColony[0].def.defName;

                    PawnKindDef animalToFind = DefDatabase<PawnKindDef>.AllDefs.ToList().Find(fetch => fetch.defName == ___thingsColony[0].def.defName);
                    if (animalToFind != null)
                    {
                        newTradeItem.isAnimal = true;
                        newTradeItem.stackCount = 1;
                        newTradeItem.itemQuality = 2;
                    }

                    else
                    {
                        if (___thingsColony[0].Stuff != null)
                        {
                            newTradeItem.madeOfDef = ___thingsColony[0].Stuff.defName;
                        }

                        newTradeItem.stackCount = ___countToTransfer;

                        QualityCategory qc = QualityCategory.Normal;
                        ___thingsColony[0].TryGetQuality(out qc);
                        newTradeItem.itemQuality = (int)qc;

                        if (___thingsColony[0].def == ThingDefOf.MinifiedThing || ___thingsColony[0].def == ThingDefOf.MinifiedTree)
                        {
                            newTradeItem.isMinified = true;
                        }
                    }

                    TradeCache.tradeItems.Add(newTradeItem);
                    return true;
                }
            }
        }
    }

    //Allow All Items To Be Traded In Online
    [HarmonyPatch(typeof(TradeDeal), "AddAllTradeables")]
    public static class AddAllTradeables
    {
        [HarmonyPrefix]
        public static bool ModifyPre(ref List<Tradeable> ___tradeables)
        {
            if (!FactionsCache.allOnlineFactions.Contains(TradeSession.trader.Faction)) return true;
            else
            {
                ___tradeables = new List<Tradeable>();
                ___tradeables.AddRange(TradeCache.listToShowInTradesMenu);
                return false;
            }
        }
    }

    //Prevent Goodwill Change Next To Other Player
    [HarmonyPatch(typeof(SettlementProximityGoodwillUtility), "AppendProximityGoodwillOffsets")]
    public static class PrevenGoodwillChangeOnSettle
    {
        [HarmonyPrefix]
        public static bool PreventGoodwillChange(ref int tile, ref List<Pair<Settlement, int>> outOffsets)
        {
            int maxDist = SettlementProximityGoodwillUtility.MaxDist;
            List<Settlement> settlements = Find.WorldObjects.Settlements;
            for (int i = 0; i < settlements.Count; i++)
            {
                Settlement settlement = settlements[i];

                if (FactionsCache.allOnlineFactions.Contains(settlement.Faction)) continue;

                int num = Find.WorldGrid.TraversalDistanceBetween(tile, settlement.Tile, passImpassable: false, maxDist);
                if (num != int.MaxValue)
                {
                    int num2 = Mathf.RoundToInt(DiplomacyTuning.Goodwill_PerQuadrumFromSettlementProximity.Evaluate(num));
                    if (num2 != 0)
                    {
                        outOffsets.Add(new Pair<Settlement, int>(settlement, num2));
                    }
                }
            }

            return false;
        }
    }

    //Modify autosave
    [HarmonyPatch(typeof(Autosaver), "AutosaverTick")]
    public static class Autosave
    {
        [HarmonyPrefix]
        public static bool ModifyPre(Autosaver __instance)
        {
            if (!BooleanCache.isConnectedToServer) return true;
            else
            {
                FocusCache.actualSaveTicks++;
                if (FocusCache.actualSaveTicks >= FocusCache.autosaveInternalTicks && !GameDataSaveLoader.SavingIsTemporarilyDisabled)
                {
                    LongEventHandler.QueueLongEvent(__instance.DoAutosave, "Autosaving", doAsynchronously: false, null);
                    FocusCache.actualSaveTicks = 0;
                }

                return false;
            }
        }
    }
}