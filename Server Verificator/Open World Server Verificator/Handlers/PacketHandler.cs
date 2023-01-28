using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldServerVerificator
{
    public static class PacketHandler
    {
        public static void HandlePacket(ServerClient client, string data)
        {
            Packet receivedPacket = Serializer.Deserialize(data);

            if (receivedPacket == null) client.disconnectFlag = true;
            else
            {
                Debug.WriteLine(receivedPacket.header);

                if (receivedPacket.header == "LoginToVerificatorPacket")
                {
                    ClientHandler.LoginClient(client, receivedPacket);
                }
            }
        }
    }
}