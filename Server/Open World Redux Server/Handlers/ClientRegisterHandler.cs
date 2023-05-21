using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class ClientRegisterHandler
    {
        public static void RegisterClient(ServerClient client, Packet receivedPacket)
        {
            if (!CheckForClientInDatabase(receivedPacket))
            {
                Packet AlreadyRegisteredPacket = new Packet("AlreadyRegisteredPacket");
                Network.SendData(client, AlreadyRegisteredPacket);

                ServerHandler.WriteToConsole($"[Already registered] > {client.SavedIP}");
                client.disconnectFlag = true;
                return;
            }

            WriteNewClient(client, receivedPacket);
        }

        private static bool CheckForClientInDatabase(Packet receivedPacket)
        {
            string[] clientFiles = Directory.GetFiles(Server.playersFolderPath);

            if (clientFiles.Length == 0) return true;

            string usernameToRegister = (string)receivedPacket.contents[0];
            foreach (string clientFile in clientFiles)
            {
                ServerClient clientToCheck = Serializer.DeserializeFromFile<ServerClient>(clientFile);
                if (clientToCheck.Username == usernameToRegister) return false;
            }

            return true;
        }

        private static void WriteNewClient(ServerClient client, Packet receivedPacket)
        {
            ServerClient newClientFile = new ServerClient(client.tcp);
            newClientFile.Username = receivedPacket.contents[0];
            newClientFile.Password = receivedPacket.contents[1];
            newClientFile.SavedID = GetFilePathForNewUsername();

            ClientHandler.SaveClient(newClientFile);

            Packet RegisteredClientPacket = new Packet("RegisteredClientPacket");
            Network.SendData(client, RegisteredClientPacket);

            ServerHandler.WriteToConsole($"[Registered] > {client.SavedIP}");
            // Log in client
            ClientLoginHandler.LoginClient(client, receivedPacket);


        }

        private static string GetFilePathForNewUsername()
        {
            string[] clientFiles = Directory.GetFiles(Server.playersFolderPath);

            if (clientFiles.Length == 0) return 1.ToString();

            List<double> idList = new List<double>();

            foreach (string file in clientFiles)
            {
                idList.Add(Convert.ToInt64(Path.GetFileNameWithoutExtension(file)));
            }

            return (idList.Max() + 1).ToString();
        }
    }
}