using RimWorld;
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
    public static class WorldFactionUIOverride
    {
        private static Vector2 scrollPosition;

        private static float listingHeight;

        private static float warningHeight;

        private const float RowHeight = 24f;

        private const float AddButtonHeight = 28f;

        private const int MaxVisibleFactions = 12;

        private const int MaxVisibleFactionsRecommended = 11;

        private const float RowMarginX = 6f;
        
        public static void DoWindowContents(Rect rect, List<FactionDef> factions) //,bool isDefaultFactionCounts)
        {
            Rect rect2 = new Rect(rect.x, rect.y, rect.width, Text.LineHeight);
            Widgets.Label(rect2, "Factions".Translate());
            TooltipHandler.TipRegion(rect2, () => "FactionSelectionDesc".Translate(12), 4534123);
            float num = Text.LineHeight + 4f;
            float num2 = rect.width * 0.0500000119f;
            Rect rect3 = new Rect(rect.x + num2, rect.y + num, rect.width * 0.9f, rect.height - num - Text.LineHeight - 28f - warningHeight);
            Rect outRect = rect3.ContractedBy(4f);
            Rect rect4 = new Rect(outRect.x, outRect.y, outRect.width, listingHeight);
            Widgets.BeginScrollView(outRect, ref scrollPosition, rect4);
            listingHeight = 0f;
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = rect4.width;
            listing_Standard.Begin(rect4);
            for (int i = 0; i < factions.Count; i++)
            {
                if (factions[i].displayInFactionSelection)
                {
                    listing_Standard.Gap(4f);
                    if (DoRow(listing_Standard.GetRect(24f), factions[i], factions, i))
                    {
                        i--;
                    }
                    listing_Standard.Gap(4f);
                    listingHeight += 32f;
                }
            }
            listing_Standard.End();
            Widgets.EndScrollView();
            Rect rect5 = new Rect(outRect.x, Mathf.Min(rect3.yMax, outRect.y + listingHeight + 4f), outRect.width, 28f);
            if (Widgets.ButtonText(rect5, "Add".Translate().CapitalizeFirst() + "...") && TutorSystem.AllowAction("ConfiguringWorldFactions"))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (FactionDef configurableFaction in FactionGenerator.ConfigurableFactions)
                {
                    if (!configurableFaction.displayInFactionSelection)
                    {
                        continue;
                    }
                    FactionDef localDef = configurableFaction;
                    string text = localDef.LabelCap;
                    Action action = delegate
                    {
                        factions.Add(localDef);
                    };
                    AcceptanceReport acceptanceReport = CanAddFaction(localDef);
                    if (!acceptanceReport)
                    {
                        action = null;
                        if (!acceptanceReport.Reason.NullOrEmpty())
                        {
                            text = text + " (" + acceptanceReport.Reason + ")";
                        }
                    }
                    else
                    {
                        int num3 = factions.Count((FactionDef x) => x == localDef);
                        if (num3 > 0)
                        {
                            text = text + " (" + num3 + ")";
                        }
                    }
                    FloatMenuOption floatMenuOption = new FloatMenuOption(text, action, localDef.FactionIcon, localDef.DefaultColor, MenuOptionPriority.Default, null, null, 24f, (Rect r) => Widgets.InfoCardButton(r.x, r.y + 3f, localDef), null, playSelectionSound: true, 0, HorizontalJustification.Left, extraPartRightJustified: true);
                    floatMenuOption.tooltip = text.AsTipTitle() + "\n" + localDef.Description;
                    list.Add(floatMenuOption);
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            float yMax = rect5.yMax;
            int num4 = factions.Count((FactionDef x) => !x.hidden);
            StringBuilder stringBuilder = new StringBuilder();
            if (num4 == 0)
            {
                stringBuilder.AppendLine("FactionsDisabledWarning".Translate());
            }
            else
            {
                if (ModsConfig.RoyaltyActive && !factions.Contains(FactionDefOf.Empire))
                {
                    stringBuilder.AppendLine("Warning".Translate() + ": " + "FactionDisabledContentWarning".Translate(FactionDefOf.Empire.label));
                }
                if (!factions.Contains(FactionDefOf.Mechanoid))
                {
                    stringBuilder.AppendLine("Warning".Translate() + ": " + "MechanoidsDisabledContentWarning".Translate(FactionDefOf.Mechanoid.label));
                }
                if (!factions.Contains(FactionDefOf.Insect))
                {
                    stringBuilder.AppendLine("Warning".Translate() + ": " + "InsectsDisabledContentWarning".Translate(FactionDefOf.Insect.label));
                }
            }
            warningHeight = 0f;
            if (stringBuilder.Length > 0)
            {
                bool wordWrap = Text.WordWrap;
                string text2 = stringBuilder.ToString().TrimEndNewlines();
                Rect rect6 = new Rect(rect.x, yMax, rect.width, rect.yMax - yMax);
                GUI.color = Color.yellow;
                Text.Font = GameFont.Tiny;
                warningHeight = Text.CalcHeight(text2, rect6.width);
                Text.WordWrap = true;
                Widgets.Label(rect6, text2);
                Text.WordWrap = wordWrap;
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
            }
            AcceptanceReport CanAddFaction(FactionDef f)
            {
                if (!f.hidden && factions.Count((FactionDef x) => !x.hidden) >= 12)
                {
                    return "TotalFactionsAllowed".Translate(12).ToString().UncapitalizeFirst();
                }
                if (factions.Count((FactionDef x) => x == f) >= f.maxConfigurableAtWorldCreation)
                {
                    return "MaxFactionsForType".Translate(f.maxConfigurableAtWorldCreation).ToString().UncapitalizeFirst();
                }
                return true;
            }
        }

        public static bool DoRow(Rect rect, FactionDef factionDef, List<FactionDef> factions, int index)
        {
            bool result = false;
            Rect rect2 = new Rect(rect.x, rect.y - 4f, rect.width, rect.height + 8f);
            if (index % 2 == 1)
            {
                Widgets.DrawLightHighlight(rect2);
            }
            Widgets.BeginGroup(rect);
            WidgetRow widgetRow = new WidgetRow(6f, 0f);
            GUI.color = factionDef.DefaultColor;
            widgetRow.Icon(factionDef.FactionIcon);
            GUI.color = Color.white;
            widgetRow.Gap(4f);
            Text.Anchor = TextAnchor.MiddleCenter;
            widgetRow.Label(factionDef.LabelCap);
            //Log.Message(factionDef.LabelCap); // Prints labal of Open World
            Text.Anchor = TextAnchor.UpperLeft;
            if (factionDef.LabelCap != "Open World Settlements")
            {

                if (Widgets.ButtonImage(new Rect(rect.width - 24f - 6f, 0f, 24f, 24f), TexButton.DeleteX) && TutorSystem.AllowAction("ConfiguringWorldFactions"))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    // Log.Message(factionDef.LabelCap);

                    factions.RemoveAt(index);
                    result = true;
                }
                
            }
            Widgets.EndGroup();
            if (Mouse.IsOver(rect2))
            {
                TooltipHandler.TipRegion(rect2, factionDef.LabelCap.AsTipTitle() + "\n" + factionDef.Description);
                Widgets.DrawHighlight(rect2);
            }
            return result;
        }
    }
}
