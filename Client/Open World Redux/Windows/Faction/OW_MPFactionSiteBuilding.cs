
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux
{
    public class OW_MPFactionSiteBuilding : Window
    {
        public override Vector2 InitialSize => new Vector2(325f, 355f);

        private Vector2 scrollPosition = Vector2.zero;

        private string windowTitle = "Faction Site";
        private string windowDescription = "Build an utility site for your faction";

        private float buttonX = 150f;
        private float buttonY = 38f;

        bool isBuildingSilo;
        bool isBuildingMarketplace;
        bool isBuildingProductionSite;
        bool isBuildingWonder;
        bool isBuildingBank;
        bool isBuildingAeroport;
        bool isBuildingCourierStation;

        public OW_MPFactionSiteBuilding()
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            forcePause = false;
            closeOnAccept = false;
            closeOnCancel = true;
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

            if (isBuildingSilo)
            {
                windowDescription = "About silo construction";

                string siteDescription = "Global faction storage";

                string costList = $"Cost: {FactionCache.siloCost}x silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(siteDescription).x / 2, rect.y + 80, Text.CalcSize(siteDescription).x, Text.CalcSize(siteDescription).y), siteDescription);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(costList).x / 2, rect.y + 100, Text.CalcSize(costList).x, Text.CalcSize(costList).y), costList);
                Text.Font = GameFont.Medium;

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Build"))
                {
                    Find.WindowStack.Add(new OW_MPFactionSiteConfirmBuild(FactionCache.StructureTypes.Silo));
                    Close();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    isBuildingSilo = false;
                }
            }

            else if (isBuildingMarketplace)
            {
                windowDescription = "About marketplace construction";

                string siteDescription = "Global trading between members";

                string costList = $"Cost: {FactionCache.marketplaceCost}x silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(siteDescription).x / 2, rect.y + 80, Text.CalcSize(siteDescription).x, Text.CalcSize(siteDescription).y), siteDescription);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(costList).x / 2, rect.y + 100, Text.CalcSize(costList).x, Text.CalcSize(costList).y), costList);
                Text.Font = GameFont.Medium;

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Build"))
                {
                    Find.WindowStack.Add(new OW_MPFactionSiteConfirmBuild(FactionCache.StructureTypes.Marketplace));
                    Close();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    isBuildingMarketplace = false;
                }
            }

            else if (isBuildingProductionSite)
            {
                windowDescription = "About production site construction";

                string siteDescription = "Passive material generation";

                string costList = $"Cost: {FactionCache.productionSiteCost}x silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(siteDescription).x / 2, rect.y + 80, Text.CalcSize(siteDescription).x, Text.CalcSize(siteDescription).y), siteDescription);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(costList).x / 2, rect.y + 100, Text.CalcSize(costList).x, Text.CalcSize(costList).y), costList);
                Text.Font = GameFont.Medium;

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Build"))
                {
                    Find.WindowStack.Add(new OW_MPFactionSiteConfirmBuild(FactionCache.StructureTypes.ProductionSite));
                    Close();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    isBuildingProductionSite = false;
                }
            }

            else if (isBuildingWonder)
            {
                windowDescription = "About wonder construction";

                string siteDescription = "Unique server structure";

                string costList = $"Cost: {FactionCache.wonderCost}x silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(siteDescription).x / 2, rect.y + 80, Text.CalcSize(siteDescription).x, Text.CalcSize(siteDescription).y), siteDescription);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(costList).x / 2, rect.y + 100, Text.CalcSize(costList).x, Text.CalcSize(costList).y), costList);
                Text.Font = GameFont.Medium;

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Build"))
                {
                    Find.WindowStack.Add(new OW_MPFactionSiteConfirmBuild(FactionCache.StructureTypes.Wonder));
                    Close();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    isBuildingWonder = false;
                }
            }

            else if (isBuildingBank)
            {
                windowDescription = "About bank construction";

                string siteDescription = "Wealth generation over time";

                string costList = $"Cost: {FactionCache.bankCost}x silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(siteDescription).x / 2, rect.y + 80, Text.CalcSize(siteDescription).x, Text.CalcSize(siteDescription).y), siteDescription);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(costList).x / 2, rect.y + 100, Text.CalcSize(costList).x, Text.CalcSize(costList).y), costList);
                Text.Font = GameFont.Medium;

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Build"))
                {
                    Find.WindowStack.Add(new OW_MPFactionSiteConfirmBuild(FactionCache.StructureTypes.Bank));
                    Close();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    isBuildingBank = false;
                }
            }

            else if (isBuildingAeroport)
            {
                windowDescription = "About aeroport construction";

                string siteDescription = "Fast travel between other aeroports";

                string costList = $"Cost: {FactionCache.aeroportCost}x silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(siteDescription).x / 2, rect.y + 80, Text.CalcSize(siteDescription).x, Text.CalcSize(siteDescription).y), siteDescription);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(costList).x / 2, rect.y + 100, Text.CalcSize(costList).x, Text.CalcSize(costList).y), costList);
                Text.Font = GameFont.Medium;

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Build"))
                {
                    Find.WindowStack.Add(new OW_MPFactionSiteConfirmBuild(FactionCache.StructureTypes.Aeroport));
                    Close();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    isBuildingAeroport = false;
                }
            }

            else if (isBuildingCourierStation)
            {
                windowDescription = "About courier station construction";

                string siteDescription = "Trade all across the globe from here";

                string costList = $"Cost: {FactionCache.courierCost}x silver";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(siteDescription).x / 2, rect.y + 80, Text.CalcSize(siteDescription).x, Text.CalcSize(siteDescription).y), siteDescription);

                Widgets.Label(new Rect(centeredX - Text.CalcSize(costList).x / 2, rect.y + 100, Text.CalcSize(costList).x, Text.CalcSize(costList).y), costList);
                Text.Font = GameFont.Medium;

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 2, rect.yMax - buttonY * 2 - 10), new Vector2(buttonX * 2, buttonY)), "Build"))
                {
                    Find.WindowStack.Add(new OW_MPFactionSiteConfirmBuild(FactionCache.StructureTypes.Courier));
                    Close();
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Back"))
                {
                    isBuildingCourierStation = false;
                }
            }

            else
            {
                windowDescription = "Build an utility site for your faction";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                string[] buttons = new string[]
                {
                    "Silo",
                    "Marketplace",
                    "Production Site",
                    "Wonder Structure",
                    "Bank",
                    "Aeroport",
                    "Courier Station"
                };

                GenerateList(new Rect(rect.x, rect.yMax - buttonY * 5 - 40, rect.width, 175f), buttons);

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2) * 1.5f, rect.yMax - buttonY), new Vector2(buttonX * 1.5f, buttonY)), "Close"))
                {
                    Close();
                }
            }
        }

        private void GenerateList(Rect mainRect, string[] buttons)
        {
            float height = 6f + buttons.Count() * buttonY;

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float yPadding = 0;
            float extraLenght = 32f;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;

            int index = 0;
            foreach (string str in buttons)
            {
                if (yPadding > num2 && yPadding < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + yPadding, viewRect.width + extraLenght, buttonY);
                    DrawCustomRow(rect, str, index);
                    index++;
                }

                yPadding += buttonY;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string buttonName, int index)
        {
            Text.Font = GameFont.Small;
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 36f, rect.height));

            if (Widgets.ButtonText(fixedRect, buttonName))
            {
                if (buttonName == "Silo") isBuildingSilo = true;
                else if (buttonName == "Marketplace") isBuildingMarketplace = true;
                else if (buttonName == "Production Site") isBuildingProductionSite = true;
                else if (buttonName == "Wonder Structure") isBuildingWonder = true;
                else if (buttonName == "Bank") isBuildingBank = true;
                else if (buttonName == "Aeroport") isBuildingAeroport = true;
                else if (buttonName == "Courier Station") isBuildingCourierStation = true;
            }
        }
    }
}