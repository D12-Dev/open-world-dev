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

        public static Faction onlineNeutralTribe;
        public static Faction onlineEnemyTribe;

        public static FactionDef onlineNeutralFactionDef;
        public static FactionDef onlineAllyFactionDef;
        public static FactionDef onlineEnemyFactionDef;

        public static FactionDef onlineNeutralTribeDef;
        public static FactionDef onlineEnemyTribeDef;

        public static List<Faction> allOnlineFactions = new List<Faction>();

        public static void FindOnlineFactionsInWorld()
        {
            List<Faction> factions = Find.FactionManager.AllFactions.ToList();
            onlineNeutralFaction = factions.Find(fetch => fetch.def.defName == "OnlineNeutral");
            onlineAllyFaction = factions.Find(fetch => fetch.def.defName == "OnlineAlly");
            onlineEnemyFaction = factions.Find(fetch => fetch.def.defName == "OnlineEnemy");

            onlineNeutralTribe = factions.Find(fetch => fetch.def.defName == "OnlineNeutralTribe");
            onlineEnemyTribe = factions.Find(fetch => fetch.def.defName == "OnlineEnemyTribe");

            allOnlineFactions.Clear();
            allOnlineFactions.Add(onlineNeutralFaction);
            allOnlineFactions.Add(onlineAllyFaction);
            allOnlineFactions.Add(onlineEnemyFaction);

            //allOnlineFactions.Add(onlineNeutralTribe);
            //allOnlineFactions.Add(onlineEnemyTribe);
        }

        public static void FindFactionDefsInGame()
        {
            foreach (FactionDef def in DefDatabase<FactionDef>.AllDefs)
            {
                if (def.defName == "OnlineNeutral") onlineNeutralFactionDef = def;
                else if (def.defName == "OnlineAlly") onlineAllyFactionDef = def;
                else if (def.defName == "OnlineEnemy") onlineEnemyFactionDef = def;
                else if (def.defName == "OnlineNeutralTribe") onlineNeutralTribeDef = def;
                else if (def.defName == "OnlineEnemyTribe") onlineEnemyTribeDef = def;
            }
        }
    }
}
