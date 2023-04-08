using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class WorldCache
    {

        public static string seedString;

        public static float planetCoverage;

        public static OverallRainfall overallRainfall;

        public static OverallTemperature overallTemperature;

        public static OverallPopulation overallPopulation;
        public static List<FactionDef> factions = new List<FactionDef>()
        {
            FactionsCache.onlineNeutralFactionDef,
            FactionsCache.onlineAllyFactionDef,
            FactionsCache.onlineEnemyFactionDef,

        };
  
        public static void CreateInitalFactionsForWorldCache()
        {
            List<FactionDef> curfactions;

            List<FactionDef> initiallyFactions;


            curfactions = new List<FactionDef>();
            foreach (FactionDef configurableFaction in FactionGenerator.ConfigurableFactions)
            {
                if (configurableFaction.startingCountAtWorldCreation > 0)
                {
                    for (int i = 0; i < configurableFaction.startingCountAtWorldCreation; i++)
                    {
                        curfactions.Add(configurableFaction);
                    }
                }
            }
            foreach (FactionDef faction in FactionGenerator.ConfigurableFactions)
            {
                if (faction.replacesFaction != null)
                {
                    curfactions.RemoveAll((FactionDef x) => x == faction.replacesFaction);
                }
            }
            initiallyFactions = new List<FactionDef>();
            initiallyFactions.AddRange(curfactions);
            WorldCache.factions.AddRange(initiallyFactions);
        }

        public static float pollution;

        public static List<SettlementFile> onlineSettlementsDeflate = new List<SettlementFile>();
        public static List<Settlement> onlineSettlements = new List<Settlement>();

        public static List<StructureFile> onlineStructuresDeflate = new List<StructureFile>();
        public static List<Site> onlineStructures = new List<Site>();
    }

}
