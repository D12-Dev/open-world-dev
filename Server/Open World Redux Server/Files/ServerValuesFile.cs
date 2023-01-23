using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class ServerValuesFile
    {
        //Black market
        public int RaidCost;
        public int InfestationCost;
        public int MechClusterCost;
        public int ToxicFalloutCost;
        public int ManhunterCost;
        public int WandererCost;
        public int FarmAnimalsCost;
        public int ShipChunkCost;
        public int GiveQuestCost;
        public int TraderCaravanCost;

        //Factions
        public int SiloCost;
        public int MarketplaceCost;
        public int ProductionSiteCost;
        public int WonderCost;
        public int BankCost;
        public int AeroportCost;
        public int CourierCost;

        public int AeroportUsageCost;
        public int CourierUsageCost;

        //Misc
        public int AidUsageCost;
    }
}
