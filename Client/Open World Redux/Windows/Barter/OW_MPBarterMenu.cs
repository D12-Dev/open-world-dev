﻿using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OpenWorldRedux
{
    public class OW_MPBarterMenu : Window
    {
        public override Vector2 InitialSize => new Vector2(835f, 512f);

        public string windowTitle = "Barter Menu";

        public string windowDescription = "Select the items you wish to barter with this settlement";

        private List<Tradeable> cachedTradeables;

        private Vector2 scrollPosition = Vector2.zero;

        private Caravan caravan = FocusCache.focusedCaravan;

        private Settlement settlement = FocusCache.focusedSettlement;

        private QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

        public override QuickSearchWidget CommonSearchWidget => quickSearchWidget;

        public string invoker;

        public bool fromOtherSide;

        public OW_MPBarterMenu(string invoker = null)
        {
            FocusCache.barterMenuInstance = this;
            Pawn playerNegotiator = null;

            if (invoker == null)
            {
                this.invoker = settlement.Tile.ToString();
                fromOtherSide = false;

                playerNegotiator = caravan.PawnsListForReading.Find(fetch => fetch.IsColonist && !fetch.skills.skills[10].PermanentlyDisabled);
            }

            else
            {
                this.invoker = invoker;
                fromOtherSide = true;

                TradeCache.incomingBarterItems = TradeCache.tradeItems.ToList();
                TradeCache.tradeItems.Clear();

                playerNegotiator = Find.AnyPlayerHomeMap.mapPawns.AllPawns.Find(fetch => fetch.IsColonist && !fetch.skills.skills[10].PermanentlyDisabled);
            }

            GenerateTradeList();
            CacheTradeables();
            SetupParameters();

            if (fromOtherSide) TradeSession.SetupWith(Find.WorldObjects.Settlements.Find(fetch => FactionsCache.allOnlineFactions.Contains(fetch.Faction)), playerNegotiator, true);
            else TradeSession.SetupWith(settlement, playerNegotiator, true);

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;

            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        private void SetupParameters()
        {
            commonSearchWidgetOffset.x = InitialSize.x - 50;
            commonSearchWidgetOffset.y = InitialSize.y - 50;
        }

        public void CacheTradeables()
        {
            cachedTradeables = (from tr in TradeCache.listToShowInTradesMenu
                                where quickSearchWidget.filter.Matches(tr.Label)
                                orderby 0 descending
                                select tr)
                                .ThenBy((Tradeable tr) => tr.ThingDef.label)
                                .ThenBy((Tradeable tr) => tr.AnyThing.TryGetQuality(out QualityCategory qc) ? ((int)qc) : (-1))
                                .ThenBy((Tradeable tr) => tr.AnyThing.HitPoints)
                                .ToList();
            quickSearchWidget.noResultsMatched = !cachedTradeables.Any();
        }

        public void GenerateTradeList()
        {
            List<Thing> tradeableItems = new List<Thing>();
            TradeCache.listToShowInTradesMenu = new List<Tradeable>();

            if (fromOtherSide)
            {
                Map map = Find.AnyPlayerHomeMap;

                Zone[] zones = map.zoneManager.AllZones.ToArray();
                foreach (Zone zone in zones)
                {
                    Thing[] items = zone.AllContainedThings.ToArray();
                    foreach (Thing item in items)
                    {
                        if (item is Pawn) continue;
                        if (item.stackCount < 1) continue;
                        if (!item.def.alwaysHaulable) continue;
                        if (!DefDatabase<ThingDef>.AllDefs.Contains(item.def)) continue;

                        tradeableItems.Add(item);
                    }
                }

                Pawn[] pawnsInMap = map.mapPawns.PawnsInFaction(Faction.OfPlayer).ToArray();
                foreach (Pawn pawn in pawnsInMap)
                {
                    if (!pawn.NonHumanlikeOrWildMan()) continue;
                    tradeableItems.Add(pawn);
                }
            }

            else
            {
                tradeableItems = CaravanInventoryUtility.AllInventoryItems(caravan);

                foreach (Pawn pawn in caravan.pawns)
                {
                    if (!pawn.NonHumanlikeOrWildMan()) continue;

                    Tradeable tradeable = new Tradeable();
                    tradeable.AddThing(pawn, Transactor.Colony);
                    TradeCache.listToShowInTradesMenu.Add(tradeable);
                }
            }

            foreach (Thing item in tradeableItems)
            {
                Tradeable tradeable = new Tradeable();
                tradeable.AddThing(item, Transactor.Colony);
                TradeCache.listToShowInTradesMenu.Add(tradeable);
            }
        }

        private void FillMainRect(Rect mainRect)
        {
            Widgets.DrawLineHorizontal(mainRect.x, mainRect.y - 1, mainRect.width);
            Widgets.DrawLineHorizontal(mainRect.x, mainRect.yMax + 1, mainRect.width);

            float height = 6f + (float)cachedTradeables.Count * 30f;
            Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);
            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);
            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            for (int i = 0; i < cachedTradeables.Count; i++)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, num, viewRect.width, 30f);
                    DrawCustomRow(rect, cachedTradeables[i], num4);
                }

                num += 30f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        public override void DoWindowContents(Rect inRect)
        {
            float windowDescriptionDif = Text.CalcSize(windowDescription).y + 8;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect((inRect.width / 2) - Text.CalcSize(windowTitle).x / 2, inRect.y, inRect.width, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect((inRect.width / 2) - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, inRect.width, Text.CalcSize(windowDescription).y), windowDescription);

            float num2 = 0f;
            Rect mainRect = new Rect(0f, 58f + num2, inRect.width, inRect.height - 58f - 38f - num2 - 20f);
            FillMainRect(mainRect);

            float buttonX = 137f;
            float buttonY = 38f;
            if (Widgets.ButtonText(new Rect(new Vector2(inRect.x, inRect.yMax - buttonY), new Vector2(137f, buttonY)), "Accept"))
            {
                OnAccept();
            }

            if (Widgets.ButtonText(new Rect(new Vector2((inRect.width / 2) - (buttonX / 2), inRect.yMax - buttonY), new Vector2(137f, buttonY)), "Reset"))
            {
                OnReset();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(inRect.xMax - buttonX, inRect.yMax - buttonY), new Vector2(137f, buttonY)), "Cancel"))
            {
                OnCancel();
            }
        }

        private void DrawCustomRow(Rect rect, Tradeable trad, int index)
        {
            Text.Font = GameFont.Small;
            float width = rect.width;

            Widgets.DrawLightHighlight(rect);

            GUI.BeginGroup(rect);

            //Change Quantity Buttons
            Rect rect5 = new Rect(width - 225, 0f, 240f, rect.height);
            bool flash = Time.time - Dialog_Trade.lastCurrencyFlashTime < 1f && trad.IsCurrency;
            TransferableUIUtility.DoCountAdjustInterface(rect5, trad, index, trad.GetMinimumToTransfer(), trad.GetMaximumToTransfer(), flash);

            width -= 225;

            //Caravan Quantity Label
            int num2 = trad.CountHeldBy(Transactor.Colony);
            if (num2 != 0)
            {
                Rect rect6 = new Rect(width, 0f, 100f, rect.height);
                Rect rect7 = new Rect(rect6.x - 75f, 0f, 75f, rect.height);
                if (Mouse.IsOver(rect7))
                {
                    Widgets.DrawHighlight(rect7);
                }

                Rect rect8 = rect7;
                rect8.xMin += 5f;
                rect8.xMax -= 5f;
                Widgets.Label(rect8, num2.ToStringCached());
                TooltipHandler.TipRegionByKey(rect7, "ColonyCount");
            }

            width -= 90f;

            //Caravan Items Label
            TransferableUIUtility.DoExtraIcons(trad, rect, ref width);

            Rect idRect = new Rect(0f, 0f, width, rect.height);
            TransferableUIUtility.DrawTransferableInfo(trad, idRect, Color.white);
            GenUI.ResetLabelAlign();
            GUI.EndGroup();
        }

        
        
        private void OnAccept()
        {
            Find.WindowStack.Add(new OW_MPConfirmBarter(invoker, fromOtherSide));
        }

        private void OnCancel()
        {
            if (fromOtherSide)
            {
                string[] contentsReject = new string[] { invoker };
                Packet RejectTradePacket = new Packet("RejectTradePacket", contentsReject);
                Network.SendData(RejectTradePacket);

                if (!TradeCache.inRebarter) TradeCache.tradeItems.Clear();
                TradeHandler.GetRejectedTrade();

                FocusCache.barterRequestInstance.Close();
                Close();
            }

            else Close();
        }

        private void OnReset()
        {
            SoundDefOf.Tick_Low.PlayOneShotOnCamera();
            GenerateTradeList();
            CacheTradeables();
            TradeSession.deal.Reset();
        }
    }
}