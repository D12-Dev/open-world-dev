using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldServerVerificator
{
    public static class ClientHandler
    {
        public static void RegisterClient(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ServerHandler.WriteToConsole($"[ERROR] > Parameters can't be empty", ServerHandler.LogMode.Warning);
                return;
            }

            if (CheckIfClientExists(username))
            {
                ServerHandler.WriteToConsole($"[ERROR] > Client [{username}] already existed", ServerHandler.LogMode.Warning);
                return;
            }

            ServerClient newClient = new ServerClient();
            newClient.Username = username;
            newClient.Token = HashHandler.GetTokenForNewClient();
            newClient.SavedID = GetNewClientID();

            SaveClient(newClient);

            ServerHandler.WriteToConsole($"New client registered with ID {newClient.SavedID} > {newClient.Username}", ServerHandler.LogMode.Title);
        }

        public static void LoginClient(ServerClient client, Packet receivedPacket)
        {
            ServerClient getFromCredentials = GetClientFromCredentials(receivedPacket);
            if (getFromCredentials == null)
            {
                Packet RejectedCredentialsPacket = new Packet("RejectedCredentialsPacket");
                Network.SendData(client, RejectedCredentialsPacket);

                client.disconnectFlag = true;
                ServerHandler.WriteToConsole($"[Wrong auth credentials] > {client.SavedIP}");
                return;
            }

            else
            {
                client.Username = getFromCredentials.Username;
                client.Token = getFromCredentials.Token;
                client.SavedID = getFromCredentials.SavedID;
                SaveClient(client);

                CheckForClientCopies(client);

                Packet AcceptedCredentialsPacket = new Packet("AcceptedCredentialsPacket");
                Network.SendData(client, AcceptedCredentialsPacket);

                ServerHandler.WriteToConsole($"[Logged in] > {client.SavedIP}");
            }
        }

        public static void RenewClient(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ServerHandler.WriteToConsole($"[ERROR] > Parameters can't be empty", ServerHandler.LogMode.Warning);
                return;
            }

            ServerClient toRenew = GetClientFromUsername(username);

            if (toRenew == null)
            {
                ServerHandler.WriteToConsole($"[ERROR] > Client {username} was not found", ServerHandler.LogMode.Warning);
                return;
            }

            else
            {
                toRenew.ToBeRenewed = true;
                SaveClient(toRenew);

                ServerHandler.WriteToConsole($"Client {toRenew.Username} was renewed", ServerHandler.LogMode.Title);
            }
        }

        public static void UnrenewClient(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ServerHandler.WriteToConsole($"[ERROR] > Parameters can't be empty", ServerHandler.LogMode.Warning);
                return;
            }

            ServerClient toUnrenew = GetClientFromUsername(username);

            if (toUnrenew == null)
            {
                ServerHandler.WriteToConsole($"[ERROR] > Client {username} was not found", ServerHandler.LogMode.Warning);
                return;
            }

            else
            {
                toUnrenew.ToBeRenewed = false;
                SaveClient(toUnrenew);

                ServerHandler.WriteToConsole($"Client {toUnrenew.Username} renewal was taken away", ServerHandler.LogMode.Title);
            }
        }

        public static void SaveClient(ServerClient client)
        {
            string savePath = Server.usersFolderPath + Path.DirectorySeparatorChar + client.SavedID + ".json";
            Serializer.SerializeToFile(client, savePath);
        }

        public static string GetNewClientID()
        {
            string[] clientFiles = Directory.GetFiles(Server.usersFolderPath);

            if (clientFiles.Length == 0) return 1.ToString();

            List<double> idList = new List<double>();

            foreach (string file in clientFiles)
            {
                idList.Add(Convert.ToInt64(Path.GetFileNameWithoutExtension(file)));
            }

            return (idList.Max() + 1).ToString();
        }

        public static ServerClient GetClientFromCredentials(Packet receivedPacket)
        {
            string[] clientFiles = Directory.GetFiles(Server.usersFolderPath);
            foreach (string file in clientFiles)
            {
                ServerClient existingClient = Serializer.DeserializeFromFile<ServerClient>(file);
                if (existingClient.Username == receivedPacket.contents[0] &&
                    existingClient.Token == receivedPacket.contents[1])
                {
                    return existingClient;
                }
            }

            return null;
        }

        public static ServerClient GetClientFromUsername(string username)
        {
            string[] clientFiles = Directory.GetFiles(Server.usersFolderPath);
            foreach (string file in clientFiles)
            {
                ServerClient existingClient = Serializer.DeserializeFromFile<ServerClient>(file);
                if (existingClient.Username == username)
                {
                    return existingClient;
                }
            }

            return null;
        }

        public static void CheckForClientCopies(ServerClient client)
        {
            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach(ServerClient connected in connectedClients)
            {
                if (connected == client) continue;

                if (connected.Username == client.Username)
                {
                    connected.disconnectFlag = true;
                }
            }
        }

        public static bool CheckIfClientExists(string username)
        {
            string[] clientFiles = Directory.GetFiles(Server.usersFolderPath);
            foreach(string file in clientFiles)
            {
                ServerClient existingClient = Serializer.DeserializeFromFile<ServerClient>(file);
                if (existingClient.Username == username) return true;
            }

            return false;
        }

        public static void CheckClientsRenewal()
        {
            while (true)
            {
                Thread.Sleep(3600000);

                try
                {
                    List<string> toCancel = new List<string>();
                    string[] clientFiles = Directory.GetFiles(Server.usersFolderPath);
                    foreach (string file in clientFiles)
                    {
                        ServerClient toCheck = Serializer.DeserializeFromFile<ServerClient>(file);
                        if (!toCheck.ToBeRenewed) toCancel.Add(file);
                    }

                    foreach (string file in toCancel)
                    {
                        File.Delete(file);

                        ServerClient[] connectedClients = Network.connectedClients.ToArray();
                        foreach (ServerClient connected in connectedClients)
                        {
                            if (connected.SavedID == Path.GetFileNameWithoutExtension(file))
                            {
                                connected.disconnectFlag = true;
                            }
                        }
                    }

                    ServerHandler.WriteToConsole("[Client renewal tick]", ServerHandler.LogMode.Title);
                }

                catch { ServerHandler.WriteToConsole("Critical error occured in renewal thread", ServerHandler.LogMode.Error); }
            }
        }
    }
}
