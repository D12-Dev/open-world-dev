using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux
{
    public class OW_MPFactionSiteUse : Window
    {
        public override Vector2 InitialSize => new Vector2(500f, 375f);

        private Vector2 scrollPosition;

        private string windowTitle = "";
        private string windowDescription = "";

        private float buttonX = 150f;
        private float buttonY = 38f;

        int siteType = 0;

        public OW_MPFactionSiteUse(int siteType)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = false;
            closeOnAccept = false;
            closeOnCancel = true;

            this.siteType = siteType;

            if (siteType == (int)FactionCache.StructureTypes.Silo)
            {
                Packet RefreshFactionSiloPacket = new Packet("RefreshFactionSiloPacket");
                Network.SendData(RefreshFactionSiloPacket);
            }

            if (siteType == (int)FactionCache.StructureTypes.Bank)
            {
                Packet RefreshFactionBankPacket = new Packet("RefreshFactionBankPacket");
                Network.SendData(RefreshFactionBankPacket);
            }
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Widgets.DrawLineHorizontal(rect.xMin, (rect.y + Text.CalcSize(windowTitle).y + 10), rect.width);

            HandleWindowContents(rect);
        }

        private void HandleWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            float descriptionLineDif = windowDescriptionDif + Text.CalcSize(windowDescription).y;

            if (siteType == (int)FactionCache.StructureTypes.Silo)
            {
                windowTitle = "Silo Management Menu";
                windowDescription = "Manage the resources available for all faction members";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Rect listRect = new Rect(new Vector2(rect.xMin, rect.yMin + 90), new Vector2(rect.width, 146));
                GenerateList(listRect, 0);

                Text.Font = GameFont.Medium;
                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Deposit"))
                {
                    Settlement settlementToUse = Find.WorldObjects.Settlements.First((Settlement x) => FactionsCache.allOnlineFactions.Contains(x.Faction));

                    if (settlementToUse == null) return;
                    else FocusCache.focusedSettlement = settlementToUse;

                    if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new OW_MPFactionSiloDeposit());
                    else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                }
            }

            else if (siteType == (int)FactionCache.StructureTypes.Marketplace)
            {
                windowTitle = "Marketplace Management Menu";
                windowDescription = "Trade over unlimited distances with all faction members";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Rect listRect = new Rect(new Vector2(rect.xMin, rect.yMin + 90), new Vector2(rect.width, 194f));
                GenerateList(listRect, 1);
            }

            else if (siteType == (int)FactionCache.StructureTypes.ProductionSite)
            {
                windowTitle = "Production Site Management Menu";
                windowDescription = "Generate resources over time for all members";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 5 - 40), new Vector2(buttonX * 2, buttonY)), "Receive Food"))
                {
                    FactionCache.selectedProduct = FactionCache.ProductionSiteProduct.Food;
                    DoAfter();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 4 - 30), new Vector2(buttonX * 2, buttonY)), "Receive Neutroamine"))
                {
                    FactionCache.selectedProduct = FactionCache.ProductionSiteProduct.Neutroamine;
                    DoAfter();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 3 - 20), new Vector2(buttonX * 2, buttonY)), "Receive Components"))
                {
                    FactionCache.selectedProduct = FactionCache.ProductionSiteProduct.Components;
                    DoAfter();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Receive Fuel"))
                {
                    FactionCache.selectedProduct = FactionCache.ProductionSiteProduct.Fuel;
                    DoAfter();
                }

                void DoAfter()
                {
                    Find.WindowStack.Add(new OW_InfoDialog("You will now start receiving this material"));
                    Close();

                    string[] contents = new string[] { ((int)FactionCache.selectedProduct).ToString() };
                    Packet ChangeProductionItemPacket = new Packet("ChangeProductionItemPacket", contents);
                    Network.SendData(ChangeProductionItemPacket);
                }
            }

            else if (siteType == (int)FactionCache.StructureTypes.Wonder)
            {
                //windowTitle = "Wonder Structure Management Menu";
                //windowDescription = "Assert superiority against the whole planet";

                //Text.Font = GameFont.Small;
                //Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                //Text.Font = GameFont.Medium;

                //Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                //if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Utilize"))
                //{
                //    Find.WindowStack.Add(new OW_ErrorDialog("This action is not implemented yet"));
                //}
            }

            else if (siteType == (int)FactionCache.StructureTypes.Bank)
            {
                windowTitle = "Bank Management Menu";
                windowDescription = "Generate passive wealth over time";

                string windowMoneyText = $"Current funds: {FactionCache.bankSilver} silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowMoneyText).x / 2, windowDescriptionDif + 30, Text.CalcSize(windowMoneyText).x, Text.CalcSize(windowMoneyText).y), windowMoneyText);

                Text.Font = GameFont.Medium;
                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 3 - 20), new Vector2(buttonX * 2, buttonY)), "Deposit"))
                {
                    Settlement settlementToUse = Find.WorldObjects.Settlements.First((Settlement x) => FactionsCache.allOnlineFactions.Contains(x.Faction));

                    if (settlementToUse == null) return;
                    else FocusCache.focusedSettlement = settlementToUse;

                    if (RimworldHandler.CheckIfAnySocialPawn(0)) Find.WindowStack.Add(new OW_MPFactionBankDeposit());
                    else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                }

                Text.Font = GameFont.Medium;
                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Withdraw"))
                {
                    Action actionToDo = delegate
                    {
                        if (FocusCache.quantityChosenOnDialog > FactionCache.bankSilver)
                        {
                            Find.WindowStack.Add(new OW_ErrorDialog("You are trying to withdraw more than available silver"));
                        }

                        else if (FocusCache.quantityChosenOnDialog == 0) return;

                        else
                        {
                            string[] contents = new string[] { FocusCache.quantityChosenOnDialog.ToString() };
                            Packet FactionBankWithdrawPacket = new Packet("FactionBankWithdrawPacket", contents);
                            Network.SendData(FactionBankWithdrawPacket);

                            Find.WindowStack.Add(new OW_WaitingDialog());
                        }
                    };

                    Find.WindowStack.Add(new OW_QuantityDialog(actionToDo));
                }
            }

            else if (siteType == (int)FactionCache.StructureTypes.Aeroport)
            {
                windowTitle = "Aeroport Management Menu";
                windowDescription = "Jump between allied aeroports through the planet for a fee";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Rect listRect = new Rect(new Vector2(rect.xMin, rect.yMin + 90), new Vector2(rect.width, 194f));
                GenerateList(listRect, 3);
            }

            else if (siteType == (int)FactionCache.StructureTypes.Courier)
            {
                windowTitle = "Courier Station Management Menu";
                windowDescription = "Trade over unlimited distances with everyone for a fee";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Rect listRect = new Rect(new Vector2(rect.xMin, rect.yMin + 90), new Vector2(rect.width, 194f));
                GenerateList(listRect, 2);
            }

            Text.Font = GameFont.Medium;
            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Close"))
            {
                Close();
            }
        }

        private void GenerateList(Rect mainRect, int listMode)
        {
            if (listMode == 0)
            {
                var orderedDictionary = FactionCache.siloContents
                    .OrderBy(x => x.stackCount)
                    .ToList();

                DoList(mainRect, listMode, items: orderedDictionary);
            }

            else if (listMode == 1)
            {
                var orderedDictionary = WorldCache.onlineSettlements
                    .Where(x => x.Faction == FactionsCache.onlineAllyFaction)
                    .OrderBy(x => x.Name)
                    .ToList();

                DoList(mainRect, listMode, settlements: orderedDictionary);
            }

            else if (listMode == 2)
            {
                var orderedDictionary = WorldCache.onlineSettlements
                    .OrderBy(x => x.Name)
                    .ToList();

                DoList(mainRect, listMode, settlements: orderedDictionary);
            }

            else if (listMode == 3)
            {
                var orderedDictionary = WorldCache.onlineStructures
                    .Where(x => x.MainSitePartDef.defName == FactionCache.aeroportDef.defName 
                        && x.Tile != FocusCache.focusedCaravan.Tile &&
                        (x.Faction == FactionsCache.onlineNeutralFaction || x.Faction == FactionsCache.onlineAllyFaction))
                    .ToList();

                DoList(mainRect, listMode, sites: orderedDictionary);
            }
        }

        private void DoList(Rect mainRect, int listMode, List<TradeItem> items = null, List<Settlement> settlements = null, List<Site> sites = null)
        {
            float num = 0;
            float num2 = 0;
            float num3 = 0;
            int num4 = 0;

            if (listMode == 0)
            {
                float height = 6f + items.Count() * 30f;
                Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

                Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

                num = 0;
                num2 = scrollPosition.y - 30f;
                num3 = scrollPosition.y + mainRect.height;
                num4 = 0;

                foreach (TradeItem file in items)
                {
                    if (num > num2 && num < num3)
                    {
                        Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 30f);
                        DrawCustomRow(rect, num4, listMode, item: file);
                    }

                    num += 30f;
                    num4++;
                }
            }

            else if (listMode == 1 || listMode == 2)
            {
                float height = 6f + settlements.Count() * 30f;
                Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

                Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

                num = 0;
                num2 = scrollPosition.y - 30f;
                num3 = scrollPosition.y + mainRect.height;
                num4 = 0;

                foreach (Settlement settlement in settlements)
                {
                    if (num > num2 && num < num3)
                    {
                        Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 30f);
                        DrawCustomRow(rect, num4, listMode, settlement: settlement);
                    }

                    num += 30f;
                    num4++;
                }
            }

            else if (listMode == 3)
            {
                float height = 6f + sites.Count() * 30f;
                Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

                Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

                num = 0;
                num2 = scrollPosition.y - 30f;
                num3 = scrollPosition.y + mainRect.height;
                num4 = 0;

                foreach (Site site in sites)
                {
                    if (num > num2 && num < num3)
                    {
                        Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 30f);
                        DrawCustomRow(rect, num4, listMode, structure: site);
                    }

                    num += 30f;
                    num4++;
                }
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, int index, int rowType, TradeItem item = null, Settlement settlement = null, Site structure = null)
        {
            float buttonX = 47f;
            float buttonY = 30f;

            Text.Font = GameFont.Small;

            if (index % 2 == 0) Widgets.DrawLightHighlight(rect);
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 52f, rect.height));

            if (rowType == 0)
            {
                string itemName = "NULL";
                foreach (ThingDef toCheck in DefDatabase<ThingDef>.AllDefs)
                {
                    if (item.defName == toCheck.defName)
                    {
                        itemName = toCheck.label;
                        break;
                    }
                }

                if (itemName.Length > 1) itemName = char.ToUpper(itemName[0]) + itemName.Substring(1);
                else itemName = itemName.ToUpper();

                Widgets.Label(fixedRect, $"{itemName} (x{item.stackCount}) ({(QualityCategory)item.itemQuality})");
                    
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.y), new Vector2(buttonX, buttonY)), "Take"))
                {
                    if (FactionCache.canWithdrawFromSilo)
                    {
                        string[] contents = new string[] { Serializer.SoftSerialize(item) };
                        Packet WithdrawFromFactionSiloPacket = new Packet("WithdrawFromFactionSiloPacket", contents);
                        Network.SendData(WithdrawFromFactionSiloPacket);

                        FactionCache.canWithdrawFromSilo = false;
                    }
                }
            }

            else if (rowType == 1 || rowType == 2)
            {
                Widgets.Label(fixedRect, settlement.Label);
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX * 3 - 20, rect.y), new Vector2(buttonX, buttonY)), "Trade"))
                {
                    DoTradeCheck(rowType, settlement, 0);
                }

                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX * 2 - 10, rect.y), new Vector2(buttonX, buttonY)), "Barter"))
                {
                    DoTradeCheck(rowType, settlement, 1);
                }
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.y), new Vector2(buttonX, buttonY)), "Gift"))
                {
                    DoTradeCheck(rowType, settlement, 2);
                }
            }

            else if (rowType == 3)
            {
                Widgets.Label(fixedRect, "Aeroport - " + structure.Tile);
                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.y), new Vector2(buttonX, buttonY)), "Travel"))
                {
                    if (!CaravanInventoryUtility.HasThings(FocusCache.focusedCaravan, ThingDefOf.Silver, FactionCache.aeroportUsageCost))
                    {
                        Find.WindowStack.Add(new OW_ErrorDialog($"You need {FactionCache.aeroportUsageCost} silver to use the aeroport"));
                        Close();
                    }

                    else
                    {
                        RimworldHandler.TakeSilverFromCaravan(FactionCache.aeroportUsageCost);
                        FocusCache.focusedCaravan.Tile = structure.Tile;
                        FocusCache.focusedCaravan.pather.nextTile = structure.Tile;
                        Close();
                    }
                }
            }
        }

        private void DoTradeCheck(int rowType, Settlement settlement, int tradeMode)
        {
            if (rowType == 2)
            {
                if (!CaravanInventoryUtility.HasThings(FocusCache.focusedCaravan, ThingDefOf.Silver, FactionCache.courierUsageCost))
                {
                    Find.WindowStack.Add(new OW_ErrorDialog($"You need {FactionCache.courierUsageCost} silver to use the courier"));
                    Close();
                    return;
                }

                else
                {
                    FocusCache.focusedSettlement = settlement;
                    if (RimworldHandler.CheckIfAnySocialPawn(0))
                    {
                        RimworldHandler.TakeSilverFromCaravan(FactionCache.aeroportUsageCost);

                        if (tradeMode == 0) Find.WindowStack.Add(new OW_MPTradeMenu());
                        else if (tradeMode == 1) Find.WindowStack.Add(new OW_MPBarterMenu());
                        else if (tradeMode == 2) Find.WindowStack.Add(new OW_MPGiftMenu());
                    }
                    else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
                }
            }

            FocusCache.focusedSettlement = settlement;
            if (RimworldHandler.CheckIfAnySocialPawn(0))
            {
                if (tradeMode == 0) Find.WindowStack.Add(new OW_MPTradeMenu());
                else if (tradeMode == 1) Find.WindowStack.Add(new OW_MPBarterMenu());
                else if (tradeMode == 2) Find.WindowStack.Add(new OW_MPGiftMenu());
            }
            else Find.WindowStack.Add(new OW_ErrorDialog("Pawn does not have enough social to trade"));
        }
    }
}
