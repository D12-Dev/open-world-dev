using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Verse;

namespace OpenWorldRedux
{
    public static class WorldHandler
    {
        public static bool HasBioTechFactions;
        public static bool HasRoyaltyFactions;
        public static bool HasIdeologyFactions;
        
        public static void CreateNewWorldHandle()
        {
            FocusCache.waitWindowInstance.Close();

            BooleanCache.isGeneratingNewWorld = true;

            Find.WindowStack.Add(new Page_CustomSelectScenario());

            string[] chainInfo = new string[]
            {
                "Welcome to the multiplayer world generation screen",
                "You are the first player joining this server",
                "Configure the settings that everyone else will have",
                "Locked variables are automatically handled by the server"
            };
            Find.WindowStack.Add(new OW_ChainInfoDialog(chainInfo));

        }

        public static void CreateWorldFromPacketHandle(Packet receivedPacket)
        {


 
          //  Log.Message("Getting world from packet");
            BooleanCache.isGeneratingWorldFromPacket = true;
            SaveHandler.LoadFromWorldGen(receivedPacket);
          //  Log.Message("Finished loading world from packet");
            FocusCache.waitWindowInstance.Close();





    




        }

        public static void SendWorldDataRequest()
        {
            Log.Message("Trying to send data request!");

            Packet GetSaveData = new Packet("ReceiveBaseSaveRequest");
            Network.SendData(GetSaveData);
        }

        public static void AddSettlementToWorld(Packet receivedPacket)
        {
            if (!BooleanCache.hasLoadedCorrectly) return;

            SettlementFile settlementFile = Serializer.SerializeToClass<SettlementFile>(receivedPacket.contents[0]);

            Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            settlement.Tile = settlementFile.settlementTile;
            settlement.Name = settlementFile.settlementUsername + "'s Settlement";
            settlement.SetFaction(FactionsCache.onlineNeutralFaction);

            Find.WorldObjects.Add(settlement);
            WorldCache.onlineSettlements.Add(settlement);
        }

        public static void RemoveSettlementFromWorld(Packet receivedPacket)
        {
            if (!BooleanCache.hasLoadedCorrectly) return;

            List<Settlement> settlements = Find.WorldObjects.Settlements.ToList();
            Settlement settlementToDestroy = settlements.Find(item => item.Tile == int.Parse(receivedPacket.contents[0]));
            if(settlementToDestroy != null)
            {
                Find.WorldObjects.Remove(settlementToDestroy);
                WorldCache.onlineSettlements.Remove(settlementToDestroy);
            }
        }

        public static void AddStructureToWorld(Packet receivedPacket)
        {
            if (!BooleanCache.hasLoadedCorrectly) return;

            StructureFile file = Serializer.SerializeToClass<StructureFile>(receivedPacket.contents[0]);

            SitePartDef siteDef = FactionCache.GetStructureDef(file.structureType);

            Faction factionToGet = null;
            if (file.structureFaction == 0) factionToGet = FactionsCache.onlineNeutralFaction;
            else if (file.structureFaction == 1) factionToGet = FactionsCache.onlineAllyFaction;
            else if (file.structureFaction == 2) factionToGet = FactionsCache.onlineEnemyFaction;

            Site newOnlineSite = SiteMaker.MakeSite(sitePart: siteDef,
                                              tile: file.structureTile,
                                              threatPoints: 5000,
                                              faction: factionToGet);

            if (siteDef == FactionCache.aeroportDef) newOnlineSite.customLabel = $"{newOnlineSite.Label} - {newOnlineSite.Tile}";

            Find.WorldObjects.Add(newOnlineSite);
            WorldCache.onlineStructuresDeflate.Add(file);
            WorldCache.onlineStructures.Add(newOnlineSite);
        }

        public static void RemoveStructureFromWorld(Packet receivedPacket)
        {
            if (!BooleanCache.hasLoadedCorrectly) return;

            List<Site> sites = Find.WorldObjects.Sites.ToList();
            Site siteToDestroy = sites.Find(item => item.Tile == int.Parse(receivedPacket.contents[0]));
            if (siteToDestroy != null)
            {
                Find.WorldObjects.Remove(siteToDestroy);
                WorldCache.onlineStructures.Remove(siteToDestroy);
            }
        }
        
