using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class FactionFile
    {
        public string factionName;

        public enum MemberRank { Member, Moderator, Admin }

        public List<string> memberString = new List<string>();

        public List<Tuple<string, MemberRank>> factionMembers = new List<Tuple<string, MemberRank>>();

        public enum StructureTypes { Silo, Marketplace, ProductionSite, Wonder, Bank, Aeroport, Courier };

        public int bankSilver;

        public List<TradeItem> siloItems = new List<TradeItem>();

        public List<StructureFile> factionStructures = new List<StructureFile>();

        public void AddMemberToFaction(ServerClient client)
        {
            FactionFile factionFile = FactionHandler.GetFactionFromClient(client);

            if (factionFile != null)
            {
                Packet InAnotherFactionErrorPacket = new Packet("InAnotherFactionErrorPacket");
                Network.SendData(client, InAnotherFactionErrorPacket);
            }

            else
            {
                Tuple<string, MemberRank> newMember;
                if (factionMembers.Count == 0) newMember = new Tuple<string, MemberRank>(client.Username, MemberRank.Admin);
                else newMember = new Tuple<string, MemberRank>(client.Username, MemberRank.Member);
                factionMembers.Add(newMember);

                SaveFaction();

                client.FactionName = factionName;
                ClientHandler.SaveClient(client);

                GiveDetailsToEveryMember();
            }
        }

        public void RemoveMemberFromFaction(ServerClient client, string memberUsername)
        {
            Tuple<string, MemberRank> toRemove = null;
            Tuple<string, MemberRank>[] members = factionMembers.ToArray();
            foreach (Tuple<string, MemberRank> member in members)
            {
                if (member.Item1 == memberUsername)
                {
                    toRemove = member;
                    break;
                }
            }

            if (toRemove == null)
            {
                Packet NotInFactionErrorPacket = new Packet("NotInFactionErrorPacket");
                Network.SendData(client, NotInFactionErrorPacket);
            }

            else
            {
                ServerClient foundClient = ClientHandler.GetClientFromSave(memberUsername);
                if (foundClient != null)
                {
                    foundClient.FactionName = null;
                    ClientHandler.SaveClient(foundClient);
                }

                ServerClient connectedClient = ClientHandler.GetClientFromConnected(memberUsername);
                if (connectedClient != null)
                {
                    connectedClient.FactionName = null;

                    Packet NoFactionPacket = new Packet("NoFactionPacket");
                    Network.SendData(connectedClient, NoFactionPacket);
                }

                if (factionMembers.Count == 1) DeleteFaction();
                else
                {
                    factionMembers.Remove(toRemove);
                    SaveFaction();
                }
            }
        }

        public void DisbandFaction(ServerClient client)
        {
            if (GetMemberRank(client) != MemberRank.Admin)
            {
                Packet NotEnoughPowerErrorPacket = new Packet("NotEnoughPowerErrorPacket");
                Network.SendData(client, NotEnoughPowerErrorPacket);
            }

            else
            {
                Packet NoFactionPacket = new Packet("NoFactionPacket");

                Tuple<string, MemberRank>[] members = factionMembers.ToArray();

                foreach (Tuple<string, MemberRank> member in members)
                {
                    ServerClient foundClient = ClientHandler.GetClientFromSave(member.Item1);
                    if (foundClient != null)
                    {
                        foundClient.FactionName = null;
                        ClientHandler.SaveClient(foundClient);
                    }
                }

                foreach (Tuple<string, MemberRank> member in members)
                {
                    ServerClient connectedClient = ClientHandler.GetClientFromConnected(member.Item1);
                    if (connectedClient != null)
                    {
                        Network.SendData(connectedClient, NoFactionPacket);
                    }
                }

                DeleteFaction();
            }
        }

        public void DeleteFaction()
        {
            string filePath = Server.factionsFolderPath + Path.DirectorySeparatorChar + factionName + ".json";
            File.Delete(filePath);
        }

        public void GetFactionDetails(ServerClient client)
        {
            PopulateMemberString();

            string[] contents = new string[] { Serializer.SoftSerialize(this) };
            Packet GetFactionDetails = new Packet("GetFactionDetails", contents);
            Network.SendData(client, GetFactionDetails);
        }

        public MemberRank GetMemberRank(ServerClient client)
        {
            Tuple<string, MemberRank>[] members = factionMembers.ToArray();
            foreach (Tuple<string, MemberRank> member in members)
            {
                if (member.Item1 == client.Username) return member.Item2;
            }

            return MemberRank.Member;
        }

        public void PopulateMemberString()
        {
            memberString.Clear();
            Tuple<string, MemberRank>[] members = factionMembers.ToArray();
            foreach (Tuple<string, MemberRank> member in members)
            {
                memberString.Add(member.Item1 + "," + (int)member.Item2);
            }
        }

        public void GiveDetailsToEveryMember()
        {
            Tuple<string, MemberRank>[] members = factionMembers.ToArray();
            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach (Tuple<string, MemberRank> member in members)
            {
                foreach (ServerClient connectedClient in connectedClients)
                {
                    if (connectedClient.Username == member.Item1)
                    {
                        GetFactionDetails(connectedClient);
                    }
                }
            }
        }

        public void SaveFaction()
        {
            string filePath = Server.factionsFolderPath + Path.DirectorySeparatorChar + factionName + ".json";
            Serializer.SerializeToFile(this, filePath);
        }

        public void PromoteMember(ServerClient client, Packet receivedPacket)
        {
            if (GetMemberRank(client) != MemberRank.Admin)
            {
                Packet NotEnoughPowerErrorPacket = new Packet("NotEnoughPowerErrorPacket");
                Network.SendData(client, NotEnoughPowerErrorPacket);
            }

            else
            {
                ServerClient toPromote = SettlementHandler.GetPlayerFileFromSettlement(int.Parse(receivedPacket.contents[0]));

                Tuple<string, MemberRank> toRemove = null;
                Tuple<string, MemberRank> toAdd = null;
                foreach (Tuple<string, MemberRank> member in factionMembers)
                {
                    if (member.Item1 == toPromote.Username)
                    {
                        toRemove = member;
                        toAdd = Tuple.Create(member.Item1, MemberRank.Moderator);
                        break;
                    }
                }

                if (toRemove == null)
                {
                    Packet NotInFactionErrorPacket = new Packet("NotInFactionErrorPacket");
                    Network.SendData(client, NotInFactionErrorPacket);
                }

                else
                {
                    factionMembers.Remove(toRemove);
                    factionMembers.Add(toAdd);

                    SaveFaction();

                    GiveDetailsToEveryMember();
                }
            }
        }

        public void DemoteMember(ServerClient client, Packet receivedPacket)
        {
            if (GetMemberRank(client) != MemberRank.Admin)
            {
                Packet NotEnoughPowerErrorPacket = new Packet("NotEnoughPowerErrorPacket");
                Network.SendData(client, NotEnoughPowerErrorPacket);
            }

            else
            {
                ServerClient toDemote = SettlementHandler.GetPlayerFileFromSettlement(int.Parse(receivedPacket.contents[0]));

                Tuple<string, MemberRank> toRemove = null;
                Tuple<string, MemberRank> toAdd = null;
                foreach (Tuple<string, MemberRank> member in factionMembers)
                {
                    if (member.Item1 == toDemote.Username)
                    {
                        toRemove = member;
                        toAdd = Tuple.Create(member.Item1, MemberRank.Member);
                        break;
                    }
                }

                if (toRemove == null)
                {
                    Packet NotInFactionErrorPacket = new Packet("NotInFactionErrorPacket");
                    Network.SendData(client, NotInFactionErrorPacket);
                }

                else
                {
                    factionMembers.Remove(toRemove);
                    factionMembers.Add(toAdd);

                    SaveFaction();

                    GiveDetailsToEveryMember();
                }
            }
        }

        public void AddMemberFromSettlement(ServerClient client, Packet receivedPacket)
        {
            if (GetMemberRank(client) == MemberRank.Member)
            {
                Packet NotEnoughPowerErrorPacket = new Packet("NotEnoughPowerErrorPacket");
                Network.SendData(client, NotEnoughPowerErrorPacket);
            }

            else
            {
                ServerClient toAdd = null;

                ServerClient[] connectedClients = Network.connectedClients.ToArray();
                foreach (ServerClient connectedClient in connectedClients)
                {
                    if (connectedClient.SettlementTile == receivedPacket.contents[0])
                    {
                        toAdd = connectedClient;
                        break;
                    }
                }

                if (toAdd != null)
                {
                    FactionFile factionFile = FactionHandler.GetFactionFromClient(toAdd);

                    if (factionFile != null)
                    {
                        Packet InAnotherFactionErrorPacket = new Packet("InAnotherFactionErrorPacket");
                        Network.SendData(client, InAnotherFactionErrorPacket);
                    }

                    else
                    {
                        string[] contents = new string[] { client.FactionName };
                        Packet FactionInvitationPacket = new Packet("FactionInvitationPacket", contents);
                        Network.SendData(toAdd, FactionInvitationPacket);
                    }
                }

                else
                {
                    Packet PlayerNotAvailablePacket = new Packet("PlayerNotAvailablePacket");
                    Network.SendData(client, PlayerNotAvailablePacket);
                }
            }
        }

        public void RemoveMemberFromSettlement(ServerClient client, Packet receivedPacket)
        {
            if (GetMemberRank(client) != MemberRank.Admin)
            {
                Packet NotEnoughPowerErrorPacket = new Packet("NotEnoughPowerErrorPacket");
                Network.SendData(client, NotEnoughPowerErrorPacket);
            }

            else
            {
                ServerClient toKick = SettlementHandler.GetPlayerFileFromSettlement(int.Parse(receivedPacket.contents[0]));

                RemoveMemberFromFaction(client, toKick.Username);
            }
        }

        public void BuildStructure(ServerClient client, Packet receivedPacket)
        {
            StructureFile newStructure = Serializer.SerializeToClass<StructureFile>(receivedPacket.contents[0]);

            if (newStructure.structureType == (int)StructureTypes.ProductionSite)
            {
                List<StructureFile> existentStructures = factionStructures.ToList();
                StructureFile existent = existentStructures.Find(fetch => fetch.structureType == (int)StructureTypes.ProductionSite);

                if (existent != null)
                {
                    ServerHandler.WriteToConsole($"[Tried to build an illegal structure] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
                    return;
                }
            }

            if (newStructure.structureType == (int)StructureTypes.Wonder && FactionHandler.CheckIfUniqueStructureExists())
            {
                ServerHandler.WriteToConsole($"[Tried to build an illegal structure] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
                return;
            }

            factionStructures.Add(newStructure);

            SaveFaction();

            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach (ServerClient sc in connectedClients)
            {
                newStructure.structureFaction = FactionHandler.GetRelationShipWithPlayer(sc, this);

                string[] contents = new string[] { Serializer.SoftSerialize(newStructure) };
                Packet SpawnNewStructurePacket = new Packet("SpawnNewStructurePacket", contents);
                Network.SendData(sc, SpawnNewStructurePacket);
            }

            ServerHandler.WriteToConsole($"[Built new faction structure] > [{client.Username}] - [{client.SavedID}]");
        }

        public void DestroyStructure(ServerClient client, Packet receivedPacket)
        {
            StructureFile[] structures = factionStructures.ToArray();
            StructureFile toDestroy = null;
            foreach(StructureFile file in structures)
            {
                if (file.structureTile == int.Parse(receivedPacket.contents[0]))
                {
                    toDestroy = file;
                }
            }

            if (toDestroy == null) return;
            else
            {
                factionStructures.Remove(toDestroy);

                SaveFaction();

                ServerClient[] connectedClients = Network.connectedClients.ToArray();
                foreach (ServerClient sc in connectedClients)
                {
                    string[] contents = new string[] { toDestroy.structureTile.ToString() };
                    Packet DestroyStructurePacket = new Packet("DestroyStructurePacket", contents);
                    Network.SendData(sc, DestroyStructurePacket);
                }

                ServerHandler.WriteToConsole($"[Destroyed faction structure] > [{client.Username}] - [{client.SavedID}]");
            }
        }

        public void RefreshBank()
        {
            string[] contents = new string[] { bankSilver.ToString() };
            Packet BankRefreshPacket = new Packet("BankRefreshPacket", contents);

            Tuple<string, MemberRank>[] members = factionMembers.ToArray();
            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach (Tuple<string, MemberRank> member in members)
            {
                foreach (ServerClient connectedClient in connectedClients)
                {
                    if (connectedClient.Username == member.Item1)
                    {
                        Network.SendData(connectedClient, BankRefreshPacket);
                    }
                }
            }
        }

        public void RefreshSilo()
        {
            List<string> contents = new List<string>();
            foreach(TradeItem item in siloItems)
            {
                contents.Add(Serializer.SoftSerialize(item));
            }

            Packet SiloRefreshPacket = new Packet("SiloRefreshPacket", contents.ToArray());

            Tuple<string, MemberRank>[] members = factionMembers.ToArray();
            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach (Tuple<string, MemberRank> member in members)
            {
                foreach (ServerClient connectedClient in connectedClients)
                {
                    if (connectedClient.Username == member.Item1)
                    {
                        Network.SendData(connectedClient, SiloRefreshPacket);
                    }
                }
            }
        }

        public void DepositSilverIntoBank(ServerClient client, string[] contents)
        {
            int silverToDeposit = 0;

            foreach (string str in contents)
            {
                TradeItem item = Serializer.SerializeToClass<TradeItem>(str);
                if (item.defName != "Silver")
                {
                    ServerHandler.WriteToConsole($"[Tried to add non-silver to silo] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
                    return;
                }
                else silverToDeposit += item.stackCount;
            }

            bankSilver += silverToDeposit;

            SaveFaction();

            RefreshBank();
        }

        public void WithdrawSilverFromBank(ServerClient client, Packet receivedPacket)
        {
            int silverToWithdraw = int.Parse(receivedPacket.contents[0]);

            if (silverToWithdraw > bankSilver)
            {
                ServerHandler.WriteToConsole($"[Tried to withdraw above max bank silver] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
            }

            else
            {
                bankSilver -= silverToWithdraw;

                string[] contents = new string[] { silverToWithdraw.ToString() };
                Packet ReceiveSilverFromBankPacket = new Packet("ReceiveSilverFromBankPacket", contents);
                Network.SendData(client, ReceiveSilverFromBankPacket);

                SaveFaction();

                RefreshBank();
            }
        }

        public void DepositItemsIntoSilo(string[] contents)
        {
            List<TradeItem> existentItems = siloItems.ToList();
            
            foreach(string str in contents)
            {
                TradeItem item = Serializer.SerializeToClass<TradeItem>(str);
                existentItems.Add(item);
            }

            siloItems = existentItems.ToList();

            SaveFaction();

            RefreshSilo();
        }

        public void WithdrawItemFromSilo(ServerClient client, Packet receivedPacket)
        {
            List<TradeItem> itemsInSilo = siloItems.ToList();
            TradeItem item = Serializer.SerializeToClass<TradeItem>(receivedPacket.contents[0]);
            TradeItem toGet = itemsInSilo.Find(fetch => 
            fetch.defName == item.defName && 
            fetch.stackCount == item.stackCount &&
            fetch.itemQuality == item.itemQuality);

            if (toGet != null)
            {
                siloItems.Remove(toGet);

                string[] contents = new string[] { Serializer.SoftSerialize(toGet) };
                Packet ReceiveItemFromSiloPacket = new Packet("ReceiveItemFromSiloPacket", contents);
                Network.SendData(client, ReceiveItemFromSiloPacket);

                SaveFaction();

                RefreshSilo();
            }

            else
            {
                ServerHandler.WriteToConsole($"[Tried to withdraw an unknown item] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
            }
        }

        public void SendProduction()
        {
            Packet FactionProductionProductsPacket = new Packet("ProductionProductsPacket");

            Tuple<string, MemberRank>[] members = factionMembers.ToArray();
            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach (Tuple<string, MemberRank> member in members)
            {
                foreach (ServerClient connectedClient in connectedClients)
                {
                    if (connectedClient.Username == member.Item1)
                    {
                        Network.SendData(connectedClient, FactionProductionProductsPacket);
                    }
                }
            }
        }
    }
}
