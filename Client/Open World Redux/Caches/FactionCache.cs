using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace OpenWorldRedux
{
    public static class FactionCache
    {
        public static bool hasFaction;

        public static string factionName;

        public enum MemberRank { Member, Moderator, Admin }

        public enum StructureTypes { Silo, Marketplace, ProductionSite, Wonder, Bank, Aeroport, Courier };

        public static List<Tuple<string, MemberRank>> factionMembers = new List<Tuple<string, MemberRank>>();

        public static Dictionary<StructureTypes, int> costDictionary = new Dictionary<StructureTypes, int> { };

        public static Dictionary<StructureTypes, string> nameDictionary = new Dictionary<StructureTypes, string> { };

        public static Dictionary<int, SitePartDef> defDictionary = new Dictionary<int, SitePartDef> { };

        public static List<Tuple<ProductionSiteProduct, string, int>> productionSiteDataList = new List<Tuple<ProductionSiteProduct, string, int>>()
        {
            Tuple.Create(ProductionSiteProduct.Food, "RawPotatoes", 25),
            Tuple.Create(ProductionSiteProduct.Neutroamine, "Neutroamine", 1),
            Tuple.Create(ProductionSiteProduct.Components, "ComponentIndustrial", 1),
            Tuple.Create(ProductionSiteProduct.Fuel, "Chemfuel", 10)
        };

        public static int siloCost;
        public static int marketplaceCost;
        public static int productionSiteCost;
        public static int wonderCost;
        public static int bankCost;
        public static int aeroportCost;
        public static int courierCost;

        public static int aeroportUsageCost;
        public static int courierUsageCost;

        public static SitePartDef siloDef;
        public static SitePartDef marketplaceDef;
        public static SitePartDef productionSiteDef;
        public static SitePartDef wonderDef;
        public static SitePartDef bankDef;
        public static SitePartDef aeroportDef;
        public static SitePartDef courierDef;

        public static int bankSilver;

        public static bool canWithdrawFromSilo = true;

        public static List<TradeItem> siloContents = new List<TradeItem>();

        public enum ProductionSiteProduct { Food, Neutroamine, Components, Fuel }
        public static ProductionSiteProduct selectedProduct;

        public static void ReadStructureValues()
        {
            costDictionary = new Dictionary<StructureTypes, int>
            {
                { StructureTypes.Silo, siloCost },
                { StructureTypes.Marketplace, marketplaceCost },
                { StructureTypes.ProductionSite, productionSiteCost },
                { StructureTypes.Wonder, wonderCost },
                { StructureTypes.Bank, bankCost },
                { StructureTypes.Aeroport, aeroportCost },
                { StructureTypes.Courier, courierCost }
            };

            ReadStructureDefs();
        }

        public static void ReadStructureDefs()
        {
            foreach (SitePartDef def in DefDatabase<SitePartDef>.AllDefs)
            {
                if (def.defName == "OnlineSilo") siloDef = def;
                else if (def.defName == "OnlineMarketplace") marketplaceDef = def;
                else if (def.defName == "OnlineProductionSite") productionSiteDef = def;
                else if (def.defName == "OnlineWonder") wonderDef = def;
                else if (def.defName == "OnlineBank") bankDef = def;
                else if (def.defName == "OnlineAeroport") aeroportDef = def;
                else if (def.defName == "OnlineCourier") courierDef = def;
            }

            defDictionary = new Dictionary<int, SitePartDef>
            {
                { 0, siloDef},
                { 1, marketplaceDef},
                { 2, productionSiteDef},
                { 3, wonderDef},
                { 4, bankDef},
                { 5, aeroportDef},
                { 6, courierDef}
            };
        }

        public static int GetStructureCost(int selectedStructure)
        {
            foreach (KeyValuePair<StructureTypes, int> pair in costDictionary)
            {
                if ((int)pair.Key == selectedStructure)
                {
                    return pair.Value;
                }
            }

            return 0;
        }

        public static SitePartDef GetStructureDef(int structureType)
        {
            foreach(KeyValuePair<int, SitePartDef> pair in defDictionary)
            {
                if (pair.Key == structureType) return pair.Value;
            }

            return null;
        }

        public static TradeItem GetProductionSiteItem()
        {
            foreach (Tuple<ProductionSiteProduct, string, int> tuple in productionSiteDataList)
            {
                if (tuple.Item1 == selectedProduct)
                {
                    TradeItem newTradeItem = new TradeItem();
                    newTradeItem.defName = tuple.Item2;
                    newTradeItem.stackCount = tuple.Item3;
                    return newTradeItem;
                }
            }

            return null;
        }

        public static void GetBankSilver(Packet receivedPacket)
        {
            bankSilver = int.Parse(receivedPacket.contents[0]);
        }

        public static void GetSiloItems(Packet receivedPacket)
        {
            siloContents.Clear();
            foreach(string str in receivedPacket.contents)
            {
                TradeItem item = Serializer.SerializeToClass<TradeItem>(str);
                siloContents.Add(item);
            }
        }
    }
}