        public static void PrepareWorld()
        {
            CleanWorld();
            RebuildWorld();
            PlaceDlcFactionsOnMap();
        }

        private static void CleanWorld()
        {

            //Destroy settlements
            Settlement[] settlementsToDestroy = Find.WorldObjects.Settlements.ToArray();
            foreach (Settlement settlement in settlementsToDestroy)
            {
                if (settlement.Faction == Faction.OfPlayer) continue;
                if (settlement.Faction == FactionsCache.onlineNeutralFaction ||
                    settlement.Faction == FactionsCache.onlineAllyFaction ||
                    settlement.Faction == FactionsCache.onlineEnemyFaction)
                {
                    Find.WorldObjects.Remove(settlement);



                }
                
                //Log.Message(settlement.ToString());
                //if (settlement in WorldCache.onlineSettlementsDeflate) continue;
                // else if (settlement.Faction == FactionsCache.onlineNeutralTribe) continue;
                // else if (settlement.Faction == FactionsCache.onlineEnemyTribe) continue;

               

















                // if(settlement.Faction == Faction.)
                // if (settlement.Faction == Faction.OfPlayer) continue;
                //  else if (settlement.Faction == FactionsCache.onlineNeutralTribe) continue;
                //   else if (settlement.Faction == FactionsCache.onlineEnemyTribe) continue;
                // else Find.WorldObjects.Remove(settlement);
            }

            //Destroy sites
            Site[] sitesToDestroy = Find.WorldObjects.Sites.ToArray();
            foreach (Site site in sitesToDestroy)
            {
                if (site.Faction == FactionsCache.onlineNeutralFaction ||
                    site.Faction == FactionsCache.onlineAllyFaction ||
                    site.Faction == FactionsCache.onlineEnemyFaction)
                {
                    Find.WorldObjects.Remove(site);
                }

                else continue;
            }

        }


        public static void PlaceDlcFactionsOnMap() { // Place dlc, use this for future map sync

                List<FactionDef> ToTryToAddFactions = new List<FactionDef>();

                if (ModsConfig.IsActive("ludeon.rimworld.biotech"))
                {
                    List<FactionDef> BiotechFactions = new List<FactionDef>() {
                        FactionDefOf.PirateWaster,


                    };
                    ToTryToAddFactions.AddRange(BiotechFactions);
                }
                if (ModsConfig.IsActive("ludeon.rimworld.royalty"))
                {
                    List<FactionDef> RoyaltyFactions = new List<FactionDef>() {
                        FactionDefOf.Empire,
                        FactionDefOf.OutlanderRefugee
                    };
                    ToTryToAddFactions.AddRange(RoyaltyFactions);
                }

                if (ModsConfig.IsActive("ludeon.rimworld.ideology"))
                {
                    List<FactionDef> IdeologyFactions = new List<FactionDef>() {
                            FactionDefOf.Pilgrims,
                            FactionDefOf.Beggars,
                            FactionDefOf.Ancients,
                            FactionDefOf.AncientsHostile,
                        };
                    ToTryToAddFactions.AddRange(IdeologyFactions);
                }
                foreach (FactionDef X in ToTryToAddFactions)
                {
                    Log.Message("Logging Faction Name");
                    Log.Message(X.defName);


                }
                if(ToTryToAddFactions.Count > 0) {


                    FloatRange SettlementsPer100kTiles = new FloatRange(150f, 190f);
                    if (ToTryToAddFactions != null)
                    {
                        foreach (FactionDef faction2 in ToTryToAddFactions)
                        {
                            Find.FactionManager.Add(FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(faction2)));
                        }
                    }
                    else
                    {
                        foreach (FactionDef item in DefDatabase<FactionDef>.AllDefs.OrderBy((FactionDef x) => x.hidden))
                        {
                            for (int i = 0; i < item.requiredCountAtGameStart; i++)
                            {
                                Find.FactionManager.Add(FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(item)));
                            }
                        }
                    }

