using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class SettlementHandler
    {
        public static void AddSettlement(ServerClient client, Packet receivedPacket)
        {
            if (CheckIfSettlementExists(int.Parse(receivedPacket.contents[0])))
            {
                ServerHandler.WriteToConsole($"[Tried to override existing settlement] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
            }

            else
            {
                SettlementFile settlementFile = new SettlementFile();
                settlementFile.settlementTile = int.Parse(receivedPacket.contents[0]);
                settlementFile.settlementUsername = client.Username;

                string savePath1 = Server.settlementsFolderPath + Path.DirectorySeparatorChar + client.SavedID + ".json";
                Serializer.SerializeToFile(settlementFile, savePath1);

                client.SettlementTile = receivedPacket.contents[0];
                ClientHandler.SaveClient(client);

                string[] contents = new string[] { Serializer.SoftSerialize(settlementFile) };
                Packet SpawnNewSettlementPacket = new Packet("SpawnNewSettlementPacket", contents);

                ServerClient[] connectedClients = Network.connectedClients.ToArray();
                foreach (ServerClient sc in connectedClients)
                {
                    if (sc == client) continue;
                    else Network.SendData(sc, SpawnNewSettlementPacket);
                }

                ServerHandler.WriteToConsole($"[Added settlement] > {receivedPacket.contents[0]}");
            }
        }

        public static void RemoveSettlement(ServerClient client, Packet receivedPacket)
        {
            if (CheckIfSettlementExists(int.Parse(receivedPacket.contents[0])))
            {
                if (client.SettlementTile == receivedPacket.contents[0])
                {
                    string fileLocation = Server.settlementsFolderPath + Path.DirectorySeparatorChar + client.SavedID + ".json";
                    File.Delete(fileLocation);

                    client.SettlementTile = null;
                    ClientHandler.SaveClient(client);

                    string[] contents = new string[] { receivedPacket.contents[0] };
                    Packet DestroySettlementPacket = new Packet("DestroySettlementPacket", contents);
                    ServerClient[] connectedClients = Network.connectedClients.ToArray();
                    foreach (ServerClient sc in connectedClients)
                    {
                        if (sc == client) continue;
                        else Network.SendData(sc, DestroySettlementPacket);
                    }

                    ServerHandler.WriteToConsole($"[Removed settlement] > {receivedPacket.contents[0]}");
                }

                else ServerHandler.WriteToConsole($"[Tried to delete other settlement] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
            }

            else ServerHandler.WriteToConsole($"[Tried to delete nonexistent settlement] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
        }

        public static bool CheckIfSettlementExists(int settlementTile)
        {
            string[] files = Directory.GetFiles(Server.settlementsFolderPath);
            foreach (string file in files)
            {
                SettlementFile toCheck = Serializer.DeserializeFromFile<SettlementFile>(file);
                if (toCheck.settlementTile == settlementTile) return true;
            }

            return false;
        }
        public static void SaveSettlementFile(SettlementFile settlementFile, string UserReference)
        {

            string savePath1 = Server.settlementsFolderPath + Path.DirectorySeparatorChar + UserReference + ".json";
            Serializer.SerializeToFile(settlementFile, savePath1);


        }
        public static ServerClient GetPlayerFromSettlement(int settlementTile)
        {
            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach (ServerClient sc in connectedClients)
            {
                if (sc.SettlementTile == settlementTile.ToString()) return sc;
            }

            return null;
        }

        public static ServerClient GetPlayerFileFromSettlement(int settlementTile)
        {
            string[] clientFiles = Directory.GetFiles(Server.playersFolderPath);
            foreach (string clientFile in clientFiles)
            {
                ServerClient clientToCheck = Serializer.DeserializeFromFile<ServerClient>(clientFile);
                if (clientToCheck.SettlementTile == settlementTile.ToString())
                {
                    return clientToCheck;
                }
            }

            return null;
        }

        public static void SendCurrentSettlements(ServerClient client)
        {
            List<string> contents = new List<string>();

            string[] files = Directory.GetFiles(Server.settlementsFolderPath);
            foreach (string file in files)
            {
                SettlementFile settlement = Serializer.DeserializeFromFile<SettlementFile>(file);
                if (settlement.settlementUsername == client.Username) continue;
                else
                {
                    settlement.settlementFaction = GetSettlementRelationshipWithPlayer(client, settlement.settlementUsername);
                    contents.Add(Serializer.SoftSerialize(settlement));
                }
            }

            Packet CurrentSettlementsInWorldPacket = new Packet("CurrentSettlementsInWorldPacket", contents.ToArray());
            Network.SendData(client, CurrentSettlementsInWorldPacket);
        }

        private static int GetSettlementRelationshipWithPlayer(ServerClient client, string otherClientUsername)
        {
            ServerClient otherClient = ClientHandler.GetClientFromSave(otherClientUsername);

            if (string.IsNullOrWhiteSpace(client.FactionName) || string.IsNullOrWhiteSpace(otherClient.FactionName)) return 0;
            else if (client.FactionName == otherClient.FactionName) return 1;
            else return 2;
        }
    }
}