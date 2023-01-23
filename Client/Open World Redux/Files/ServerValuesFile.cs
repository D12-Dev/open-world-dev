using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
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

        public void ApplyValues()
        {
            BlackMarketCache.raidCost = RaidCost;
            BlackMarketCache.infestationCost = InfestationCost;
            BlackMarketCache.mechClusterCost = MechClusterCost;
            BlackMarketCache.toxicFalloutCost = ToxicFalloutCost;
            BlackMarketCache.manhunterCost = ManhunterCost;
            BlackMarketCache.wandererCost = WandererCost;
            BlackMarketCache.farmAnimalsCost = FarmAnimalsCost;
            BlackMarketCache.shipChunkCost = ShipChunkCost;
            BlackMarketCache.giveQuestCost = GiveQuestCost;
            BlackMarketCache.traderCaravanCost = TraderCaravanCost;

            BlackMarketCache.ReadEventValues();

            FactionCache.siloCost = SiloCost;
            FactionCache.marketplaceCost = MarketplaceCost;
            FactionCache.productionSiteCost = ProductionSiteCost;
            FactionCache.wonderCost = WonderCost;
            FactionCache.bankCost = BankCost;
            FactionCache.aeroportCost = AeroportCost;
            FactionCache.courierCost = CourierCost;

            FactionCache.aeroportUsageCost = AeroportUsageCost;
            FactionCache.courierUsageCost = CourierUsageCost;

            FactionCache.ReadStructureValues();

            AidHandler.aidUsageCost = AidUsageCost;
        }
    }
}
