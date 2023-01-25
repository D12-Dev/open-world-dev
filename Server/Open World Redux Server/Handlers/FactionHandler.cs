using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using static OpenWorldReduxServer.FactionFile;

namespace OpenWorldReduxServer
{
    public static class FactionHandler
    {
        public static void CreateFaction(ServerClient client, Packet receivedPacket)
        {
            if (string.IsNullOrWhiteSpace(receivedPacket.contents[0])) return;
            else
            {
                if (CheckIfFactionExists(receivedPacket.contents[0]))
                {
                    Packet FactionAlreadyExistsPacket = new Packet("FactionAlreadyExistsPacket");
                    Network.SendData(client, FactionAlreadyExistsPacket);
                }

                else
                {
                    FactionFile newFaction = new FactionFile();
                    newFaction.factionName = receivedPacket.contents[0];
                    newFaction.factionMembers = new List<Tuple<string, MemberRank>>();
                    newFaction.AddMemberToFaction(client);
                }
            }
        }

        public static void RefreshFactionDetails(ServerClient client)
        {
            FactionFile faction = GetFactionFromClient(client);

            if (faction == null)
            {
                Packet NoFactionPacket = new Packet("NoFactionPacket");
                Network.SendData(client, NoFactionPacket);
            }

            else faction.GetFactionDetails(client);
        }

        public static void LeaveFaction(ServerClient client)
        {
            FactionFile faction = GetFactionFromClient(client);

            if (faction == null) return;
            else faction.RemoveMemberFromFaction(client, client.Username);
        }

        public static void DisbandFaction(ServerClient client)
        {
            FactionFile faction = GetFactionFromClient(client);

            if (faction == null) return;
            else faction.DisbandFaction(client);
        }

        public static void PromoteMember(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.PromoteMember(client, receivedPacket);
        }

        public static void DemoteMember(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.DemoteMember(client, receivedPacket);
        }

        public static void AddMember(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.AddMemberFromSettlement(client, receivedPacket);
        }

        public static void RemoveMember(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.RemoveMemberFromSettlement(client, receivedPacket);
        }

        public static void AcceptInvitation(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromFiles(receivedPacket.contents[0]);
            faction.AddMemberToFaction(client);
        }

        public static void BuildStructure(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.BuildStructure(client, receivedPacket);
        }

        public static void DestroyStructure(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.DestroyStructure(client, receivedPacket);
        }

        public static void WithdrawFromFactionSilo(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.WithdrawItemFromSilo(client, receivedPacket);
        }

        public static void GetAllFactionStructures(ServerClient client)
        {
            List<string> contents = new List<string>();

            string[] factions = Directory.GetFiles(Server.factionsFolderPath);
            foreach (string faction in factions)
            {
                FactionFile file = Serializer.DeserializeFromFile<FactionFile>(faction);
                int relationShip = GetRelationShipWithPlayer(client, file);

                foreach(StructureFile structure in file.factionStructures)
                {
                    structure.structureFaction = relationShip;
                    contents.Add(Serializer.SoftSerialize(structure));
                }
            }

            Packet CurrentStructuresInWorldPacket = new Packet("CurrentStructuresInWorldPacket", contents.ToArray());
            Network.SendData(client, CurrentStructuresInWorldPacket);
        }

        public static FactionFile GetFactionFromClient(ServerClient client)
        {
            string[] factions = Directory.GetFiles(Server.factionsFolderPath);
            foreach (string faction in factions)
            {
                FactionFile file = Serializer.DeserializeFromFile<FactionFile>(faction);
                if (file.factionName == client.FactionName) return file;
            }

            return null;
        }

        public static FactionFile GetFactionFromFiles(string factionName)
        {
            string[] factions = Directory.GetFiles(Server.factionsFolderPath);
            foreach (string faction in factions)
            {
                FactionFile file = Serializer.DeserializeFromFile<FactionFile>(faction);
                if (file.factionName == factionName) return file;
            }

            return null;
        }

        public static int GetRelationShipWithPlayer(ServerClient client, FactionFile file)
        {
            if (client.FactionName == null) return 0;
            else if (client.FactionName == file.factionName) return 1;
            else return 2;
        }

        public static bool CheckIfFactionExists(string factionName)
        {
            string[] factions = Directory.GetFiles(Server.factionsFolderPath);
            foreach (string faction in factions)
            {
                FactionFile file = Serializer.DeserializeFromFile<FactionFile>(faction);
                if (file.factionName == factionName) return true;
            }

            return false;
        }

        public static void RefreshBank(ServerClient client)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.RefreshBank();
        }

        public static void RefreshSilo(ServerClient client)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.RefreshSilo();
        }

        public static void DepositSilverToBank(ServerClient client, string[] contents)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.DepositSilverIntoBank(client, contents);
        }

        public static void DepositItemsIntoSilo(ServerClient client, string[] contents)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.DepositItemsIntoSilo(contents);
        }

        public static void WithdrawFromFactionBank(ServerClient client, Packet receivedPacket)
        {
            FactionFile faction = GetFactionFromClient(client);
            faction.WithdrawSilverFromBank(client, receivedPacket);
        }

        public static void StructureClock()
        {
            while(true)
            {
                Thread.Sleep(600000);

                try
                {
                    string[] factions = Directory.GetFiles(Server.factionsFolderPath);
                    foreach (string faction in factions)
                    {
                        FactionFile factionFile = Serializer.DeserializeFromFile<FactionFile>(faction);

                        bool hasBanks = false;
                        bool hasStructures = false;

                        StructureFile[] factionStructures = factionFile.factionStructures.ToArray();
                        foreach (StructureFile structure in factionStructures)
                        {
                            if (structure.structureType == (int)StructureTypes.Bank)
                            {
                                if (factionFile.bankSilver >= Server.serverDeepConfigs.MaximumCashInBank) continue;
                                else
                                {
                                    hasBanks = true;
                                    factionFile.bankSilver += Server.serverDeepConfigs.BankCashPerTick;
                                }
                            }

                            else if (structure.structureType == (int)StructureTypes.ProductionSite)
                            {
                                hasStructures = true;
                            }
                        }

                        if (hasStructures) factionFile.SendProduction();
                        if (hasBanks)
                        {
                            factionFile.RefreshBank();
                            factionFile.SaveFaction();
                        }
                    }

                    ServerHandler.WriteToConsole($"[Structure clock tick]", ServerHandler.LogMode.Title);
                }

                catch { ServerHandler.WriteToConsole("Critical error occured in structure clock thread", ServerHandler.LogMode.Error); }
            }
        }

        public static bool CheckIfUniqueStructureExists()
        {
            string[] factions = Directory.GetFiles(Server.factionsFolderPath);
            foreach (string faction in factions)
            {
                FactionFile factionFile = Serializer.DeserializeFromFile<FactionFile>(faction);

                StructureFile[] factionStructures = factionFile.factionStructures.ToArray();
                foreach (StructureFile structure in factionStructures)
                {
                    if (structure.structureType == (int)StructureTypes.Wonder)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void ChangeProductionSiteItem(ServerClient client, Packet receivedPacket)
        {
            client.selectedProductionItem = int.Parse(receivedPacket.contents[0]);
            ClientHandler.SaveClient(client);
        }
    }
}
