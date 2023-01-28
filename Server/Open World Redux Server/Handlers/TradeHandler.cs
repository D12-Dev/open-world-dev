using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class TradeHandler
    {
        public static void HandleTrade(ServerClient client, Packet receivedPacket)
        {
            List<string> tempContents = receivedPacket.contents.ToList();
            int targetTile = int.Parse(tempContents[0]);
            int tradeType = int.Parse(tempContents[1]);
            tempContents.RemoveAt(0);

            if (!SettlementHandler.CheckIfSettlementExists(targetTile))
            {
                ServerHandler.WriteToConsole($"[Tried trade with unknown settlement] > [{client.Username}] - [{client.SavedID}]", ServerHandler.LogMode.Warning);
                return;
            }

            else
            {
                ServerClient clientToSendTo = SettlementHandler.GetPlayerFromSettlement(targetTile);
                if (clientToSendTo == null)
                {
                    Packet SendThingsBackPacket = new Packet("SendThingsBackPacket");
                    Network.SendData(client, SendThingsBackPacket);
                }

                else
                {
                    //Gift
                    if (tradeType == 0)
                    {
                        tempContents.Insert(0, client.SettlementTile);
                        Packet SendThingsPacket = new Packet("SendThingsPacket", tempContents.ToArray());
                        Network.SendData(clientToSendTo, SendThingsPacket);

                        Packet AcceptTradePacket = new Packet("AcceptTradePacket");
                        Network.SendData(client, AcceptTradePacket);

                        ServerHandler.WriteToConsole($"[Gift done] > [{client.Username} > {clientToSendTo.Username}]");
                    }

                    //Trade
                    else if (tradeType == 1)
                    {
                        tempContents.Insert(0, client.SettlementTile);
                        Packet SendThingsPacket = new Packet("SendThingsPacket", tempContents.ToArray());
                        Network.SendData(clientToSendTo, SendThingsPacket);
                    }

                    //Barter
                    else if (tradeType == 2)
                    {
                        tempContents.Insert(0, client.SettlementTile);
                        Packet SendThingsPacket = new Packet("SendThingsPacket", tempContents.ToArray());
                        Network.SendData(clientToSendTo, SendThingsPacket);
                    }

                    //Silo
                    else if (tradeType == 3)
                    {
                        tempContents.RemoveAt(0);
                        FactionHandler.DepositItemsIntoSilo(client, tempContents.ToArray());

                        Packet OperationCompletePacket = new Packet("OperationCompletePacket");
                        Network.SendData(client, OperationCompletePacket);
                    }

                    //Bank
                    else if (tradeType == 4)
                    {
                        tempContents.RemoveAt(0);
                        FactionHandler.DepositSilverToBank(client, tempContents.ToArray());

                        Packet OperationCompletePacket = new Packet("OperationCompletePacket");
                        Network.SendData(client, OperationCompletePacket);
                    }
                }
            }
        }

        public static void AcceptTrade(ServerClient client, Packet receivedPacket)
        {
            ServerClient clientToSendTo = SettlementHandler.GetPlayerFromSettlement(int.Parse(receivedPacket.contents[0]));
            if (clientToSendTo == null) return;
            else
            {
                Packet AcceptTradePacket = new Packet("AcceptTradePacket");
                Network.SendData(clientToSendTo, AcceptTradePacket);

                ServerHandler.WriteToConsole($"[Trade done] > [{clientToSendTo.Username} > {client.Username}]");
            }
        }

        public static void RejectTrade(Packet receivedPacket)
        {
            ServerClient clientToSendTo = SettlementHandler.GetPlayerFromSettlement(int.Parse(receivedPacket.contents[0]));
            if (clientToSendTo == null) return;
            else
            {
                Packet RejectTradePacket = new Packet("RejectTradePacket");
                Network.SendData(clientToSendTo, RejectTradePacket);
            }
        }
    }
}
