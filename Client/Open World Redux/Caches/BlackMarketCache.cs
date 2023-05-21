using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class BlackMarketCache
    {
        public static int raidCost;
        public static int infestationCost;
        public static int mechClusterCost;
        public static int toxicFalloutCost;
        public static int manhunterCost;
        public static int wandererCost;
        public static int farmAnimalsCost;
        public static int shipChunkCost;
        public static int giveQuestCost;
        public static int traderCaravanCost;

        public static int selectedEvent;

        public enum EventTypes
        {
            Raid,
            Infestation,
            MechCluster,
            ToxicFallout,
            Manhunter,
            Wanderer,
            FarmAnimals,
            ShipChunk,
            GiveQuest,
            TraderCaravan
        }

        public static Dictionary<EventTypes, int> costDictionary = new Dictionary<EventTypes, int> { };

        public static void ReadEventValues()
        {
            costDictionary = new Dictionary<EventTypes, int>
            {
                { EventTypes.Raid, raidCost },
                { EventTypes.Infestation, infestationCost },
                { EventTypes.MechCluster, mechClusterCost },
                { EventTypes.ToxicFallout, toxicFalloutCost },
                { EventTypes.Manhunter, manhunterCost },
                { EventTypes.Wanderer, wandererCost },
                { EventTypes.FarmAnimals, farmAnimalsCost },
                { EventTypes.ShipChunk, shipChunkCost },
                { EventTypes.GiveQuest, giveQuestCost },
                { EventTypes.TraderCaravan, traderCaravanCost }
            };
        }

        public static int GetEventCost()
        {
           foreach (KeyValuePair<EventTypes, int> pair in costDictionary)
           {
                if ((int)pair.Key == selectedEvent)
                {
                    return pair.Value;
                }
           }

            return 0;
        }
    }
}
