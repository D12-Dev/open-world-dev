using Microsoft.VisualBasic;
using Open_World_Redux;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class ClientHandler
    {
        public static void ReloadPlayerCount()
        {
            ServerClient[] connectedClients = Network.connectedClients.ToArray();

            foreach (ServerClient sc in connectedClients)
            {
                SendServerPlayers(sc);
            }
        }

        public static void SendServerPlayers(ServerClient client)
        {
            ServerClient[] connectedClients = Network.connectedClients.ToArray();

            List<string> contents = new List<string>();
            contents.Add((connectedClients.Length).ToString());

            foreach (ServerClient sc in connectedClients)
            {
                if (string.IsNullOrWhiteSpace(sc.Username)) continue;
                else contents.Add(sc.Username);
            }

            Packet ServerValuesPacket = new Packet("ServerPlayersPacket", contents.ToArray());
            Network.SendData(client, ServerValuesPacket);
        }

        public static void SendServerValues(ServerClient client)
        {
            string[] contents = new string[] { Serializer.SoftSerialize(Server.serverValues) };
            Packet ServerValuesPacket = new Packet("ServerValuesPacket", contents);
            Network.SendData(client, ServerValuesPacket);
        }

        public static void SendClientValues(ServerClient client)
        {
            ClientValuesFile newClientValues = new ClientValuesFile();
            newClientValues.isAdmin = client.IsAdmin;
            newClientValues.isCustomScenariosAllowed = Server.serverConfig.AllowCustomScenarios;
            newClientValues.selectedProductionItem = client.selectedProductionItem;

            string[] contents = new string[] { Serializer.SoftSerialize(newClientValues) };
            Packet ClientValuesPacket = new Packet("ClientValuesPacket", contents);
            Network.SendData(client, ClientValuesPacket);
        }

        public static void SendDifficultyValues(ServerClient client)
        {
            string[] contents = new string[] { Serializer.SoftSerialize(Server.serverDifficulty) };
            Packet ServerDifficultyPacket = new Packet("ServerDifficultyPacket", contents);
            Network.SendData(client, ServerDifficultyPacket);
        }

        public static void SaveClient(ServerClient client)
        {
            string savePath = Server.playersFolderPath + Path.DirectorySeparatorChar + client.SavedID + ".json";
            Serializer.SerializeToFile(client, savePath);
        }

        public static ServerClient GetClientFromSave(string usernameToCheck)
        {
            string[] clientFiles = Directory.GetFiles(Server.playersFolderPath);
            foreach (string clientFile in clientFiles)
            {
                ServerClient clientToCheck = Serializer.DeserializeFromFile<ServerClient>(clientFile);
                if (clientToCheck.Username == usernameToCheck)
                {
                    return clientToCheck;
                }
            }

            return null;
        }
        public static SettlementFile GetClientSettlmentFileFromName(string usernameToCheck)
        {
            string[] clientFiles = Directory.GetFiles(Server.settlementsFolderPath);
            foreach (string SettlmentFile in clientFiles)
            {
                SettlementFile SettlementToCheck = Serializer.DeserializeFromFile<SettlementFile>(SettlmentFile);
                if (SettlementToCheck.settlementUsername == usernameToCheck)
                {
                    return SettlementToCheck;
                }
            }

            return null;
        }
        public static void SwapNamesOfClientSettlementFile(string userReference, string NewReference)
        {
            string settlementFilePath = Server.settlementsFolderPath + Path.DirectorySeparatorChar + userReference + ".json";
            string newsettlementFilePath = Server.settlementsFolderPath + Path.DirectorySeparatorChar + NewReference + ".json";
            string tempsettlementFilePath = Server.settlementsFolderPath + Path.DirectorySeparatorChar + "temp" + ".json";
            FileInfo fi = new FileInfo(settlementFilePath);
            FileInfo fi2 = new FileInfo(newsettlementFilePath);

            if (fi.Exists) {
                fi.MoveTo(tempsettlementFilePath);
                fi2.MoveTo(settlementFilePath);
                fi.MoveTo(newsettlementFilePath);
            }
        }
        public static ServerClient GetClientFromConnected(string usernameToCheck)
        {
            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach(ServerClient sc in connectedClients)
            {
                if (sc.Username == usernameToCheck) return sc;
            }

            return null;
        }
    }
}
