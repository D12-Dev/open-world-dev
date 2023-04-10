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

                    if (data == null)
                    {
                        client.disconnectFlag = true;
                        return;
                    }

                    else PacketHandler.HandlePacket(client, data);
                }

                catch
                {
                    client.disconnectFlag = true;
                    return;
                }
            }
        }

        public static void SendData(ServerClient client, Packet packet)
        {
            try
            {
                if (client.isBusy) return;

                client.isBusy = true;

                NetworkStream s = client.tcp.GetStream();
                StreamWriter sw = new StreamWriter(s);

                sw.WriteLine(Serializer.Serialize(packet));
                sw.Flush();

                client.isBusy = false;
            }
            catch { client.disconnectFlag = true; }
        }

        public static void KickClient(ServerClient client)
        {


            /////// Probably Should add a save request before disconnecting them.
            connectedClients.Remove(client);

            client.tcp.Dispose();

            ServerHandler.WriteToConsole($"[Disconnected] > {client.SavedIP}", ServerHandler.LogMode.Normal);
        }

        public static void HearbeatClients()
        {
            while (true)
            {
                Thread.Sleep(100);

                try
                {
                    ServerClient[] actualClients = connectedClients.ToArray();

                    List<ServerClient> clientsToDisconnect = new List<ServerClient>();

                    foreach (ServerClient client in actualClients)
                    {
                        if (client.disconnectFlag) clientsToDisconnect.Add(client);
                    }

                    foreach (ServerClient client in clientsToDisconnect)
                    {
                        KickClient(client);
                    }

                    if (clientsToDisconnect.Count > 0)
                    {
                        ServerHandler.UpdateTitle();

                        ClientHandler.ReloadPlayerCount();
                    }
                }

                catch { ServerHandler.WriteToConsole("Critical error occured in heartbeat thread", ServerHandler.LogMode.Error); }
            }
        }
    }
}
