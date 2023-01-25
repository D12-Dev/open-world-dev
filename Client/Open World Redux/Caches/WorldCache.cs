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
            FactionsCache.onlineNeutralTribeDef,
            FactionsCache.onlineEnemyTribeDef,
            FactionDefOf.Mechanoid,
            FactionDefOf.Insect,
            FactionDefOf.Ancients,
            FactionDefOf.AncientsHostile,
            FactionDefOf.Pilgrims
        };

        public static float pollution;

        public static List<SettlementFile> onlineSettlementsDeflate = new List<SettlementFile>();
        public static List<Settlement> onlineSettlements = new List<Settlement>();

        public static List<StructureFile> onlineStructuresDeflate = new List<StructureFile>();
        public static List<Site> onlineStructures = new List<Site>();
    }
}
