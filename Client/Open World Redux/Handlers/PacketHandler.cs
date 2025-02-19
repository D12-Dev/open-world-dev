﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class PacketHandler
    {
        public static void HandlePacket(string data)
        {
            Packet receivedPacket = Serializer.Deserialize(data);

            //Log.Message(receivedPacket.header);

            if (receivedPacket.header == "AcceptedConnectionPacket")
            {
                GeneralHandler.AcceptedConnectionHandle();
            }

            else if (receivedPacket.header == "AlreadyRegisteredPacket")
            {
                ErrorHandler.AlreadyRegisteredHandle();
            }

            else if (receivedPacket.header == "IncorrectLoginPacket")
            {
                ErrorHandler.IncorrectLoginHandle();
            }

            else if (receivedPacket.header == "RegisteredClientPacket")
            {
                GeneralHandler.RegisteredClientHandle();
            }

            else if (receivedPacket.header == "CreateNewWorldPacket")
            {
                WorldHandler.CreateNewWorldHandle();
            }

            else if (receivedPacket.header == "CreateWorldFromPacketPacket")
            {
                WorldHandler.CreateWorldFromPacketHandle(receivedPacket);
            }

            else if (receivedPacket.header == "ServerSaveFilePacket")
            {
                SaveHandler.LoadFromServerSave(receivedPacket);
            }

            else if (receivedPacket.header == "SpawnNewSettlementPacket")
            {
                WorldHandler.AddSettlementToWorld(receivedPacket);
            }

            else if (receivedPacket.header == "DestroySettlementPacket")
            {
                WorldHandler.RemoveSettlementFromWorld(receivedPacket);
            }

            else if (receivedPacket.header == "CurrentSettlementsInWorldPacket")
            {
                WorldHandler.ParseCurrentWorldSettlements(receivedPacket);
            }

            else if (receivedPacket.header == "CurrentStructuresInWorldPacket")
            {
                WorldHandler.ParseCurrentWorldStructures(receivedPacket);
            }

            else if (receivedPacket.header == "SendThingsPacket")
            {
                TradeHandler.GetTradeRequest(receivedPacket);
            }

            else if (receivedPacket.header == "SendThingsBackPacket")
            {
                TradeHandler.GetItemsBackIntoCaravan();
                Find.WindowStack.Add(new OW_ErrorDialog("Player is not available for this action"));
            }

            else if (receivedPacket.header == "AcceptTradePacket")
            {
                TradeHandler.GetAcceptedTrade();
            }

            else if (receivedPacket.header == "RejectTradePacket")
            {
                TradeHandler.GetRejectedTrade();
            }

            else if (receivedPacket.header == "ServerValuesPacket")
            {
                GeneralHandler.ServerValuesHandle(receivedPacket);
            }

            else if (receivedPacket.header == "ClientValuesPacket")
            {
                GeneralHandler.ClientValuesHandle(receivedPacket);
            }

            else if (receivedPacket.header == "AcceptBlackMarketPacket")
            {
                BlackMarketHandler.GetAcceptedEvent();
            }

            else if (receivedPacket.header == "SendBlackMarketEventBackPacket")
            {
                BlackMarketHandler.GetRejectedEvent();
            }

            else if (receivedPacket.header == "SendBlackMarketEventPacket")
            {
                BlackMarketHandler.GetEvent(receivedPacket);
            }

            else if (receivedPacket.header == "ServerPlayersPacket")
            {
                GeneralHandler.ServerPlayersHandle(receivedPacket);
            }

            else if (receivedPacket.header == "GetFactionDetails")
            {
                FactionHandler.UnpackFaction(receivedPacket);
            }

            else if (receivedPacket.header == "NoFactionPacket")
            {
                FactionHandler.ResetFactionDetails();
            }

            else if (receivedPacket.header == "FactionAlreadyExistsPacket")
            {
                ErrorHandler.FactionAlreadyExistsHandle();
            }

            else if (receivedPacket.header == "FactionInvitationPacket")
            {
                FactionHandler.ReceiveFactionInvitation(receivedPacket);
            }

            else if (receivedPacket.header == "InAnotherFactionErrorPacket")
            {
                ErrorHandler.InAnotherFactionErrorHandle();
            }

            else if (receivedPacket.header == "NotInFactionErrorPacket")
            {
                ErrorHandler.NotInFactionErrorHandle();
            }

            else if (receivedPacket.header == "NotEnoughPowerErrorPacket")
            {
                ErrorHandler.NotEnoughPowerErrorHandle();
            }

            else if (receivedPacket.header == "SpawnNewStructurePacket")
            {
                WorldHandler.AddStructureToWorld(receivedPacket);
            }

            else if (receivedPacket.header == "DestroyStructurePacket")
            {
                WorldHandler.RemoveStructureFromWorld(receivedPacket);
            }

            else if (receivedPacket.header == "PlayerNotAvailablePacket")
            {
                ErrorHandler.PlayerNotAvailableHandle();
            }

            else if (receivedPacket.header == "SiloRefreshPacket")
            {
                FactionCache.GetSiloItems(receivedPacket);
            }

            else if (receivedPacket.header == "BankRefreshPacket")
            {
                FactionCache.GetBankSilver(receivedPacket);
            }

            else if (receivedPacket.header == "OperationCompletePacket")
            {
                FactionHandler.CompleteOperation();
            }

            else if (receivedPacket.header == "ReceiveSilverFromBankPacket")
            {
                FactionHandler.ReceiveBankSilver(receivedPacket);
            }

            else if (receivedPacket.header == "ReceiveItemFromSiloPacket")
            {
                FactionHandler.ReceiveSiloItem(receivedPacket);
            }

            else if (receivedPacket.header == "ProductionProductsPacket")
            {
                FactionHandler.ReceiveProductionSiteItems();
            }

            else if (receivedPacket.header == "OpCommandPacket")
            {
                GeneralHandler.ToggleOp(true);
            }

            else if (receivedPacket.header == "DeopCommandPacket")
            {
                GeneralHandler.ToggleOp(false);
            }

            else if (receivedPacket.header == "BannedPlayerPacket")
            {
                ErrorHandler.BannedPlayerHandler();
            }

            else if (receivedPacket.header == "WrongVersionPacket")
            {
                ErrorHandler.WrongVersionHandle();
            }

            else if (receivedPacket.header == "MissingModsPacket")
            {
                ErrorHandler.MissingModsHandle(receivedPacket);
            }

            else if (receivedPacket.header == "ExtraModsPacket")
            {
                ErrorHandler.ExtraModsHandle(receivedPacket);
            }

            else if (receivedPacket.header == "ForbiddenModsPacket")
            {
                ErrorHandler.ForbiddenModsHandle(receivedPacket);
            }

            else if (receivedPacket.header == "AnnouncementMessagePacket")
            {
                GeneralHandler.GetAnnouncement(receivedPacket);
            }

            else if (receivedPacket.header == "SentAidPacket")
            {
                AidHandler.SendAidHandle();
            }

            else if (receivedPacket.header == "AidNotAvailablePacket")
            {
                AidHandler.AidNotAvailableHandle();
            }

            else if (receivedPacket.header == "ReceiveAidPacket")
            {
                AidHandler.ReceiveAidHandle();
            }

            else if (receivedPacket.header == "ServerDifficultyPacket")
            {
                GeneralHandler.ServerDifficultyHandle(receivedPacket);
            }

            else if (receivedPacket.header == "ServerFullPacket")
            {
                ErrorHandler.ServerFullHandle();
            }

            else if (receivedPacket.header == "NotWhitelistedPacket")
            {
                ErrorHandler.NotWhitelistedHandle();
            }
        }
    }
}
