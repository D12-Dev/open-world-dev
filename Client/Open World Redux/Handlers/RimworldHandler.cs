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
    public static class RimworldHandler
    {
		public static bool CheckIfAnySocialPawn(int searchLocation)
        {
			if (searchLocation == 0)
            {
				Caravan caravan = FocusCache.focusedCaravan;
				Pawn playerNegotiator = caravan.PawnsListForReading.Find(fetch => fetch.IsColonist && !fetch.skills.skills[10].PermanentlyDisabled);
				if (playerNegotiator == null) return false;
				else return true;
			}

			else if (searchLocation == 1)
            {
				Map map = Find.AnyPlayerHomeMap;
				Pawn playerNegotiator = map.mapPawns.AllPawns.Find(fetch => fetch.IsColonist && !fetch.skills.skills[10].PermanentlyDisabled);
				if (playerNegotiator == null) return false;
				else return true;
			}

			return false;
        }

        public static void EnforceDificultyTweaks()
        {
            Current.Game.Info.permadeathMode = true;

            if (DifficultyCache.usingCustomDifficulty)
            {
                Current.Game.storyteller.difficulty.threatScale = DifficultyCache.threatScale;
                Current.Game.storyteller.difficulty.allowBigThreats = DifficultyCache.allowBigThreats;
                Current.Game.storyteller.difficulty.allowViolentQuests = DifficultyCache.allowViolentQuests;
                Current.Game.storyteller.difficulty.allowIntroThreats = DifficultyCache.allowIntroThreats;
                Current.Game.storyteller.difficulty.predatorsHuntHumanlikes = DifficultyCache.predatorsHuntHumanlikes;
                Current.Game.storyteller.difficulty.allowExtremeWeatherIncidents = DifficultyCache.allowExtremeWeatherIncidents;

                Current.Game.storyteller.difficulty.cropYieldFactor = DifficultyCache.cropYieldFactor;
                Current.Game.storyteller.difficulty.mineYieldFactor = DifficultyCache.mineYieldFactor;
                Current.Game.storyteller.difficulty.butcherYieldFactor = DifficultyCache.butcherYieldFactor;
                Current.Game.storyteller.difficulty.researchSpeedFactor = DifficultyCache.researchSpeedFactor;
                Current.Game.storyteller.difficulty.questRewardValueFactor = DifficultyCache.questRewardValueFactor;
                Current.Game.storyteller.difficulty.raidLootPointsFactor = DifficultyCache.raidLootPointsFactor;
                Current.Game.storyteller.difficulty.tradePriceFactorLoss = DifficultyCache.tradePriceFactorLoss;
                Current.Game.storyteller.difficulty.maintenanceCostFactor = DifficultyCache.maintenanceCostFactor;
                Current.Game.storyteller.difficulty.scariaRotChance = DifficultyCache.scariaRotChance;
                Current.Game.storyteller.difficulty.enemyDeathOnDownedChanceFactor = DifficultyCache.enemyDeathOnDownedChanceFactor;

                Current.Game.storyteller.difficulty.colonistMoodOffset = DifficultyCache.colonistMoodOffset;
                Current.Game.storyteller.difficulty.foodPoisonChanceFactor = DifficultyCache.foodPoisonChanceFactor;
                Current.Game.storyteller.difficulty.manhunterChanceOnDamageFactor = DifficultyCache.manhunterChanceOnDamageFactor;
                Current.Game.storyteller.difficulty.playerPawnInfectionChanceFactor = DifficultyCache.playerPawnInfectionChanceFactor;
                Current.Game.storyteller.difficulty.diseaseIntervalFactor = DifficultyCache.diseaseIntervalFactor;
                Current.Game.storyteller.difficulty.deepDrillInfestationChanceFactor = DifficultyCache.deepDrillInfestationChanceFactor;
                Current.Game.storyteller.difficulty.friendlyFireChanceFactor = DifficultyCache.friendlyFireChanceFactor;
                Current.Game.storyteller.difficulty.allowInstantKillChance = DifficultyCache.allowInstantKillChance;

                Current.Game.storyteller.difficulty.allowTraps = DifficultyCache.allowTraps;
                Current.Game.storyteller.difficulty.allowTurrets = DifficultyCache.allowTurrets;
                Current.Game.storyteller.difficulty.allowMortars = DifficultyCache.allowMortars;

                Current.Game.storyteller.difficulty.adaptationEffectFactor = DifficultyCache.adaptationEffectFactor;
                Current.Game.storyteller.difficulty.adaptationGrowthRateFactorOverZero = DifficultyCache.adaptationGrowthRateFactorOverZero;
                Current.Game.storyteller.difficulty.fixedWealthMode = DifficultyCache.fixedWealthMode;
            }
        }

        public static void ToggleDevOptions()
        {
            Prefs.MaxNumberOfPlayerSettlements = 1;

            if (BooleanCache.isAdmin) return;
            else Prefs.DevMode = false;
        }

        public static Thing GetThingFromFile(TradeItem item)
        {
            Thing thingToGet = null;
            ThingDef thingDef = null;
            ThingDef madeOfDef = null;

            foreach(ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.defName == item.defName)
                {
                    thingDef = def;
                    break;
                }
            }

            if (thingDef == null) return null;

            if (item.isAnimal)
            {
                PawnKindDef toSpawn = DefDatabase<PawnKindDef>.AllDefs.ToList().Find(fetch => fetch.defName == item.defName);
                Pawn animal = PawnGenerator.GeneratePawn(new PawnGenerationRequest(toSpawn, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null));
                animal.SetFaction(Faction.OfPlayer);
                return animal;
            }

            else
            {
                foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
                {
                    if (def.defName == item.madeOfDef)
                    {
                        madeOfDef = def;
                        break;
                    }
                }

                thingToGet = ThingMaker.MakeThing(thingDef, madeOfDef);
                thingToGet.stackCount = item.stackCount;

                CompQuality compQuality = thingToGet.TryGetComp<CompQuality>();
                if (compQuality != null)
                {
                    QualityCategory q = (QualityCategory)item.itemQuality;
                    compQuality.SetQuality(q, ArtGenerationContext.Colony);
                }

                if (item.isMinified) thingToGet.TryMakeMinified();
            }

            return thingToGet;
        }

        public static IntVec3 GetTradeLocationInMap(Map map)
        {
            IntVec3 positionToDrop = new IntVec3(map.Center.x, map.Center.y, map.Center.z);

            foreach(Thing thing in map.listerThings.AllThings)
            {
                if (thing.def.defName == "TradingSpot_OpenWorld")
                {
                    positionToDrop = thing.Position;
                    break;
                }
            }

            return positionToDrop;
        }

        public static bool CheckIfEnoughSilverInSettlement(int quantityNeeded)
        {
            Map map = Find.AnyPlayerHomeMap;
            int quantityAvailable = 0;

            foreach (Zone zone in map.zoneManager.AllZones)
            {
                foreach (Thing item in zone.AllContainedThings)
                {
                    if (item.def == ThingDefOf.Silver)
                    {
                        quantityAvailable += item.stackCount;
                    }
                }
            }

            if (quantityAvailable < quantityNeeded) return false;
            else return true;
        }

        public static void TakeSilverFromSettlement(int silverToTake)
        {
            Map map = Find.AnyPlayerHomeMap;

            foreach (Zone zone in map.zoneManager.AllZones)
            {
                foreach (Thing item in zone.AllContainedThings)
                {
                    if (item.def == ThingDefOf.Silver)
                    {
                        if (silverToTake == 0) break;

                        if (silverToTake - item.stackCount < 0)
                        {
                            int quantityDif = silverToTake - item.stackCount;
                            item.stackCount = -quantityDif;
                            break;
                        }
                        else
                        {
                            silverToTake -= item.stackCount;
                            item.Destroy();
                        }
                    }
                }
            }
        }

        public static void GetSilverToCaravan(int quantity)
        {
            Caravan caravan = FocusCache.focusedCaravan;

            TradeItem silverToReturn = new TradeItem();
            silverToReturn.defName = "Silver";
            silverToReturn.stackCount = quantity;
            silverToReturn.itemQuality = 0;
            Thing silverToGive = GetThingFromFile(silverToReturn);

            caravan.AddPawnOrItem(silverToGive, false);

            Injections.thingsToDoInUpdate.Add(MPGame.ForceSave);
        }

        public static void TakeSilverFromCaravan(int quantity)
        {
            List<Thing> caravanSilver = CaravanInventoryUtility.AllInventoryItems(FocusCache.focusedCaravan)
                .Where((Thing x) => x.def == ThingDefOf.Silver).ToList();

            if (quantity == 0) return;

            int silverInCaravan = 0;
            foreach (Thing silverStack in caravanSilver) silverInCaravan += silverStack.stackCount;

            if (silverInCaravan < quantity) return;
            else
            {
                int takenSilver = 0;
                foreach (Thing silverStack in caravanSilver)
                {
                    if (takenSilver + silverStack.stackCount >= quantity)
                    {
                        silverStack.holdingOwner.Take(silverStack, quantity - takenSilver);
                        break;
                    }

                    else if (takenSilver + silverStack.stackCount < quantity)
                    {
                        silverStack.holdingOwner.Take(silverStack, silverStack.stackCount);
                        takenSilver += silverStack.stackCount;
                    }
                }

                Injections.thingsToDoInUpdate.Add(MPGame.ForceSave);
            }
        }

        public static void GetItemsToCaravan(TradeItem[] items, bool forceSave = true)
        {
            Caravan caravan = FocusCache.focusedCaravan;

            foreach(TradeItem item in items)
            {
                Thing toGet = GetThingFromFile(item);
                if (item.isAnimal) Find.WorldPawns.PassToWorld(toGet as Pawn);
                caravan.AddPawnOrItem(toGet, true);
            }

            if (forceSave) Injections.thingsToDoInUpdate.Add(MPGame.ForceSave);
        }

        public static void GetItemsToSettlement(TradeItem[] items, bool forceSave = true)
        {
            Map map = Find.AnyPlayerHomeMap;
            IntVec3 location = GetTradeLocationInMap(map);

            foreach (TradeItem item in items)
            {
                Thing toGet = GetThingFromFile(item);

                if (toGet == null) continue;
                else if (item.isAnimal) SpawnAnimal(toGet, location, map);
                else GenPlace.TryPlaceThing(toGet, location, map, ThingPlaceMode.Near);
            }

            if (forceSave) Injections.thingsToDoInUpdate.Add(MPGame.ForceSave);
        }

        public static void SpawnAnimal(Thing toGet, IntVec3 location, Map map)
        {
            PawnKindDef toSpawn = DefDatabase<PawnKindDef>.AllDefs.ToList().Find(fetch => fetch.defName == toGet.def.defName);
            if (toSpawn != null)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(location, map, 12);
                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(toSpawn, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null));
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
                pawn.SetFaction(Faction.OfPlayer);
            }
        }
    }
}
