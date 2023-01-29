using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OpenWorldReduxServer
{
    public static class BlackMarketHandler
    {
        public static void HandleEvent(ServerClient client, Packet receivedPacket)
        {
            List<string> tempContents = receivedPacket.contents.ToList();
            int targetTile = int.Parse(tempContents[0]);
            tempContents.RemoveAt(0);

            if (!SettlementHandler.CheckIfSettlementExists(targetTile))
            {
                ServerHandler.WriteToConsole($"[Tried black market unknown settlement] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
                return;
            }

            else
            {
                ServerClient clientToSendTo = SettlementHandler.GetPlayerFromSettlement(targetTile);
                if (clientToSendTo == null || clientToSendTo.isEventProtected)
                {
                    Packet SendBlackMarketEventBackPacket = new Packet("SendBlackMarketEventBackPacket");
                    Network.SendData(client, SendBlackMarketEventBackPacket);
                    return;
                }

                else
                {
                    Packet SendBlackMarketEventPacket = new Packet("SendBlackMarketEventPacket", tempContents.ToArray());
                    Network.SendData(clientToSendTo, SendBlackMarketEventPacket);

                    Packet AcceptBlackMarketPacket = new Packet("AcceptBlackMarketPacket");
                    Network.SendData(client, AcceptBlackMarketPacket);

                    clientToSendTo.isEventProtected = true;

                    ServerHandler.WriteToConsole($"[Black market event done] > [{client.Username} > {clientToSendTo.Username}]");
                }
            }
        }
    }
}
