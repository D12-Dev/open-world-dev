using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class AuthPacketHandler
    {
        public static void HandlePacket(string data)
        {
            Packet receivedPacket = Serializer.Deserialize(data);

            if (receivedPacket == null) AuthNetwork.DisconnectFromServer();
            else
            {
                Debug.WriteLine(receivedPacket.header);

                if (receivedPacket.header == "AcceptedCredentialsPacket")
                {
                    AcceptedPacketHandle();
                }
            }
        }

        public static void AcceptedPacketHandle()
        {
            ServerHandler.WriteToConsole("Logged into auth server", ServerHandler.LogMode.Warning);

            AuthNetwork.isLoggedIntoAuthServer = true;

            if (Network.hasServerStarted) return;
            else
            {
                ThreadHandler.GenerateServerThread(0);
                ThreadHandler.GenerateServerThread(1);
                ThreadHandler.GenerateServerThread(2);
                ServerHandler.UpdateTitle();
            }
        }
    }
}
