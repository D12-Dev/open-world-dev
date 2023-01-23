using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace OpenWorldRedux
{
    public static class TradeHandler
    {
        public enum TakeMode { Gift, Trade, Barter, Silo, Bank }

        public static void TakeItemsForTrade(TakeMode mode)
        {
            TradeCache.takeMode = mode;

            if (mode == TakeMode.Gift)
            {
                Action action = delegate
                {
                    if (TradeSession.deal.TryExecute(out bool actuallyTraded))
                    {
                        SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                        TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
                        FocusCache.giftMenuInstance.Close();

                        Find.WindowStack.Add(new OW_WaitingDialog());
                    }
                };

                action();
            }

            else if (mode == TakeMode.Trade)
            {
                Action action = delegate
                {
                    if (TradeSession.deal.TryExecute(out bool actuallyTraded))
                    {
                        SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                        TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
                        FocusCache.tradeMenuInstance.Close();

                        Find.WindowStack.Add(new OW_WaitingDialog());
                    }
                };

                action();
            }

            else if (mode == TakeMode.Barter)
            {
                Action action = delegate
                {
                    if (TradeSession.deal.TryExecute(out bool actuallyTraded))
                    {
                        SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                        TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
                        FocusCache.barterMenuInstance.Close();

                        Find.WindowStack.Add(new OW_WaitingDialog());
                    }
                };

                action();
            }

            else if (mode == TakeMode.Silo)
            {
                Action action = delegate
                {
                    if (TradeSession.deal.TryExecute(out bool actuallyTraded))
                    {
                        SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                        TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
                        FocusCache.siloDepositMenuInstance.Close();

                        Find.WindowStack.Add(new OW_WaitingDialog());
                    }
                };

                action();
            }

            else if (mode == TakeMode.Bank)
            {
                Action action = delegate
                {
                    if (TradeSession.deal.TryExecute(out bool actuallyTraded))
                    {
                        SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                        TradeSession.playerNegotiator.GetCaravan()?.RecacheImmobilizedNow();
                        FocusCache.bankDepositMenuInstance.Close();

                        Find.WindowStack.Add(new OW_WaitingDialog());
                    }
                };

                action();
            }

            Event.current.Use();
        }

        public static void SendTrades()
        {
            List<string> contents = new List<string>();

            if (TradeCache.takeMode == TakeMode.Gift)
            {
                contents.Add(FocusCache.focusedSettlement.Tile.ToString());
                contents.Add(((int)TradeCache.takeMode).ToString());

                foreach (TradeItem thing in TradeCache.tradeItems)
                {
                    contents.Add(Serializer.SoftSerialize(thing));
                }
            }

            else if (TradeCache.takeMode == TakeMode.Trade)
            {
                contents.Add(FocusCache.focusedSettlement.Tile.ToString());
                contents.Add(((int)TradeCache.takeMode).ToString());
                contents.Add(TradeCache.wantedSilver.ToString());

                foreach (TradeItem thing in TradeCache.tradeItems)
                {
                    contents.Add(Serializer.SoftSerialize(thing));
                }
            }

            else if (TradeCache.takeMode == TakeMode.Barter)
            {
                contents.Add(FocusCache.focusedSettlement.Tile.ToString());
                contents.Add(((int)TradeCache.takeMode).ToString());

                foreach (TradeItem thing in TradeCache.tradeItems)
                {
                    contents.Add(Serializer.SoftSerialize(thing));
                }

                TradeCache.inRebarter = true;
            }

            else if (TradeCache.takeMode == TakeMode.Silo)
            {
                contents.Add(Find.AnyPlayerHomeMap.Tile.ToString());
                contents.Add(((int)TradeCache.takeMode).ToString());

                foreach (TradeItem thing in TradeCache.tradeItems)
                {
                    contents.Add(Serializer.SoftSerialize(thing));
                }

                TradeCache.tradeItems.Clear();
            }

            else if (TradeCache.takeMode == TakeMode.Bank)
            {
                contents.Add(Find.AnyPlayerHomeMap.Tile.ToString());
                contents.Add(((int)TradeCache.takeMode).ToString());

                foreach (TradeItem thing in TradeCache.tradeItems)
                {
                    contents.Add(Serializer.SoftSerialize(thing));
                }

                TradeCache.tradeItems.Clear();
            }

            Packet SendThingsPacket = new Packet("SendThingsPacket", contents.ToArray());
            Network.SendData(SendThingsPacket);
        }

        public static void GetTradeRequest(Packet receivedPacket)
        {
            if (TradeCache.inTrade) return;
            else
            {
                List<string> tempContents = receivedPacket.contents.ToList();
                int tradeMode = int.Parse(tempContents[1]);
                tempContents.RemoveAt(1);

                if (tradeMode == (int)TakeMode.Gift)
                {
                    Find.WindowStack.Add(new OW_MPGiftRequest(tempContents.ToArray()));
                }

                else if (tradeMode == (int)TakeMode.Trade)
                {
                    Find.WindowStack.Add(new OW_MPTradeRequest(tempContents.ToArray()));
                }

                else if (tradeMode == (int)TakeMode.Barter)
                {
                    Find.WindowStack.Add(new OW_MPBarterRequest(tempContents.ToArray()));
                }
            }
        }

        public static void GetAcceptedTrade()
        {
            if (TradeCache.takeMode == TakeMode.Gift)
            {
                TradeCache.ResetTradeVariables();

                LetterCache.GetLetterDetails("Successful gift",
                    "The gift has been successful!", LetterDefOf.PositiveEvent);

                Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);

                Injections.thingsToDoInUpdate.Add(MPGame.ForceSave);
            }

            else if (TradeCache.takeMode == TakeMode.Trade)
            {
                GetSilverIntoCaravan();
            }

            else if (TradeCache.takeMode == TakeMode.Barter)
            {
                if (TradeCache.inRebarter)
                {
                    GetItemsIntoCaravan();
                }

                else
                {
                    TradeCache.tradeItems = TradeCache.incomingBarterItems.ToList();
                    GetItemsIntoSettlement();
                }
            }

            FocusCache.waitWindowInstance.Close();
        }

        public static void GetRejectedTrade()
        {
            if (TradeCache.takeMode == TakeMode.Trade)
            {
                GetItemsBackIntoCaravan();
                FocusCache.waitWindowInstance.Close();
                Find.WindowStack.Add(new OW_ErrorDialog("Trade was rejected by the player"));
            }

            else if (TradeCache.takeMode == TakeMode.Barter)
            {
                if (TradeCache.inRebarter)
                {
                    GetItemsBackIntoCaravan();
                    Find.WindowStack.Add(new OW_ErrorDialog("Trade was rejected by the player"));
                }

                else
                {
                    GetItemsBackIntoSettlement();
                    Find.WindowStack.Add(new OW_ErrorDialog("Trade was rejected by the player"));
                }

                FocusCache.waitWindowInstance.Close();
            }
        }

        public static void GetSilverIntoCaravan()
        {
            RimworldHandler.GetSilverToCaravan(TradeCache.wantedSilver);

            TradeCache.ResetTradeVariables();

            LetterCache.GetLetterDetails("Successful trade",
                "The trade has been successful!", LetterDefOf.PositiveEvent);

            Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);
        }

        public static void GetItemsIntoCaravan()
        {
            RimworldHandler.GetItemsToCaravan(TradeCache.incomingBarterItems.ToArray());

            LetterCache.GetLetterDetails("Successful trade",
                "The trade has been successful", LetterDefOf.PositiveEvent);

            Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);

            TradeCache.ResetTradeVariables();
        }

        public static void GetItemsBackIntoCaravan()
        {
            RimworldHandler.GetItemsToCaravan(TradeCache.tradeItems.ToArray(), false);

            TradeCache.ResetTradeVariables();

            FocusCache.waitWindowInstance.Close();
        }

        public static void GetItemsIntoSettlement()
        {
            RimworldHandler.GetItemsToSettlement(TradeCache.tradeItems.ToArray());

            LetterCache.GetLetterDetails("Successful trade",
                "The trade has been successful", LetterDefOf.PositiveEvent);

            Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);

            TradeCache.ResetTradeVariables();
        }

        public static void GetItemsBackIntoSettlement()
        {
            RimworldHandler.GetItemsToSettlement(TradeCache.tradeItems.ToArray(), false);

            TradeCache.ResetTradeVariables();
        }

        public static void GetItemsFromPods(CompLaunchable representative)
        {
            TradeCache.takeMode = TakeMode.Gift;

            foreach (CompTransporter pod in representative.TransportersInGroup)
            {
                ThingOwner directlyHeldThings = pod.GetDirectlyHeldThings();

                for (int num = directlyHeldThings.Count - 1; num >= 0; num--)
                {
                    TradeItem newTradeItem = new TradeItem();
                    newTradeItem.defName = directlyHeldThings[num].def.defName;

                    if (directlyHeldThings[num].Stuff != null)
                    {
                        newTradeItem.madeOfDef = directlyHeldThings[num].Stuff.defName;
                    }

                    newTradeItem.stackCount = directlyHeldThings[num].stackCount;

                    QualityCategory qc = QualityCategory.Normal;
                    directlyHeldThings[num].TryGetQuality(out qc);
                    newTradeItem.itemQuality = (int)qc;

                    if (directlyHeldThings[num].def == ThingDefOf.MinifiedThing || directlyHeldThings[num].def == ThingDefOf.MinifiedTree)
                    {
                        newTradeItem.isMinified = true;
                    }

                    TradeCache.tradeItems.Add(newTradeItem);
                }
            }
        }
    }
}
