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
            FactionDefOf.Mechanoid,
           //FactionDefOf.Empire,
            FactionDefOf.TribeCivil,
            FactionDefOf.TribeRough,
            //FactionDefOf.PirateWaster,
           // FactionDefOf.Beggars,
            FactionDefOf.OutlanderCivil,
            //FactionDefOf.OutlanderRefugee,
            FactionDefOf.OutlanderRough,
            FactionDefOf.Insect,
           // FactionDefOf.Ancients,
            //FactionDefOf.AncientsHostile,
            //FactionDefOf.Pilgrims
            
        };

        public static void AddFactionDlc()
        {
            if (ModsConfig.IsActive("ludeon.rimworld.royalty"))
            {
                WorldCache.factions.Add(FactionDefOf.Empire);
                WorldCache.factions.Add(FactionDefOf.OutlanderRefugee);
            };
            if (ModsConfig.IsActive("ludeon.rimworld.biotech"))
            {
                WorldCache.factions.Add(FactionDefOf.PirateWaster);
                //FactionDefOf.PirateWaster,   
            };
            if (ModsConfig.IsActive("ludeon.rimworld.ideology"))
            {
                WorldCache.factions.Add(FactionDefOf.Beggars);
                WorldCache.factions.Add(FactionDefOf.Ancients);
                WorldCache.factions.Add(FactionDefOf.AncientsHostile);
                WorldCache.factions.Add(FactionDefOf.Pilgrims);
                // FactionDefOf.Ancients,
                //FactionDefOf.AncientsHostile,
                //FactionDefOf.Pilgrims
                //FactionDefOf.Beggars,   
            };
            
        }

        public static float pollution;

        public static List<SettlementFile> onlineSettlementsDeflate = new List<SettlementFile>();
        public static List<Settlement> onlineSettlements = new List<Settlement>();

        public static List<StructureFile> onlineStructuresDeflate = new List<StructureFile>();
        public static List<Site> onlineStructures = new List<Site>();
    }

}
