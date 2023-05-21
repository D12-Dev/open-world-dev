
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
    public class OW_MPFactionMenu : Window
    {
        public override Vector2 InitialSize => FactionCache.hasFaction ? new Vector2(525f, 305f) : new Vector2(525f, 225f);

        private int startAcceptingInputAtFrame;
        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        private Vector2 scrollPosition = Vector2.zero;

        private string windowTitle = "";
        private string windowDescription = "Manage your faction from here";

        private float buttonX = 150f;
        private float buttonY = 38f;

        private bool inCreateWindow;
        private string createFactionName;

        private bool inOptionsWindow;

        private bool inMembersWindow;

        public OW_MPFactionMenu()
        {
            FocusCache.factionMenuInstance = this;

            if (FactionCache.hasFaction) windowTitle = $"Faction Menu - {FactionCache.factionName}";
            else windowTitle = "Faction Menu";

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

            if (inCreateWindow)
            {
                string line1 = "Name Your Faction";

                Widgets.Label(new Rect(centeredX - Text.CalcSize(line1).x / 2, rect.y + 60, Text.CalcSize(line1).x, Text.CalcSize(line1).y), line1);

                string nameField = Widgets.TextField(new Rect(centeredX - 137.5f, rect.y + 95, 275f, 30f), createFactionName);
                if (AcceptsInput && nameField.Length <= 24 && nameField.All(character => Char.IsLetterOrDigit(character) || character == ' ')) createFactionName = nameField;

                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMin, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Create"))
                {
                    if (string.IsNullOrWhiteSpace(createFactionName))
                    {
                        Find.WindowStack.Add(new OW_ErrorDialog("Faction name must not be empty"));
                    }

                    else
                    {
                        string[] contents = new string[] { createFactionName };
                        Packet CreateFactionPacket = new Packet("CreateFactionPacket", contents);
                        Network.SendData(CreateFactionPacket);

                        Close();
                    }
                }

                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Cancel"))
                {
                    inCreateWindow = false;
                }
            }

            else if (inOptionsWindow)
            {
                windowDescription = "Available options for your faction";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX * 2 / 2), rect.y + 90), new Vector2(buttonX * 2, buttonY)), "Leave Faction"))
                {
                    Find.WindowStack.Add(new OW_MPFactionLeave());
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX * 2 / 2), rect.y + 135), new Vector2(buttonX * 2, buttonY)), "Disband Faction"))
                {
                    Find.WindowStack.Add(new OW_MPFactionDisband());
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Back"))
                {
                    inOptionsWindow = false;
                }
            }

            else if (inMembersWindow)
            {
                windowDescription = $"Your faction member count: [{FactionCache.factionMembers.Count}]";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                GenerateList(new Rect(new Vector2(centeredX - rect.width / 2, rect.y + 85), new Vector2(rect.width, rect.height - (buttonY * 3 + (buttonY / 2)))));

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Back"))
                {
                    inMembersWindow = false;
                }
            }

            else if (!FactionCache.hasFaction)
            {
                string line1 = "You are not a member of any faction";
                string line2 = "Create or join one to use this menu";

                Widgets.Label(new Rect(centeredX - Text.CalcSize(line1).x / 2, rect.y + 50, Text.CalcSize(line1).x, Text.CalcSize(line1).y), line1);
                Widgets.Label(new Rect(centeredX - Text.CalcSize(line2).x / 2, rect.y + 100, Text.CalcSize(line2).x, Text.CalcSize(line2).y), line2);

                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMin, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Create"))
                {
                    inCreateWindow = true;
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - (buttonX / 2), rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Refresh"))
                {
                    Packet RefreshFactionPacket = new Packet("RefreshFactionPacket");
                    Network.SendData(RefreshFactionPacket);
                }

                if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Close"))
                {
                    Close();
                }
            }

            else
            {
                string windowDescription = "Manage your faction from here";

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);
                Text.Font = GameFont.Medium;

                Widgets.DrawLineHorizontal(rect.x, descriptionLineDif, rect.width);

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - ((buttonX * 3) / 2), descriptionLineDif + 10), new Vector2(buttonX * 3, buttonY)), "Members"))
                {
                    inMembersWindow = true;
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - ((buttonX * 3) / 2), descriptionLineDif + buttonY + 20f), new Vector2(buttonX * 3, buttonY)), "Options"))
                {
                    inOptionsWindow = true;
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - ((buttonX * 3) / 2), descriptionLineDif + buttonY * 2 + 30f), new Vector2(buttonX * 3, buttonY)), "Refresh"))
                {
                    Packet RefreshFactionPacket = new Packet("RefreshFactionPacket");
                    Network.SendData(RefreshFactionPacket);
                }

                if (Widgets.ButtonText(new Rect(new Vector2(centeredX - ((buttonX * 3) / 2), descriptionLineDif + buttonY * 3 + 40), new Vector2(buttonX * 3, buttonY)), "Close"))
                {
                    Close();
                }
            }
        }

        private void GenerateList(Rect mainRect)
        {
            float height = 6f;

            foreach (Tuple<string, FactionCache.MemberRank> member in FactionCache.factionMembers)
            {
                string displayName = $"[{member.Item2}] - {member.Item1}";
                height += Text.CalcHeight(displayName, mainRect.width) + 6f;
            }

            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            int index = 0;
            foreach (Tuple<string, FactionCache.MemberRank> member in FactionCache.factionMembers)
            {
                string displayName = $"[{member.Item2}] - {member.Item1}";

                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, Text.CalcHeight(displayName, mainRect.width));
                    DrawCustomRow(rect, displayName, GetIconForRole(member.Item2), index);
                }

                num += Text.CalcHeight(displayName, mainRect.width) + 6f;
                num4++;
                index++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, string message, Texture2D icon, int index)
        {
            Text.Font = GameFont.Medium;
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 36f, rect.height));
            if (index % 2 == 0) Widgets.DrawHighlight(fixedRect);
            Widgets.LabelWithIcon(fixedRect, message, icon);
        }

        private Texture2D GetIconForRole(FactionCache.MemberRank rank)
        {
            string iconPath = "";
            if (rank == FactionCache.MemberRank.Member) iconPath = "UI/Designators/Strip";
            else if (rank == FactionCache.MemberRank.Moderator) iconPath = "UI/Designators/Strip";
            else if (rank == FactionCache.MemberRank.Admin) iconPath = "UI/Designators/Claim";

            return ContentFinder<Texture2D>.Get(iconPath);
        }
    }
}