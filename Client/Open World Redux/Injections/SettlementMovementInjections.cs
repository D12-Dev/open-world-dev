﻿using HarmonyLib;
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
    class SettlementMovementInjections
    {
		//Get Tile ID Of New Settlement
		[HarmonyPatch(typeof(SettleInEmptyTileUtility), "Settle")]
		public static class GetTileIDOfNewSettlement
		{
			[HarmonyPostfix]
			public static void GetIDFromNewSettlement(ref Caravan caravan)
			{
				//if (BooleanCache.isConnectedToServer)
				//{
				//	//Need to implement wealth and pawn values in here
				//	if (Find.AnyPlayerHomeMap == null) Network.SendData("UserSettlement│NewSettlementID│" + caravan.Tile);
				//	else return;
				//}
				//else return;
			}
		}

		//Get Tile ID Of Abandoned Settlement
		[HarmonyPatch(typeof(SettlementAbandonUtility), "Abandon")]
		public static class GetTileIDOfAbandonedSettlement
		{
			[HarmonyPostfix]
			public static void GetIDFromAbandonedSettlement(ref Settlement settlement)
			{
				//if (BooleanCache.isConnectedToServer)
				//{
				//	if (Find.AnyPlayerHomeMap == null) Network.SendData("UserSettlement│AbandonSettlementID│" + settlement.Tile);
				//	else return;
				//}
				//else return;
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

					if (settlement.Faction == FactionsCache.onlineNeutralFaction||
						settlement.Faction == FactionsCache.onlineAllyFaction ||
						settlement.Faction == FactionsCache.onlineEnemyFaction)
					{
						continue;
					}

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
	}
}
