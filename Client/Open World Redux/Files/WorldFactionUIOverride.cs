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

        public static void DoWindowContents(Rect rect, List<FactionDef> factions)
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
                    if (DoRow(listing_Standard.GetRect(24f), factions[i], factions, i)) i--;

                    listing_Standard.Gap(4f);
                    listingHeight += 32f;
                }
            }

            listing_Standard.End();
            Widgets.EndScrollView();
        }

        public static bool DoRow(Rect rect, FactionDef factionDef, List<FactionDef> factions, int index)
        {
            bool result = false;
            Rect rect2 = new Rect(rect.x, rect.y - 4f, rect.width, rect.height + 8f);
            if (index % 2 == 1) Widgets.DrawLightHighlight(rect2);

            Widgets.BeginGroup(rect);
            WidgetRow widgetRow = new WidgetRow(6f, 0f);
            GUI.color = factionDef.DefaultColor;
            widgetRow.Icon(factionDef.FactionIcon);
            GUI.color = Color.white;
            widgetRow.Gap(4f);
            Text.Anchor = TextAnchor.MiddleCenter;
            widgetRow.Label(factionDef.LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;

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
