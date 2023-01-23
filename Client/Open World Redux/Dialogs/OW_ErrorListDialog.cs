using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public class OW_ErrorListDialog : Window
    {
        public override Vector2 InitialSize => new Vector2(400f, 299f);
        private Vector2 scrollPosition = Vector2.zero;

        private string windowTitle = "ERROR";
        private string windowDescription = "";

        private float buttonX = 150f;
        private float buttonY = 38f;

        string[] listContents;

        public OW_ErrorListDialog(string windowDescription, string[] listContents)
        {
            this.windowDescription = windowDescription;
            this.listContents = listContents;

            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;
            float windowDescriptionDif = Text.CalcSize(windowDescription).y + (StandardMargin / 2);
            float listingDif = windowDescriptionDif + StandardMargin + 6f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            FillMainRect(new Rect(new Vector2(rect.x, listingDif), new Vector2(rect.width, rect.yMax - listingDif - buttonY - StandardMargin)));

            if (Widgets.ButtonText(new Rect(new Vector2(centeredX - buttonX / 2, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "OK"))
            {
                OnAccept();
            }
        }

        private void FillMainRect(Rect mainRect)
        {
            float objectSize = 23f;
            float height = 6f + listContents.Count() * objectSize;
            Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);
            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);
            float num = 0;
            float num2 = scrollPosition.y - objectSize;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            for (int i = 0; i < listContents.Count(); i++)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, num, viewRect.width, objectSize);
                    DrawCustomRow(rect, listContents[i], num4);
                }

                num += objectSize;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string name, int index)
        {
            if (index % 2 == 0) Widgets.DrawHighlight(rect);

            Text.Font = GameFont.Small;
            Widgets.Label(rect, name);
            Text.Font = GameFont.Medium;
        }
        
        private void OnAccept()
        {
            Close();
        }
    }
}
