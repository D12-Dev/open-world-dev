using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class BlackMarketHandler
    {
        private static IncidentDef incidentDef = null;
        private static IncidentParms parms = null;

        public static void GetAcceptedEvent()
        {
            RimworldHandler.TakeSilverFromCaravan(BlackMarketCache.GetEventCost());
            FocusCache.waitWindowInstance.Close();

            LetterCache.GetLetterDetails("Successful black market",
                "The event has been sent successfully!", LetterDefOf.PositiveEvent);

            Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);
        }

        public static void GetRejectedEvent()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("Player is unavailable for this action"));
            FocusCache.waitWindowInstance.Close();
        }

        public static void GetEvent(Packet receivedPacket)
        {
            if (!BooleanCache.hasLoadedCorrectly) return;
            else
            {
                int eventType = int.Parse(receivedPacket.contents[0]);

                if (eventType == 0)
                {
                    incidentDef = IncidentDefOf.RaidEnemy;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Raid",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points,
                        faction = Faction.OfMechanoids,
                        raidStrategy = defaultParms.raidStrategy,
                        raidArrivalMode = defaultParms.raidArrivalMode,
                    }; 
                }

                else if (eventType == 1)
                {
                    incidentDef = IncidentDefOf.Infestation;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Infestation",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points,
                    };
                }

                else if (eventType == 2)
                {
                    incidentDef = IncidentDefOf.MechCluster;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Cluster",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points
                    };
                }

                else if (eventType == 3)
                {
                    foreach (GameCondition condition in Find.World.GameConditionManager.ActiveConditions)
                    {
                        if (condition.def == GameConditionDefOf.ToxicFallout) return;
                    }

                    incidentDef = IncidentDefOf.ToxicFallout;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Fallout",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points
                    };
                }

                else if (eventType == 4)
                {
                    incidentDef = IncidentDefOf.ManhunterPack;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Manhunter",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points
                    };
                }

                else if (eventType == 5)
                {
                    incidentDef = IncidentDefOf.WandererJoin;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Wanderer",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points
                    };
                }

                else if (eventType == 6)
                {
                    incidentDef = IncidentDefOf.FarmAnimalsWanderIn;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Animals",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points
                    };
                }

                else if (eventType == 7)
                {
                    incidentDef = IncidentDefOf.ShipChunkDrop;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points
                    };

                    LetterCache.GetLetterDetails("Black Market Event - Chunks",
                        "Space chunks seem to be falling from the sky! You might be able to get materials from them!", LetterDefOf.PositiveEvent);

                    Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);
                }

                else if (eventType == 8)
                {
                    incidentDef = IncidentDefOf.GiveQuest_Random;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Quest",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points
                    };
                }

                else if (eventType == 9)
                {
                    incidentDef = IncidentDefOf.TraderCaravanArrival;
                    IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

                    parms = new IncidentParms
                    {
                        customLetterLabel = "Black Market Event - Trader",
                        target = Find.AnyPlayerHomeMap,
                        points = defaultParms.points,
                        faction = FactionsCache.onlineNeutralFaction,
                        traderKind = defaultParms.traderKind
                    };
                }

                Injections.thingsToDoInUpdate.Add(ExecuteEvent);
            }
        }

        private static void ExecuteEvent()
        {
            incidentDef.Worker.TryExecute(parms);

            Injections.thingsToDoInUpdate.Add(MPGame.ForceSave);
        }
    }
}
