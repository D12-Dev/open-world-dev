using OpenWorldReduxServer.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
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

                if (receivedPacket.header == "ClientAuthPacket")
                {
                    GeneralPacketHandler.ClientAuthHandle(client, receivedPacket);
                }

                else if (receivedPacket.header == "RegisterClientPacket")
                {
                    ClientRegisterHandler.RegisterClient(client, receivedPacket);
                }

                else if (receivedPacket.header == "LoginClientPacket")
                {
                    ClientLoginHandler.LoginClient(client, receivedPacket);
                }


                else if (receivedPacket.header == "RecieveBaseSaveFromClient") ///////// Receives base save game from client
                {
                    SaveBaseGameHandler.SaveBaseGame(client, receivedPacket);
                }

                else if (receivedPacket.header == "ReceiveBaseSaveRequest") ///////// Receives base save request from client
                {
                    ClientSaveHandler.SendWorldGenSave(client);
                }

                else if (receivedPacket.header == "ForceClientSyncPacketReturn") ///////// Receives forced sync save from client from shutdown command
                {
                    SimpleCommands.ReturnedForceSync(client, receivedPacket);
                }


                else if (receivedPacket.header == "NewServerDataPacket")
                {
                    WorldHandler.SaveNewServerData(client, receivedPacket);
                }

                else if (receivedPacket.header == "ClientSaveFilePacket")
                {
                    ClientSaveHandler.SaveClientSave(client, receivedPacket);
                }

                else if (receivedPacket.header == "AddNewSettlementPacket")
                {
                    SettlementHandler.AddSettlement(client, receivedPacket);
                }

                else if (receivedPacket.header == "RemoveSettlementPacket")
                {
                    SettlementHandler.RemoveSettlement(client, receivedPacket);
                }

                else if (receivedPacket.header == "SendThingsPacket")
                {
                    TradeHandler.HandleTrade(client, receivedPacket);
                }

                else if (receivedPacket.header == "AcceptTradePacket")
                {
                    TradeHandler.AcceptTrade(client, receivedPacket);
                }

                else if (receivedPacket.header == "RejectTradePacket")
                {
                    TradeHandler.RejectTrade(receivedPacket);
                }

                else if (receivedPacket.header == "SendBlackMarketPacket")
                {
                    BlackMarketHandler.HandleEvent(client, receivedPacket);
                }

                else if (receivedPacket.header == "CreateFactionPacket")
                {
                    FactionHandler.CreateFaction(client, receivedPacket);
                }

                else if (receivedPacket.header == "RefreshFactionPacket")
                {
                    FactionHandler.RefreshFactionDetails(client);
                }

                else if (receivedPacket.header == "LeaveFactionPacket")
                {
                    FactionHandler.LeaveFaction(client);
                }

                else if (receivedPacket.header == "DisbandFactionPacket")
                {
                    FactionHandler.DisbandFaction(client);
                }

                else if (receivedPacket.header == "PromoteFactionMemberPacket")
                {
                    FactionHandler.PromoteMember(client, receivedPacket);
                }

                else if (receivedPacket.header == "DemoteFactionMemberPacket")
                {
                    FactionHandler.DemoteMember(client, receivedPacket);
                }

                else if (receivedPacket.header == "AddFactionMemberPacket")
                {
                    FactionHandler.AddMember(client, receivedPacket);
                }

                else if (receivedPacket.header == "RemoveFactionMemberPacket")
                {
                    FactionHandler.RemoveMember(client, receivedPacket);
                }

                else if (receivedPacket.header == "AcceptFactionInvitationPacket")
                {
                    FactionHandler.AcceptInvitation(client, receivedPacket);
                }

                else if (receivedPacket.header == "BuildFactionStructure")
                {
                    FactionHandler.BuildStructure(client, receivedPacket);
                }

                else if (receivedPacket.header == "DestroyFactionStructure")
                {
                    FactionHandler.DestroyStructure(client, receivedPacket);
                }

                else if (receivedPacket.header == "WithdrawFromFactionSiloPacket")
                {
                    FactionHandler.WithdrawFromFactionSilo(client, receivedPacket);
                }

                else if (receivedPacket.header == "RefreshFactionSiloPacket")
                {
                    FactionHandler.RefreshSilo(client);
                }

                else if (receivedPacket.header == "RefreshFactionBankPacket")
                {
                    FactionHandler.RefreshBank(client);
                }

                else if (receivedPacket.header == "FactionBankWithdrawPacket")
                {
                    FactionHandler.WithdrawFromFactionBank(client, receivedPacket);
                }

                else if (receivedPacket.header == "SendAidPacket")
                {
                    AidHandler.SendAidToPlayer(client, receivedPacket);
                }

                else if (receivedPacket.header == "ResetPlayerDataPacket")
                {
                    GeneralPacketHandler.DeleteAllPlayerData(client);
                }

                else if (receivedPacket.header == "ChangeProductionItemPacket")
                {
                    FactionHandler.ChangeProductionSiteItem(client, receivedPacket);
                }
            }
        }
    }
}