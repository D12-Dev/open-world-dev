using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class FactionsCache
    {
        public static Faction onlineNeutralFaction;
        public static Faction onlineAllyFaction;
        public static Faction onlineEnemyFaction;

        public static FactionDef onlineNeutralFactionDef;
        public static FactionDef onlineAllyFactionDef;
        public static FactionDef onlineEnemyFactionDef;

        public static List<Faction> allOnlineFactions = new List<Faction>();

        public static void FindOnlineFactionsInWorld()
        {
            List<Faction> factions = Find.FactionManager.AllFactions.ToList();
            onlineNeutralFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Neutral");
            onlineAllyFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Ally");
            onlineEnemyFaction = factions.Find(fetch => fetch.Name == "Open World Settlements Enemy");

            allOnlineFactions.Clear();
            allOnlineFactions.Add(onlineNeutralFaction);
            allOnlineFactions.Add(onlineAllyFaction);
            allOnlineFactions.Add(onlineEnemyFaction);
        }

        public static void FindFactionDefsInGame()
        {
            foreach (FactionDef def in DefDatabase<FactionDef>.AllDefs)
            {
                if (def.defName == "OnlineNeutral") onlineNeutralFactionDef = def;
                if (def.defName == "OnlineAlly") onlineAllyFactionDef = def;
                if (def.defName == "OnlineEnemy") onlineEnemyFactionDef = def;
            }
        }
    }
}
