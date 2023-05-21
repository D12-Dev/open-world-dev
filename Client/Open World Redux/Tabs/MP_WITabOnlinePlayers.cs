﻿using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public class MP_WITabOnlinePlayers : WITab
    {
        private Vector2 scrollPosition;

        private static readonly Vector2 WinSize = new Vector2(432f, 540f);

        public override bool IsVisible => true;

        private string titleText;

        public MP_WITabOnlinePlayers()
        {
            size = WinSize;
            labelKey = "List";
        }

        protected override void FillTab()
        {
            if (!BooleanCache.isConnectedToServer) return;
            else
            {
                titleText = $"Online Players [{FocusCache.playerCount}]";

                Rect outRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
                Rect rect = new Rect(10f, 10f, outRect.width - 16f, Mathf.Max(0f, outRect.height));

                float horizontalLineDif = Text.CalcSize(titleText).y + 3f + 10f;

                Text.Font = GameFont.Medium;
                Widgets.Label(rect, titleText);
                Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

                GenerateList(new Rect(new Vector2(rect.x, rect.y + 30f), new Vector2(rect.width, rect.height - 30f)));
            }
        }

        private void GenerateList(Rect mainRect)
        {
            List<string> orderedList = FocusCache.playerList;
            orderedList.Sort();

            float height = 6f + (float)orderedList.Count() * 30f;
            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            foreach (string str in orderedList)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 30f);
                    DrawCustomRow(rect, str, num4);
                }

                num += 30f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string str, int index)
        {
            Text.Font = GameFont.Small;

            if (index % 2 == 0) Widgets.DrawLightHighlight(rect);
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 52f, rect.height));

            if (str.Length > 1) str = char.ToUpper(str[0]) + str.Substring(1);
            else str = str.ToUpper();

            Widgets.Label(fixedRect, str);
        }
    }
}