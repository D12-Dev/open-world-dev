using Open_World_Redux;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class ClientLoginHandler
    {
        public static void LoginClient(ServerClient client, Packet receivedPacket)
        {
            if (!CheckForClientInDatabase(client, receivedPacket))
            {
                Packet IncorrectLoginPacket = new Packet("IncorrectLoginPacket");
                Network.SendData(client, IncorrectLoginPacket);

                ServerHandler.WriteToConsole($"[Incorrect login] > {client.SavedIP}");
                client.disconnectFlag = true;
                return;
            }

            if (!CheckIfClientShouldConnect(client))
            {
                client.disconnectFlag = true;
                return;
            }

            ClientHandler.ReloadPlayerCount();

            ClientHandler.SendServerValues(client);

            ClientHandler.SendClientValues(client);

            if (Server.serverConfig.UseCustomDifficulty) ClientHandler.SendDifficultyValues(client);

            SettlementHandler.SendCurrentSettlements(client);

            FactionHandler.GetAllFactionStructures(client);

            FactionHandler.RefreshFactionDetails(client);

            if (!CheckForClientSave(client)) WorldHandler.HandleWorld(client);
            else ClientSaveHandler.SendSaveFile(client);

            ClientHandler.SaveClient(client);

            ServerHandler.WriteToConsole($"[Logged in] > [{client.Username}] - [{client.SavedID}]");
        }

        private static bool CheckIfClientShouldConnect(ServerClient client)
        {
            if (client.IsBanned)
            {
                Packet BannedPlayerPacket = new Packet("BannedPlayerPacket");
                Network.SendData(client, BannedPlayerPacket);
                return false;
            }

            string[] clientFiles = Directory.GetFiles(Server.playersFolderPath);
            foreach (string file in clientFiles)
            {
                ServerClient savedClient = Serializer.DeserializeFromFile<ServerClient>(file);
                if (savedClient.IsBanned && savedClient.SavedIP == client.SavedIP)
                {
                    Packet BannedPlayerPacket = new Packet("BannedPlayerPacket");
                    Network.SendData(client, BannedPlayerPacket);
                    return false;
                }
            }

            if (Server.serverConfig.UseWhitelist)
            {
                List<string> whitelistedUsernames = Server.whitelist.WhitelistedUsernames.ToList();
                string whitelisted = whitelistedUsernames.Find(fetch => fetch == client.Username);
                if (whitelisted == null)
                {
                    Packet NotWhitelistedPacket = new Packet("NotWhitelistedPacket");
                    Network.SendData(client, NotWhitelistedPacket);
                    return false;
                }
            }

            List<ServerClient> connectedClients = Network.connectedClients.ToList();
            ServerClient sameClient = connectedClients.Find(fetch => fetch.Username == client.Username && fetch != client);
            if (sameClient != null) sameClient.disconnectFlag = true;

            return true;
        }

        private static bool CheckForClientInDatabase(ServerClient client, Packet receivedPacket)
        {
            string[] clientFiles = Directory.GetFiles(Server.playersFolderPath);

            if (clientFiles.Length == 0) return false;

            string usernameToLogin = receivedPacket.contents[0];
            string passwordToLogin = receivedPacket.contents[1];
            foreach (string clientFile in clientFiles)
            {
                ServerClient clientToCheck = Serializer.DeserializeFromFile<ServerClient>(clientFile);
                if (clientToCheck.Username == usernameToLogin && clientToCheck.Password == passwordToLogin)
                {
                    ReadClient(client, clientToCheck);
                    return true;
                }
            }

            return false;
        }

        private static bool CheckForClientSave(ServerClient client)
        {
            string[] saveFiles = Directory.GetFiles(Server.savesFolderPath);

            foreach(string file in saveFiles)
            {
                if (Path.GetFileNameWithoutExtension(file) == client.SavedID) return true;
            }

            return false;
        }

        private static void ReadClient(ServerClient client, ServerClient clientToCheck)
        {
            client.Username = clientToCheck.Username;
            client.Password = clientToCheck.Password;
            client.SavedID = clientToCheck.SavedID;
            client.SettlementTile = clientToCheck.SettlementTile;
            client.FactionName = clientToCheck.FactionName;
            client.IsAdmin = clientToCheck.IsAdmin;
            client.IsBanned = clientToCheck.IsBanned;
            client.selectedProductionItem = clientToCheck.selectedProductionItem;
        }

        public static void CheckClientAuthFile(ServerClient client, ClientAuthFile authFile)
        {
            if (authFile.clientVersion != Server.serverConfig.PlayerVersion)
            {
                Packet WrongVersionPacket = new Packet("WrongVersionPacket");
                Network.SendData(client, WrongVersionPacket);
                client.disconnectFlag = true;

                ServerHandler.WriteToConsole($"[Wrong version at join] > {client.SavedIP}");
                return;
            }

            List<string> missingMods = new List<string>();
            if (Server.serverConfig.EnforceMods)
            {
                foreach (string str in Server.serverConfig.enforcedMods)
                {
                    if (!authFile.clientMods.Contains(str))
                    {
                        missingMods.Add(str);
                    }
                }

                if (missingMods.Count > 0)
                {
                    Packet MissingModsPacket = new Packet("MissingModsPacket", missingMods.ToArray());
                    Network.SendData(client, MissingModsPacket);
                    client.disconnectFlag = true;

                    ServerHandler.WriteToConsole($"[Missing mods at join] > {client.SavedIP}");
                    return;
                }

                List<string> extraMods = new List<string>();
                foreach (string str in authFile.clientMods)
                {
                    if (!Server.serverConfig.enforcedMods.Contains(str) &&
                        !Server.serverConfig.whitelistedMods.Contains(str))
                    {
                        extraMods.Add(str);
                    }
                }

                if (extraMods.Count > 0)
                {
                    Packet ExtraModsPacket = new Packet("ExtraModsPacket", extraMods.ToArray());
                    Network.SendData(client, ExtraModsPacket);
                    client.disconnectFlag = true;

                    ServerHandler.WriteToConsole($"[Extra mods at join] > {client.SavedIP}");
                    return;
                }
            }

            List<string> forbiddenMods = new List<string>();
            if (Server.serverConfig.UseModBlacklist)
            {
                foreach (string str in authFile.clientMods)
                {
                    if (Server.serverConfig.blacklistedMods.Contains(str))
                    {
                        forbiddenMods.Add(str);
                    }
                }
            }

            if (forbiddenMods.Count > 0)
            {
                Packet ForbiddenModsPacket = new Packet("ForbiddenModsPacket", forbiddenMods.ToArray());
                Network.SendData(client, ForbiddenModsPacket);
                client.disconnectFlag = true;

                ServerHandler.WriteToConsole($"[Forbidden mods at join] > {client.SavedIP}");
                return;
            }

            Packet AcceptedConnectionPacket = new Packet("AcceptedConnectionPacket");
            Network.SendData(client, AcceptedConnectionPacket);
        }
    }
}
