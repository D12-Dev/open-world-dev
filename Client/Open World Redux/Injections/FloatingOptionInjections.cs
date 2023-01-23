using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    class FloatingOptionInjections
    {
		//Get All World Map Gizmos For Online Settlements
		[HarmonyPatch(typeof(Settlement), "GetFloatMenuOptions")]
		public static class SetWorldMapGizmos
		{
			[HarmonyPostfix]
			public static void ModifyPost(ref IEnumerable<FloatMenuOption> __result, Caravan caravan, Settlement __instance)
			{
                if (__instance.Faction == FactionsCache.onlineNeutralFaction ||
                    __instance.Faction == FactionsCache.onlineAllyFaction ||
                    __instance.Faction == FactionsCache.onlineEnemyFaction)
                {
                    var gizmoList = __result.ToList();
                    gizmoList.Clear();

                    if (CaravanVisitUtility.SettlementVisitedNow(caravan) != __instance)
                    {
                        foreach (FloatMenuOption floatMenuOption2 in CaravanArrivalAction_VisitSettlement.GetFloatMenuOptions(caravan, __instance))
                        {
                            gizmoList.Add(floatMenuOption2);
                        }
                    }

                    __result = gizmoList;
                }
			}
		}

		//Get All World Map Gizmos For Online Settlements
		[HarmonyPatch(typeof(Site), "GetFloatMenuOptions")]
		public static class SetSiteFloatingOptions
		{
			[HarmonyPostfix]
			public static void ModifyPost(Site __instance, ref IEnumerable<FloatMenuOption> __result)
			{
                if (__instance.Faction == FactionsCache.onlineNeutralFaction ||
                    __instance.Faction == FactionsCache.onlineAllyFaction ||
                    __instance.Faction == FactionsCache.onlineEnemyFaction)
                {
                    var gizmoList = __result.ToList();
                    gizmoList.Clear();

                    __result = gizmoList;
                    return;
                }
			}
		}

		//Get Item Transfer From Drop Pod
		[HarmonyPatch(typeof(TransportPodsArrivalAction_GiveGift), "GetFloatMenuOptions")]
		public static class ChangeMenuOptionsOnPod
		{
			[HarmonyPostfix]
			public static void ModifyPost(ref IEnumerable<FloatMenuOption> __result, Settlement settlement, CompLaunchable representative)
			{
                if (settlement.Faction == FactionsCache.onlineNeutralFaction ||
                    settlement.Faction == FactionsCache.onlineAllyFaction ||
                    settlement.Faction == FactionsCache.onlineEnemyFaction)
                {
                    var floatMenuList = __result.ToList();
                    floatMenuList.Clear();

                    if (!BooleanCache.isConnectedToServer)
                    {
                        __result = floatMenuList;
                        return;
                    }

                    FocusCache.focusedSettlement = settlement;

                    Action action = delegate
                    {
                        TradeHandler.GetItemsFromPods(representative);

                        TradeHandler.SendTrades();

                        representative.TryLaunch(settlement.Tile, new TransportPodsArrivalAction_GiveGift(settlement));
                    };

                    FloatMenuOption option = new FloatMenuOption(label: "Send a gift to " + settlement.Name, action: action);
                    floatMenuList.Add(option);
                    __result = floatMenuList;
                }
			}
		}

		//Forbid Attacking AI Via Drop Pod
		[HarmonyPatch(typeof(TransportPodsArrivalAction_AttackSettlement), "GetFloatMenuOptions")]
		public static class ForbidAttackAI
		{
			[HarmonyPostfix]
			public static void ModifyPost(ref IEnumerable<FloatMenuOption> __result, Settlement settlement)
			{
                if (settlement.Faction == FactionsCache.onlineNeutralFaction ||
                    settlement.Faction == FactionsCache.onlineAllyFaction ||
                    settlement.Faction == FactionsCache.onlineEnemyFaction)
                {
                    var floatMenuList = __result.ToList();
                    floatMenuList.Clear();

                    __result = floatMenuList;
                }
			}
		}
	}
}
