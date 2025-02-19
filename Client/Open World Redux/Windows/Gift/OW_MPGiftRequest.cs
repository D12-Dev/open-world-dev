﻿
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
    public class OW_MPGiftRequest : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 310f);

        private Vector2 scrollPosition = Vector2.zero;

        private float buttonX = 150f;
        private float buttonY = 38f;

        string windowTitle = "Gift Request";
        string silverText = "For: " + 0 + " silver";
        string senderTile;

        public OW_MPGiftRequest(string[] contents)
        {
            TradeCache.takeMode = TradeHandler.TakeMode.Gift;
            TradeCache.inTrade = true;

            List<string> tempContents = contents.ToList();
            senderTile = tempContents[0];
            tempContents.RemoveAt(0);

            foreach (string str in tempContents)
            {
                TradeItem newTradeItem = Serializer.SerializeToClass<TradeItem>(str);
                TradeCache.tradeItems.Add(newTradeItem);
            }

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;

            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = false;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float horizontalLineDif = Text.CalcSize(windowTitle).y + (StandardMargin / 4) + 3f;
            float silverDif = horizontalLineDif + StandardMargin / 2;
            float itemListDif = silverDif + StandardMargin + 10f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - (Text.CalcSize(windowTitle).x / 2), rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Widgets.Label(new Rect(centeredX - (Text.CalcSize(silverText).x / 2), silverDif, Text.CalcSize(silverText).x, Text.CalcSize(silverText).y), silverText);

            Text.Font = GameFont.Small;

            GenerateList(new Rect(rect.x, itemListDif, rect.width, 160f));

            Text.Font = GameFont.Medium;
            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Accept"))
            {
                OnAccept();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Reject"))
            {
                OnCancel();
            }
        }

        private void GenerateList(Rect mainRect)
        {
            float height = 6f + (float)TradeCache.tradeItems.Count() * 21f;

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            int index = 0;

            var orderedList = TradeCache.tradeItems.OrderBy(x => x.stackCount);

            foreach (TradeItem item in orderedList)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 21f);
                    DrawCustomRow(rect, item, index);
                    index++;
                }

                num += 21f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, TradeItem item, int index)
        {
            Text.Font = GameFont.Small;
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 36f, rect.height));
            if (index % 2 == 0) Widgets.DrawHighlight(fixedRect);

            string itemName = "NULL";
            foreach (ThingDef toCheck in DefDatabase<ThingDef>.AllDefs)
            {
                if (item.defName == toCheck.defName) itemName = toCheck.label;
            }

            if (itemName.Length > 1) itemName = char.ToUpper(itemName[0]) + itemName.Substring(1);
            else itemName = itemName.ToUpper();

            Widgets.Label(fixedRect, $"{itemName} (x{item.stackCount}) ({(QualityCategory)item.itemQuality})");
        }

        private void OnAccept()
        {
            TradeHandler.GetItemsIntoSettlement();
            Close();
        }

        private void OnCancel()
        {
            TradeCache.ResetTradeVariables();
            Close();
        }
    }
}
