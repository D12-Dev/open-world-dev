using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OpenWorldReduxServer
{
    public static class AuthNetwork
    {
        public static TcpClient connection;
        public static Stream s;
        public static NetworkStream ns;
        public static StreamWriter sw;
        public static StreamReader sr;

        public static string serverIP = "109.123.250.81";
        public static int serverPort = 25554;

        public static bool isLoggedIntoAuthServer;

        public static void TryConnectToServer()
        {
            if (isLoggedIntoAuthServer)
            {
                DisconnectFromServer(true);

                Thread.Sleep(1000);
            }

            ServerHandler.WriteToConsole("Trying to connect to auth server", ServerHandler.LogMode.Warning);

            try
            {
                connection = new TcpClient(serverIP, serverPort);
                s = connection.GetStream();
                sw = new StreamWriter(s);
                sr = new StreamReader(s);
                ns = connection.GetStream();

                ListenToServer();
            }

            catch { DisconnectFromServer(); }
        }

        public static void ListenToServer()
        {
            string[] contents = new string[] { Server.serverAuth.Username, Server.serverAuth.Token };
            Packet LoginToVerificatorPacket = new Packet("LoginToVerificatorPacket", contents);
            SendData(LoginToVerificatorPacket);

            while (true)
            {
                string data = sr.ReadLine();

                if (data == null)
                {
                    DisconnectFromServer();
                    return;
                }

                else AuthPacketHandler.HandlePacket(data);
            }
        }

        public static void SendData(Packet toSend)
        {
            try
            {
                sw.WriteLine(Serializer.Serialize(toSend));
                sw.Flush();
            }

            catch { DisconnectFromServer(); }
        }

        public static void DisconnectFromServer(bool isReconnecting = false)
        {
            if (connection != null) connection.Dispose();

            isLoggedIntoAuthServer = false;

            if (!isReconnecting)
            {
                ServerHandler.WriteToConsole("Failed to connect to auth server, clients won't be able to join your server", ServerHandler.LogMode.Warning);
            }
        }
    }
}
