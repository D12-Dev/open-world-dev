using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace OpenWorldReduxServer
{
    public static class Network
    {
        private static TcpListener server;
        public static IPAddress localAddress;
        public static int serverPort;
        public static int maxPlayers;
        //public static List<String> PlayersToSaveList = SimpleCommands.PlayersToSaveList;
        public static bool hasServerStarted;

        public static List<ServerClient> connectedClients = new List<ServerClient>();

        public static void ReadyServer()
        {
            server = new TcpListener(localAddress, serverPort);
            server.Start();

            hasServerStarted = true;

            ServerHandler.WriteToConsole("Server Launched", ServerHandler.LogMode.Title);

            while (true) ListenForIncomingUsers();
        }

        private static void ListenForIncomingUsers()
        {
            ServerClient newServerClient = new ServerClient(server.AcceptTcpClient());

            if (!AuthNetwork.isLoggedIntoAuthServer)
            {
                ServerHandler.WriteToConsole($"[Tried to join but server is not authed] > {newServerClient.SavedIP}", ServerHandler.LogMode.Warning);
                newServerClient.disconnectFlag = true;
                return;
            }

            if (connectedClients.Count >= maxPlayers)
            {
                Packet ServerFullPacket = new Packet("ServerFullPacket");
                SendData(newServerClient, ServerFullPacket);
                newServerClient.disconnectFlag = true;

                ServerHandler.WriteToConsole($"[Tried to join but server is full] > {newServerClient.SavedIP}", ServerHandler.LogMode.Warning);
            }

            else
            {
                connectedClients.Add(newServerClient);

                ServerHandler.UpdateTitle();

                ServerHandler.WriteToConsole($"[Connected] > {newServerClient.SavedIP}", ServerHandler.LogMode.Normal);

                ThreadHandler.GenerateClientThread(newServerClient);
            }
        }

        public static void ListenToClient(ServerClient client)
        {
            NetworkStream s = client.tcp.GetStream();
            StreamReader sr = new StreamReader(s, true);

            while (true)
            {
                try
                {
                    if (client.disconnectFlag) return;

                    string data = sr.ReadLine();
                  //  ServerHandler.WriteToConsole($"[Debuggging] DATA: {data}", ServerHandler.LogMode.Normal); // For Debuging purposes only
                    if (data == null)
                    {
                        // Normally occurs during alt f4 or normal save and exit
                        //ServerHandler.WriteToConsole($"An error occurred when handling client data (usually alt + f4). Data given was a null value... Kicking!", ServerHandler.LogMode.Warning); 
                        client.disconnectFlag = true;
                        return;
                    }

                    else PacketHandler.HandlePacket(client, data);
                }

                catch (Exception ex)
                {
                    ServerHandler.WriteToConsole($"An error occurred when handling client packet... Full Stack Trace: \n{ex}", ServerHandler.LogMode.Error); // Client dropped the tcp connection. AKA no more client so no save since client wont respond.
                    client.disconnectFlag = true;
                    return;
                }
            }
        }

        public static void SendData(ServerClient client, Packet packet)
        {
            try
            {
                if (client.isBusy) {

                    ServerHandler.WriteToConsole($"Failed to send data to {client.Username}. Client was busy", ServerHandler.LogMode.Warning);
                    return;
                } 


                client.isBusy = true;

                NetworkStream s = client.tcp.GetStream();
                StreamWriter sw = new StreamWriter(s);

                sw.WriteLine(Serializer.Serialize(packet));
                sw.Flush();

                client.isBusy = false;
            }
            catch(Exception ex) {
                ServerHandler.WriteToConsole($"Failed to send data to {client.Username}. Kicking...\nFull stack Trace: \n{ex}", ServerHandler.LogMode.Error);
                client.disconnectsaveFlag = true; 
            }
        }
        public static void SendDataToAllConnectedClients(Packet reqPacket) {

            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach (ServerClient client in connectedClients)
            {
                Network.SendData(client, reqPacket);
            }


        }


        public static void SaveBeforeKick(ServerClient client)
        {
            
            int TimeoutCount = 15; // Timeout for how long server will wait for player saves
            int DefCount = 0;
            while (true)
            {

                Thread.Sleep(1000); // Wait 1 second then update
              //  ServerHandler.WriteToConsole($"{SimpleCommands.PlayersToSaveList[0]}", ServerHandler.LogMode.Normal);
                if (!SimpleCommands.PlayersToSaveList.Contains(client.Username))
                {
                    break;
                }
                else
                {

                    if (DefCount == TimeoutCount)
                    {
                        ServerHandler.WriteToConsole("User " + client.Username + " timedout when waiting for save before kicking...", ServerHandler.LogMode.Warning);
                        break;
                    }
                    DefCount++;
                    continue;
                }

            }
            client.disconnectsaveFlag = false;
            client.disconnectFlag = true;
            
            //// Player Has saved and been given a disconnect flag.

        }

        public static void KickClient(ServerClient client, bool SaveBeforeKickvar)
        {


            /////// Probably Should add a save request before disconnecting them.
            if (SaveBeforeKickvar == true)
        {
                SimpleCommands.PlayersToSaveList.Add(client.Username);
                ServerHandler.WriteToConsole("Requestting user " + client.Username + " for save before kicking...", ServerHandler.LogMode.Title);
                Packet ForceTheClientSyncPacket = new Packet("ForceClientSyncPacket");
                SendData(client, ForceTheClientSyncPacket);
                SaveBeforeKick(client);
            }
            else {
            connectedClients.Remove(client);

            client.tcp.Dispose();

            ServerHandler.WriteToConsole($"[Disconnected] > {client.SavedIP}", ServerHandler.LogMode.Normal);

            }

        }

        public static void HearbeatClients()
        {
            while (true)
            {
                Thread.Sleep(100); // Every 1/10th of a second run this code.

                try
                {
                    ServerClient[] actualClients = connectedClients.ToArray();

                    List<ServerClient> clientsToDisconnect = new List<ServerClient>(); // Players to disconnect list without saving.
                    List<ServerClient> clientsToSaveAndDisconnect = new List<ServerClient>();  // Players to disconnect list with saving.
                    foreach (ServerClient client in actualClients) ///////// Loop through clients and find which clients to just disconnect.
                    {
                        if (client.disconnectFlag) clientsToDisconnect.Add(client); // Add player to list.
                    }

                    foreach (ServerClient client in actualClients) ///////// Loop through clients and find which clients to save and disconnect.
                    {
                        if (client.disconnectsaveFlag) clientsToSaveAndDisconnect.Add(client); // Add player to list.
                    }

                    foreach (ServerClient client in clientsToDisconnect)
                    {
                        KickClient(client, false); // Kick the client, false means no saving before kick.
                    }
                    foreach (ServerClient client in clientsToSaveAndDisconnect)
                    {
                        KickClient(client, true); // Kick the client, true means it will try to save before kick.
                    }

                    if (clientsToDisconnect.Count > 0)
                    {
                        ServerHandler.UpdateTitle(); // Update Server Console Title to the correct player count.

                        ClientHandler.ReloadPlayerCount(); /// Update the player count.
                    }
                }

                catch(Exception ex) { ServerHandler.WriteToConsole($"Critical error occured in heartbeat thread... Full stack error: \n{ex}", ServerHandler.LogMode.Error); } // MY HEART IS DYINGGGG.
            }
        }
    }
}
