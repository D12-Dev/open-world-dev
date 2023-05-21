using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace OpenWorldRedux
{
    public static class FactionHandler
    {
        public static void UnpackFaction(Packet receivedPacket)
        {
            FactionFile factionFile = Serializer.SerializeToClass<FactionFile>(receivedPacket.contents[0]);

            FactionCache.hasFaction = true;
            FactionCache.factionName = factionFile.factionName;

            FactionCache.factionMembers.Clear();
            foreach(string str in factionFile.memberString)
            {
                Tuple<string, FactionCache.MemberRank> newMember = Tuple.Create(str.Split(',')[0], (FactionCache.MemberRank)int.Parse(str.Split(',')[1]));
                FactionCache.factionMembers.Add(newMember);
            }
        }

        public static void ResetFactionDetails()
        {
            FactionCache.hasFaction = false;
            FactionCache.factionName = "";
            FactionCache.factionMembers.Clear();
        }

        public static void ReceiveFactionInvitation(Packet receivedPacket)
        {
            Find.WindowStack.Add(new OW_MPFactionInvite(receivedPacket));
        }

        public static void CompleteOperation()
        {
            FocusCache.waitWindowInstance.Close();

            Injections.thingsToDoInUpdate.Add(MPGame.ForceSave);
        }

        public static void ReceiveBankSilver(Packet receivedPacket)
        {
            RimworldHandler.GetSilverToCaravan(int.Parse(receivedPacket.contents[0]));

            FocusCache.waitWindowInstance.Close();
        }

        public static void ReceiveSiloItem(Packet receivedPacket)
        {
            TradeItem item = Serializer.SerializeToClass<TradeItem>(receivedPacket.contents[0]);
            TradeItem[] items = new TradeItem[] { item };

            RimworldHandler.GetItemsToCaravan(items);

            FactionCache.canWithdrawFromSilo = true;
        }

        public static void ReceiveProductionSiteItems()
        {
            if (!BooleanCache.hasLoadedCorrectly) return;
            else
            {
                if (Find.AnyPlayerHomeMap == null) return;
                else
                {
                    TradeItem[] items = new TradeItem[] { FactionCache.GetProductionSiteItem() };
                    RimworldHandler.GetItemsToSettlement(items, false);

                    if (!BooleanCache.hideProductionSiteMessages)
                    {
                        LetterCache.GetLetterDetails("Production Site Materials Arrival",
                            "The materials of your production site have arrived!", LetterDefOf.PositiveEvent);

                        Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);
                    }
                }
            }
        }
    }
}
