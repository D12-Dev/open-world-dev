using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class TradeCache
    {
        public static TradeHandler.TakeMode takeMode;
        public static List<Tradeable> listToShowInTradesMenu = new List<Tradeable>();
        public static List<TradeItem> tradeItems = new List<TradeItem>();
        public static List<TradeItem> incomingBarterItems = new List<TradeItem>();
        public static int wantedSilver;
        public static bool inRebarter;
        public static bool inTrade;

        public static void ResetTradeVariables()
        {
            inTrade = false;
            inRebarter = false;

            tradeItems.Clear();
            incomingBarterItems.Clear();
        }
    }
}
