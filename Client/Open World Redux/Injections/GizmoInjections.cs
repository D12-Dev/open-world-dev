using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OpenWorldRedux
{
    class GizmoInjections
    {
		//Get Online Settlement Gizmos
		[HarmonyPatch(typeof(Settlement), "GetGizmos")]
		public static class SetSettlementGizmosForOnline
		{
			[HarmonyPostfix]
			public static void ModifyPost(ref IEnumerable<Gizmo> __result, Settlement __instance)
			{
                if (FactionsCache.allOnlineFactions.Contains(__instance.Faction))
                {
                    var gizmoList = __result.ToList();
                    gizmoList.Clear();
                    __result = gizmoList;
                }

				else if (__instance.Faction == Find.FactionManager.OfPlayer)
				{
					var gizmoList = __result.ToList();

					Command_Action command_TradingPost = new Command_Action
					{
						defaultLabel = "Trading Post",
						defaultDesc = "See what the world has to offer",
						icon = ContentFinder<Texture2D>.Get("UI/Commands/MergeCaravans"),
						action = delegate
						{
							Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
						}
					};

					Command_Action command_FactionMenu = new Command_Action
					{
						defaultLabel = "Faction Menu",
						defaultDesc = "Open Your Faction Menu",
						icon = ContentFinder<Texture2D>.Get("UI/Icons/VisitorsHelp"),
						action = delegate
						{
							Find.WindowStack.Add(new OW_MPFactionMenu());
						}
					};

					gizmoList.Add(command_TradingPost);
					gizmoList.Add(command_FactionMenu);
					__result = gizmoList;
				}

				else return;
			}
		}

		//Get All Caravan Gizmos For Online Settlements
		[HarmonyPatch(typeof(Settlement), "GetCaravanGizmos")]
		public static class SetCaravanGizmosForOnlineSettlement
		{
			[HarmonyPostfix]
			public static void ModifyPost(ref IEnumerable<Gizmo> __result, Settlement __instance, Caravan caravan)
			{
                if (!FactionsCache.allOnlineFactions.Contains(__instance.Faction)) return;
				else
				{
					var gizmoList = __result.ToList();
					List<Gizmo> removeList = new List<Gizmo>();

					foreach (Command_Action action in gizmoList)
					{
						if (action.defaultLabel == "CommandTrade".Translate()) removeList.Add(action);
						else if (action.defaultLabel == "CommandAttackSettlement".Translate()) removeList.Add(action);
						else if (action.defaultLabel == "CommandOfferGifts".Translate()) removeList.Add(action);
					}
					foreach (Gizmo g in removeList) gizmoList.Remove(g);

                    Command_Action command_Attack = new Command_Action
                    {
                        defaultLabel = "Attack",
                        defaultDesc = "Attack this settlement",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
                        }
                    };

                    Command_Action command_RealtimeAttack = new Command_Action
                    {
                        defaultLabel = "Realtime Attack",
                        defaultDesc = "Attack this settlement realtime",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
                        }
                    };

                    Command_Action command_RealtimeVisit = new Command_Action
                    {
                        defaultLabel = "Realtime Visit",
                        defaultDesc = "Visit this settlement realtime",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/Settle"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
                        }
                    };

                    Command_Action command_Faction = new Command_Action
                    {
                        defaultLabel = "Faction Menu",
                        defaultDesc = "Open the faction menu",
                        icon = ContentFinder<Texture2D>.Get("UI/Icons/VisitorsHelp"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            Find.WindowStack.Add(new OW_MPFactionOnPlayer());
                        }
                    };

                    Command_Action command_Spy = new Command_Action
                    {
                        defaultLabel = "Spy",
                        defaultDesc = "Pay in silver to spy on this settlement",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/ShowMap"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
                        }
                    };

                    Command_Action command_Trade = new Command_Action
                    {
                        defaultLabel = "Trade",
                        defaultDesc = "Trade with this settlement",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/Trade"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new OW_MPTradeMenu());
                            else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                        }
                    };

                    Command_Action command_Barter = new Command_Action
                    {
                        defaultLabel = "Barter",
                        defaultDesc = "Barter with this settlement",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/FulfillTradeRequest"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new OW_MPBarterMenu());
                            else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                        }
                    };

                    Command_Action command_Gift = new Command_Action
                    {
                        defaultLabel = "Gift",
                        defaultDesc = "Gift items to this settlement",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/OfferGifts"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new OW_MPGiftMenu());
                            else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                        }
                    };

                    Command_Action command_BlackMarket = new Command_Action
                    {
                        defaultLabel = "Black Market",
                        defaultDesc = "Pay silver to cause events in this settlement",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/CallAid"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            Find.WindowStack.Add(new OW_MPBlackMarket());
                        }
                    };

                    Command_Action command_Aid = new Command_Action
                    {
                        defaultLabel = "Aid",
                        defaultDesc = "Pay silver to send colonists to this settlement",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/AsMedical"),
                        action = delegate
                        {
                            FocusCache.focusedSettlement = __instance;
                            FocusCache.focusedCaravan = caravan;

                            Find.WindowStack.Add(new OW_MPAidMenu());
                        }
                    };

                    gizmoList.Add(command_Attack);
                    gizmoList.Add(command_RealtimeAttack);
                    gizmoList.Add(command_RealtimeVisit);
                    gizmoList.Add(command_Spy);
                    gizmoList.Add(command_BlackMarket);
                    gizmoList.Add(command_Aid);

                    if (Find.AnyPlayerHomeMap != null)
                    {
                        gizmoList.Add(command_Trade);
                        gizmoList.Add(command_Barter);
                        gizmoList.Add(command_Gift);
                    }

                    if (FactionCache.hasFaction == true) gizmoList.Add(command_Faction);

                    __result = gizmoList;
                }
			}
		}

		//Get All Faction Gizmos For Globe
		[HarmonyPatch(typeof(Caravan), "GetGizmos")]
		public static class SetGlobeGizmos
		{
			[HarmonyPostfix]
			public static void ModifyPost(ref IEnumerable<Gizmo> __result, Caravan __instance)
			{
                if (!FactionCache.hasFaction) return;
                else
                {
                    List<Site> worldSites = Find.World.worldObjects.Sites;

                    Settlement presentSettlement = Find.World.worldObjects.Settlements.Find(fetch => fetch.Tile == __instance.Tile);
                    if (presentSettlement != null) return;

                    WorldObject objectToFind = worldSites.Find(fetch => fetch.Tile == __instance.Tile);

                    List<Gizmo> gizmoList = __result.ToList();

                    if (objectToFind == null)
                    {
                        Settlement existingSettlement = WorldCache.onlineSettlements.Find(fetch => fetch.Tile == __instance.Tile);

                        if (existingSettlement != null) return;
                        else
                        {
                            Command_Action Command_BuildOnlineSite = new Command_Action
                            {
                                defaultLabel = "Build Faction Site",
                                defaultDesc = "Build an utility site for your faction",
                                icon = ContentFinder<Texture2D>.Get("UI/Commands/Install"),
                                action = delegate
                                {
                                    FocusCache.focusedCaravan = __instance;
                                    Find.WindowStack.Add(new OW_MPFactionSiteBuilding());
                                }
                            };

                            gizmoList.Add(Command_BuildOnlineSite);
                            __result = gizmoList;
                        }
                    }

                    else
                    {
                        if (!FactionsCache.allOnlineFactions.Contains(objectToFind.Faction)) return;
                        else
                        {
                            Command_Action Command_AttackSite = new Command_Action
                            {
                                defaultLabel = "Attack site",
                                defaultDesc = "Attack this site",
                                icon = ContentFinder<Texture2D>.Get("UI/Commands/AttackSettlement"),
                                action = delegate
                                {
                                    FocusCache.focusedCaravan = __instance;
                                    Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
                                }
                            };

                            Command_Action Command_AccessSite = new Command_Action
                            {
                                defaultLabel = "Access Site",
                                defaultDesc = "Access this site",
                                icon = ContentFinder<Texture2D>.Get("UI/Commands/Install"),
                                action = delegate
                                {
                                    int siteType = 0;
                                    foreach (StructureFile structure in WorldCache.onlineStructuresDeflate)
                                    {
                                        if (structure.structureTile == objectToFind.Tile)
                                        {
                                            siteType = structure.structureType;
                                            break;
                                        }
                                    }

                                    FocusCache.focusedCaravan = __instance;

                                    //Prohibit if wonder
                                    if (siteType == 3) Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
                                    else Find.WindowStack.Add(new OW_MPFactionSiteUse(siteType));
                                }
                            };

                            Command_Action Command_DemolishSite = new Command_Action
                            {
                                defaultLabel = "Demolish Site",
                                defaultDesc = "Demolish this site",
                                icon = ContentFinder<Texture2D>.Get("UI/Commands/AbandonHome"),
                                action = delegate
                                {
                                    FocusCache.focusedCaravan = __instance;
                                    Find.WindowStack.Add(new OW_MPFactionSiteDemolish());
                                }
                            };

                            gizmoList.Add(Command_AttackSite);
                            if (objectToFind.Faction == FactionsCache.onlineAllyFaction)
                            {
                                gizmoList.Add(Command_AccessSite);
                                gizmoList.Add(Command_DemolishSite);
                            }

                            __result = gizmoList;
                        }
                    }
                }
            }
		}

        //Get Road Gizmos For Globe
        [HarmonyPatch(typeof(Caravan), "GetGizmos")]
        public static class SetRoadGizmos
        {
            [HarmonyPostfix]
            public static void ModifyPost(ref IEnumerable<Gizmo> __result, Caravan __instance)
            {
                if (!BooleanCache.isConnectedToServer) return;
                else
                {
                    //List<Tile> worldTiles = Find.World.grid.tiles;
                    //Tile toGet = worldTiles.Find(fetch => fetch.feature.uniqueID == __instance.Tile);
                    
                    //if (toGet.Roads != null) return;
                    //else
                    //{
                    //    Command_Action Command_BuildRoad = new Command_Action
                    //    {
                    //        defaultLabel = "Road Building Menu",
                    //        defaultDesc = "Build a road in this tile",
                    //        icon = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings"),
                    //        action = delegate
                    //        {
                    //            FocusCache.focusedCaravan = __instance;

                    //            Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));

                    //            //Find.WindowStack.Add(new OW_MPRoadMenu());
                    //        }
                    //    };

                    //    List<Gizmo> gizmoList = __result.ToList();
                    //    gizmoList.Add(Command_BuildRoad);
                    //    __result = gizmoList;
                    //}
                }
            }
        }
    }
}
