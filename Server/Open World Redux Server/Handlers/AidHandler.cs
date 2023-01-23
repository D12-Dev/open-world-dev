using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class AidHandler
    {
        public static void SendAidToPlayer(ServerClient client, Packet receivedPacket)
        {
            ServerClient toGet = SettlementHandler.GetPlayerFromSettlement(int.Parse(receivedPacket.contents[0]));
            if (toGet == null)
            {
                Packet AidNotAvailablePacket = new Packet("AidNotAvailablePacket");
                Network.SendData(client, AidNotAvailablePacket);
            }

            else
            {
                Packet SentAidPacket = new Packet("SentAidPacket");
                Network.SendData(client, SentAidPacket);

                Packet ReceiveAidPacket = new Packet("ReceiveAidPacket");
                Network.SendData(toGet, ReceiveAidPacket);
            }
        }
    }
}