                    IEnumerable<Faction> source = Find.World.factionManager.AllFactionsListForReading.Where((Faction x) => !x.def.isPlayer && !x.Hidden && !x.temporary);
                    if (source.Any())
                    {
                        int num = GenMath.RoundRandom((float)Find.WorldGrid.TilesCount / 100000f * SettlementsPer100kTiles.RandomInRange * Find.World.info.overallPopulation.GetScaleFactor());
                        num -= Find.WorldObjects.Settlements.Count;
                        for (int j = 0; j < num; j++)
                        {
                            Faction faction = source.RandomElementByWeight((Faction x) => x.def.settlementGenerationWeight);
                            Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                            settlement.SetFaction(faction);
                            settlement.Tile = TileFinder.RandomSettlementTileFor(faction);
                            settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement);
                            Find.WorldObjects.Add(settlement);
                        }
                    }

                    Find.IdeoManager.SortIdeos();


               //FactionGenerator.GenerateFactionsIntoWorld(ToTryToAddFactions);

                    Log.Message("[Open World] > Trying to add missing dlc factions");
                }
        }

        private static void RebuildWorld()
        {






            //Spawn settlements
            WorldCache.onlineSettlements.Clear(); // This could kill settlement
            foreach (SettlementFile settlement in WorldCache.onlineSettlementsDeflate)
            {
                Settlement newOnlineSettlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                newOnlineSettlement.Tile = settlement.settlementTile;
                newOnlineSettlement.Name = settlement.settlementUsername + "'s Settlement";

                if (settlement.settlementFaction == 0) newOnlineSettlement.SetFaction(FactionsCache.onlineNeutralFaction);
                else if (settlement.settlementFaction == 1) newOnlineSettlement.SetFaction(FactionsCache.onlineAllyFaction);
                else if (settlement.settlementFaction == 2) newOnlineSettlement.SetFaction(FactionsCache.onlineEnemyFaction);

                WorldCache.onlineSettlements.Add(newOnlineSettlement);
            }
            foreach (Settlement onlineSettlement in WorldCache.onlineSettlements) Find.WorldObjects.Add(onlineSettlement);

            //Spawn sites
            WorldCache.onlineStructures.Clear();
            foreach (StructureFile file in WorldCache.onlineStructuresDeflate)
            {
                SitePartDef siteDef = FactionCache.GetStructureDef(file.structureType);

                if (siteDef == null) continue;
                else
                {
                    Faction factionToGet = null;
                    if (file.structureFaction == 0) factionToGet = FactionsCache.onlineNeutralFaction;
                    else if (file.structureFaction == 1) factionToGet = FactionsCache.onlineAllyFaction;
                    else if (file.structureFaction == 2) factionToGet = FactionsCache.onlineEnemyFaction;

                    Site newOnlineSite = SiteMaker.MakeSite(sitePart: siteDef,
                                                      tile: file.structureTile,
                                                      threatPoints: 5000,
                                                      faction: factionToGet);

                    if (siteDef == FactionCache.aeroportDef) newOnlineSite.customLabel = $"{newOnlineSite.Label} - {newOnlineSite.Tile}";

                    WorldCache.onlineStructures.Add(newOnlineSite);
                }
            }
            foreach (Site onlineSite in WorldCache.onlineStructures) Find.WorldObjects.Add(onlineSite);
        }

        public static void TryPatchOldWorlds()
        {
            if (!Find.FactionManager.AllFactions.Contains(FactionsCache.onlineNeutralFaction))
            {


                Log.Message("[Open World] > Trying to add missing factions");

                FactionGenerator.GenerateFactionsIntoWorld(WorldCache.factions);
            }
        }

        public static void ParseCurrentWorldSettlements(Packet receivedPacket)
        {
            WorldCache.onlineSettlementsDeflate.Clear();

            foreach (string str in receivedPacket.contents)
            {
                SettlementFile settlement = Serializer.SerializeToClass<SettlementFile>(str);
                WorldCache.onlineSettlementsDeflate.Add(settlement);
            }
        }

        public static void ParseCurrentWorldStructures(Packet receivedPacket)
        {
            WorldCache.onlineStructuresDeflate.Clear();

            foreach(string str in receivedPacket.contents)
            {
                StructureFile file = Serializer.SerializeToClass<StructureFile>(str);
                WorldCache.onlineStructuresDeflate.Add(file);
            }
        }
    }
}
