/*using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public class OW_MPSpecialThanks : Window
    {
        public override Vector2 InitialSize => new Vector2(900f, 700f);

        private Vector2 scrollPosition = Vector2.zero;

        private float buttonX = 30f;
        private float buttonY = 30f;

        private string windowTitle = "SPECIAL THANKS";
        private string windowDescription = "Here is a list of each and every wonderful person that has heavily contributed to the mod";

        private string patreonsListTitle = " ";
        private string[] patreons = new string[]
        {
            "Our precious supporters",
            "",
            "Andrew",
            "Susp3kt",
            "Pallist Horror",
            "_gab5_",
            "Enviromentally unfriendly human",
            "Justin Lucke",
            "Westley Conner",
            "Void Runner",
            "",
            "",
            "",
            "Our developers and contributors",
            "",
            "Saffue - Owner and Moderator",
            "Generic - Github Contributor",
            "D12 - Github Contributor",
            "primate prime - Github Contributor",
            "Warbrain - Artist",
            "",
            "",
            "",
            "The Founding Fathers",
            "",
            "Eragon - Ex Moderator",
            "BoostHungry - Ex Github Contributor",
            "DarkIrata - Ex Github Contributor",
            "ZephyrWarrior - Ex Github Contributor",
            "Napstablook - Ex Moderator",
            "Ishiveki - Ex Moderator",
            "",
            "",
            "",
            "Best buddies that could be asked for",
            "",
            "General Tao & Not_Normal_Jesus - Together, they single-handedly holded the community at the beginning",
            "Arota - Trading is pretty much possible only thanks to this kind man",
            "Telebread - Helping out everyone even while lurking",
            "Lagz - An absolute unit of a boss",
            "Aant - The only reason this project even started",
            "Xyvern - Even with the pea incident, helped out a ton regarding bugs",
            "Secret Friend - Helped me maintain my sanity when developing the RTSE alpha",
            "bagel - Hosted events to keep an active community"
        };

        public OW_MPSpecialThanks()
        {
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

            float horizontalLineDif = windowDescriptionDif + StandardMargin;

            float list1TitleDif = horizontalLineDif + StandardMargin;
            float list1Dif = list1TitleDif + StandardMargin;

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMin), new Vector2(buttonX, buttonY)), "X"))
            {
                Close();
            }

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(patreonsListTitle).x / 2, list1TitleDif, Text.CalcSize(patreonsListTitle).x, Text.CalcSize(patreonsListTitle).y), patreonsListTitle);
            Rect rect1 = new Rect(rect.x, list1Dif, rect.width, 520f);
            GenerateList(rect1, patreons);
        }

        private void GenerateList(Rect mainRect, string[] list)
        {
            float height = 6f + (float)list.Count() * 21f;

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            int index = 0;

            var orderedList = list.OrderBy(x => x[0]);

            foreach (string str in orderedList)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 21f);
                    DrawCustomRow(rect, str, index);
                    index++;
                }

                num += 21f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string name, int index)
        {
            Text.Font = GameFont.Small;
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 36f, rect.height));

            Widgets.Label(fixedRect, name);
        }
    }
}*/





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
    public class OW_MPSpecialThanks : Window
    {
        public override Vector2 InitialSize => new Vector2(900f, 700f);

        private Vector2 scrollPosition = Vector2.zero;

        private float buttonX = 30f;
        private float buttonY = 30f;

        private string windowTitle = "SPECIAL THANKS";
        private string windowDescription = "Here is a list of each and every wonderful person that has heavily contributed to the mod";

        private string patreonsListTitle = "";
        private string[] patreons = new string[]
        {
            "Our precious supporters",
            "",
            "Andrew",
            "Susp3kt",
            "Pallist Horror",
            "gab5",
            "Enviromentally unfriendly human",
            "Justin Lucke",
            "Westley Conner",
            "Void Runner",
            "",
            "",
            "",
            "Our developers and contributors",
            "",
            "Saffue - Owner",
            "Generic - Github Contributor",
            "D12 - Github Contributor",
            "primate prime - Github Contributor",
            "Warbrain - Artist",
            "",
            "",
            "",
            "The Founding Fathers",
            "",
            "Eragon - Ex Moderator",
            "Lollipop - Ex Owner",
            "BoostHungry - Ex Github Contributor",
            "DarkIrata - Ex Github Contributor",
            "ZephyrWarrior - Ex Github Contributor",
            "Napstablook - Ex Moderator",
            "Ishiveki - Ex Moderator",

        };



        private string lastMessageTitle = "And to every player that has ever tried out the mod. Thank you.";

        public OW_MPSpecialThanks()
        {
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

            float horizontalLineDif = windowDescriptionDif + StandardMargin;

            float list1TitleDif = horizontalLineDif + StandardMargin;
            float list1Dif = list1TitleDif + StandardMargin;

            float list2TitleDif = list1Dif + StandardMargin + 160f;
            float list2Dif = list2TitleDif + StandardMargin;

            float list3TitleDif = list2Dif + StandardMargin + 160f;
            float list3Dif = list3TitleDif + StandardMargin;

            float lastMessageDif = list3Dif + StandardMargin + 150f;

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMin), new Vector2(buttonX, buttonY)), "X"))
            {
                Close();
            }

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(patreonsListTitle).x / 2, list1TitleDif, Text.CalcSize(patreonsListTitle).x, Text.CalcSize(patreonsListTitle).y), patreonsListTitle);
            Rect rect1 = new Rect(rect.x, list1Dif, rect.width, 520f);
            GenerateList(rect1, patreons);


            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(lastMessageTitle).x / 2, lastMessageDif, Text.CalcSize(lastMessageTitle).x, Text.CalcSize(lastMessageTitle).y), lastMessageTitle);
        }

        private void GenerateList(Rect mainRect, string[] list)
        {
            float height = 6f + (float)list.Count() * 21f;

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            int index = 0;

            //var orderedList = list.OrderBy(x => x[0]);

            foreach (string str in list)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 21f);
                    DrawCustomRow(rect, str, index);
                    index++;
                }

                num += 21f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string name, int index)
        {
            Text.Font = GameFont.Small;
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 36f, rect.height));
            

            Widgets.Label(fixedRect, name);
        }
    }
}

