using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.Sound;
using Verse;
using HarmonyLib;

namespace OpenWorldRedux
{
    [StaticConstructorOnStartup]
    public class Page_CustomSelectScenario : Page
    {
        private Scenario curScen;

        private Vector2 infoScrollPosition = Vector2.zero;

        private const float ScenarioEntryHeight = 68f;

        private static readonly Texture2D CanUploadIcon = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/CanUpload");

        private Vector2 scenariosScrollPosition = Vector2.zero;

        private float totalScenarioListHeight;

        public override string PageTitle => "ChooseScenario".Translate();


        ////////////// Remove Buttom Menu Buttons /////////////////

        

        /////////////////////////
        public override void PreOpen()
        {
            BooleanCache.BottomBarVis = false;
            base.PreOpen();
            infoScrollPosition = Vector2.zero;
            ScenarioLister.MarkDirty();
            EnsureValidSelection();

            RimworldHandler.ToggleDevOptions();
        }

        public override void DoWindowContents(Rect rect)
        {
            DrawPageTitle(rect);
            Rect mainRect = GetMainRect(rect);
            Widgets.BeginGroup(mainRect);
            Rect rect2 = new Rect(0f, 0f, mainRect.width * 0.35f, mainRect.height).Rounded();
            DoScenarioSelectionList(rect2);
            ScenarioUI.DrawScenarioInfo(new Rect(rect2.xMax + 17f, 0f, mainRect.width - rect2.width - 17f, mainRect.height).Rounded(), curScen, ref infoScrollPosition);
            Widgets.EndGroup();

            if (BooleanCache.isCustomScenariosAllowed)
            {

            }

            DoBottomButtons(rect, null, "ScenarioEditor".Translate(), GoToScenarioEditor);
        }

        private bool CanEditScenario(Scenario scen)
        {
            if (BooleanCache.isCustomScenariosAllowed)
            {
                if (scen.Category == ScenarioCategory.CustomLocal) return true;
                if (scen.CanToUploadToWorkshop()) return true;
                return false;
            }

            else return false;
        }

        private void GoToScenarioEditor()
        {
            if (BooleanCache.isCustomScenariosAllowed)
            {
                // Log.Message("opening scen editor");
                //Page_CustomScenarioEditor page_ScenarioEditor = new Page_CustomScenarioEditor(CanEditScenario(curScen) ? curScen : curScen.CopyForEditing());
                //page_ScenarioEditor.prev = this;
                //Find.WindowStack.Add(page_ScenarioEditor);
                Close();
            }

            else Find.WindowStack.Add(new OW_ErrorDialog("Custom scenarios are disabled in this server"));
        }

        private void DoScenarioSelectionList(Rect rect)
        {
            rect.xMax += 2f;
            Rect rect2 = new Rect(0f, 0f, rect.width - 16f - 2f, totalScenarioListHeight + 250f);
            Widgets.BeginScrollView(rect, ref scenariosScrollPosition, rect2);
            Rect rect3 = rect2.AtZero();
            rect3.height = 999999f;
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = rect2.width;
            listing_Standard.Begin(rect3);
            Text.Font = GameFont.Small;
            ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.FromDef));
            listing_Standard.Gap();

            if (BooleanCache.isCustomScenariosAllowed)
            {
                Text.Font = GameFont.Small;
                listing_Standard.Label("ScenariosCustom".Translate());
                ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.CustomLocal));
                listing_Standard.Gap();
                Text.Font = GameFont.Small;
                listing_Standard.Label("ScenariosSteamWorkshop".Translate());
                if (listing_Standard.ButtonText("OpenSteamWorkshop".Translate())) SteamUtility.OpenSteamWorkshopPage();
                ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.SteamWorkshop));
            }

            listing_Standard.End();
            totalScenarioListHeight = listing_Standard.CurHeight;
            Widgets.EndScrollView();
        }

        private void ListScenariosOnListing(Listing_Standard listing, IEnumerable<Scenario> scenarios)
        {
            bool flag = false;
            foreach (Scenario scenario in scenarios)
            {
                if (scenario.showInUI)
                {
                    if (flag)
                    {
                        listing.Gap(6f);
                    }

                    Scenario scen = scenario;
                    Rect rect = listing.GetRect(68f).ContractedBy(4f);
                    DoScenarioListEntry(rect, scen);
                    flag = true;
                }
            }

            if (!flag)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                listing.Label("(" + "NoneLower".Translate() + ")");
                GUI.color = Color.white;
            }
        }

        private void DoScenarioListEntry(Rect rect, Scenario scen)
        {
            bool flag = curScen == scen;
            Widgets.DrawOptionBackground(rect, flag);
            MouseoverSounds.DoRegion(rect);
            Rect rect2 = rect.ContractedBy(4f);
            Text.Font = GameFont.Small;
            Rect rect3 = rect2;
            rect3.height = Text.CalcHeight(scen.name, rect3.width);
            Widgets.Label(rect3, scen.name);
            Text.Font = GameFont.Tiny;
            Rect rect4 = rect2;
            rect4.yMin = rect3.yMax;
            if (!Text.TinyFontSupported)
            {
                rect4.yMin -= 6f;
                rect4.height += 6f;
            }

            Widgets.Label(rect4, scen.GetSummary());
            if (!scen.enabled)
            {
                return;
            }

            WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown);
            if (scen.Category == ScenarioCategory.CustomLocal && widgetRow.ButtonIcon(TexButton.DeleteX, "Delete".Translate(), GenUI.SubtleMouseoverColor))
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(scen.File.Name), delegate
                {
                    scen.File.Delete();
                    ScenarioLister.MarkDirty();
                }, destructive: true));
            }

            if (!flag && Widgets.ButtonInvisible(rect))
            {
                curScen = scen;
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
        }

        protected override bool CanDoNext()
        {
            if (!base.CanDoNext())
            {
                return false;
            }

            if (curScen == null)
            {
                return false;
            }

            BeginScenarioConfiguration(curScen, this);
            return true;
        }

        public static void BeginScenarioConfiguration(Scenario scen, Page originPage)
        {
            if (BooleanCache.isGeneratingNewWorld == true)
            {
                Current.Game = new Game();
                Current.Game.InitData = new GameInitData();
            }
            else
            {

                /*                foreach (ScenPart scenpart in scen.AllParts) {
                                    Log.Message(scenpart.));

                                }*/

                // Log.Message(Find.FactionManager.OfPlayer.ToString());
                Current.Game.InitData = new GameInitData()
                {
                    playerFaction = Find.FactionManager.OfPlayer
                };







            }
            Log.Message("Begin Scen Config");

            Current.Game.Scenario = scen;
            WorldCache.scenario = scen;
            Current.Game.Scenario.PreConfigure();
            Find.GameInitData.startedFromEntry = true;
            Log.Message("mid through Scen Config");
            Page firstConfigPage = Current.Game.Scenario.GetFirstConfigPage();
            //  Log.Message("Running this code");
            if (firstConfigPage == null)
            {
                Log.Message("Starting Game from scen?");
                PageUtility.InitGameStart();
                return;
            }
            Log.Message(firstConfigPage.ToString()); /// StoryTeller


            if (BooleanCache.isGeneratingNewWorld == true)
            {
                Log.Message("Is genning new world");
                originPage.next = firstConfigPage;
                firstConfigPage.prev = originPage;
            }
            else
            {
                try
                {
                    //originPage.next = originPage;
                    Log.Message("Not genning new world");
                    originPage.next = null;
                    originPage.nextAct = NewSelectStoryTellerOverride.NewStoryTellerPage; //NewSelectStoryTellerOverride.NewStoryTellerPage;
                    //originPage.Close();
                    //newSelectStoryTeller.prev = originPage;
                    //Log.Message(newSelectStoryTeller.ToString()); /// StoryTeller Custom
                }
                catch (Exception err)
                {
                    Log.Message(err.ToString());
                }
            }


        }

        private void EnsureValidSelection()
        {
            if (curScen == null || !ScenarioLister.ScenarioIsListedAnywhere(curScen))
            {
                curScen = ScenarioLister.ScenariosInCategory(ScenarioCategory.FromDef).FirstOrDefault();
            }
        }
    }


}































/*    [StaticConstructorOnStartup]
    public class SelectScenarioOverride : Page
    {
        private Scenario curScen;

        private Vector2 infoScrollPosition = Vector2.zero;

        private const float ScenarioEntryHeight = 68f;

        private static readonly Texture2D CanUploadIcon = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/CanUpload");

        private Vector2 scenariosScrollPosition = Vector2.zero;

        private float totalScenarioListHeight;

        public override string PageTitle => "ChooseScenario".Translate();

        public override void PreOpen()
        {
            base.PreOpen();
            infoScrollPosition = Vector2.zero;
            ScenarioLister.MarkDirty();
            EnsureValidSelection();

            RimworldHandler.ToggleDevOptions();
        }

        public override void DoWindowContents(Rect rect)
        {
            DrawPageTitle(rect);
            Rect mainRect = GetMainRect(rect);
            Widgets.BeginGroup(mainRect);
            Rect rect2 = new Rect(0f, 0f, mainRect.width * 0.35f, mainRect.height).Rounded();
            DoScenarioSelectionList(rect2);
            ScenarioUI.DrawScenarioInfo(new Rect(rect2.xMax + 17f, 0f, mainRect.width - rect2.width - 17f, mainRect.height).Rounded(), curScen, ref infoScrollPosition);
            Widgets.EndGroup();

            if (BooleanCache.isCustomScenariosAllowed)
            {

            }

            DoBottomButtons(rect, null, "ScenarioEditor".Translate(), GoToScenarioEditor);
        }

        private bool CanEditScenario(Scenario scen)
        {
            if (BooleanCache.isCustomScenariosAllowed)
            {
                if (scen.Category == ScenarioCategory.CustomLocal) return true;
                if (scen.CanToUploadToWorkshop()) return true;
                return false;
            }

            else return false;
        }

        private void GoToScenarioEditor()
        {
            if (BooleanCache.isCustomScenariosAllowed)
            {
                Page_ScenarioEditor page_ScenarioEditor = new Page_ScenarioEditor(CanEditScenario(curScen) ? curScen : curScen.CopyForEditing());
                page_ScenarioEditor.prev = this;
                Find.WindowStack.Add(page_ScenarioEditor);
                Close();
            }

            else Find.WindowStack.Add(new OW_ErrorDialog("Custom scenarios are disabled in this server"));
        }

        private void DoScenarioSelectionList(Rect rect)
        {
            rect.xMax += 2f;
            Rect rect2 = new Rect(0f, 0f, rect.width - 16f - 2f, totalScenarioListHeight + 250f);
            Widgets.BeginScrollView(rect, ref scenariosScrollPosition, rect2);
            Rect rect3 = rect2.AtZero();
            rect3.height = 999999f;
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = rect2.width;
            listing_Standard.Begin(rect3);
            Text.Font = GameFont.Small;
            ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.FromDef));
            listing_Standard.Gap();

            if (BooleanCache.isCustomScenariosAllowed)
            {
                Text.Font = GameFont.Small;
                listing_Standard.Label("ScenariosCustom".Translate());
                ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.CustomLocal));
                listing_Standard.Gap();
                Text.Font = GameFont.Small;
                listing_Standard.Label("ScenariosSteamWorkshop".Translate());
                if (listing_Standard.ButtonText("OpenSteamWorkshop".Translate())) SteamUtility.OpenSteamWorkshopPage();
                ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.SteamWorkshop));
            }

            listing_Standard.End();
            totalScenarioListHeight = listing_Standard.CurHeight;
            Widgets.EndScrollView();
        }

        private void ListScenariosOnListing(Listing_Standard listing, IEnumerable<Scenario> scenarios)
        {
            bool flag = false;
            foreach (Scenario scenario in scenarios)
            {
                if (scenario.showInUI)
                {
                    if (flag)
                    {
                        listing.Gap(6f);
                    }

                    Scenario scen = scenario;
                    Rect rect = listing.GetRect(68f).ContractedBy(4f);
                    DoScenarioListEntry(rect, scen);
                    flag = true;
                }
            }

            if (!flag)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                listing.Label("(" + "NoneLower".Translate() + ")");
                GUI.color = Color.white;
            }
        }

        private void DoScenarioListEntry(Rect rect, Scenario scen)
        {
            bool flag = curScen == scen;
            Widgets.DrawOptionBackground(rect, flag);
            MouseoverSounds.DoRegion(rect);
            Rect rect2 = rect.ContractedBy(4f);
            Text.Font = GameFont.Small;
            Rect rect3 = rect2;
            rect3.height = Text.CalcHeight(scen.name, rect3.width);
            Widgets.Label(rect3, scen.name);
            Text.Font = GameFont.Tiny;
            Rect rect4 = rect2;
            rect4.yMin = rect3.yMax;
            if (!Text.TinyFontSupported)
            {
                rect4.yMin -= 6f;
                rect4.height += 6f;
            }

            Widgets.Label(rect4, scen.GetSummary());
            if (!scen.enabled)
            {
                return;
            }

            WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown);
            if (scen.Category == ScenarioCategory.CustomLocal && widgetRow.ButtonIcon(TexButton.DeleteX, "Delete".Translate(), GenUI.SubtleMouseoverColor))
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(scen.File.Name), delegate
                {
                    scen.File.Delete();
                    ScenarioLister.MarkDirty();
                }, destructive: true));
            }

            if (!flag && Widgets.ButtonInvisible(rect))
            {
                curScen = scen;
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
        }

        protected override bool CanDoNext()
        {
            if (!base.CanDoNext())
            {
                return false;
            }

            if (curScen == null)
            {
                return false;
            }

            BeginScenarioConfiguration(curScen, this);
            return true;
        }
        public static void DoNext(Page originPage) {
            originPage.Close();


        }
        public static void BeginScenarioConfiguration(Scenario scen, Page originPage)
        {
            Current.Game = new Game();
            Current.Game.InitData = new GameInitData();
            Current.Game.Scenario = scen;
            WorldCache.scenario = scen;
            Current.Game.Scenario.PreConfigure();
            //Find.GameInitData.startedFromEntry = true;
            Page firstConfigPage = Current.Game.Scenario.GetFirstConfigPage();
            if (firstConfigPage == null)
            {
                Log.Message("Starting Game from scen?");
                PageUtility.InitGameStart();
                return;
            }
            Log.Message(firstConfigPage.ToString()); /// StoryTeller


            if (BooleanCache.isGeneratingNewWorld == true)
            {
                originPage.next = firstConfigPage;
                firstConfigPage.prev = originPage;
            }
            else {
                try
                {
                    originPage.next = null;
                    originPage.nextAct = NewSelectStoryTellerOverride.NewStoryTellerPage;
                    //originPage.Close();
                    //newSelectStoryTeller.prev = originPage;
                    //Log.Message(newSelectStoryTeller.ToString()); /// StoryTeller Custom
                }
                catch(Exception err) {
                    Log.Message(err.ToString());
                }
            }


        }

        private void EnsureValidSelection()
        {
            if (curScen == null || !ScenarioLister.ScenarioIsListedAnywhere(curScen))
            {
                curScen = ScenarioLister.ScenariosInCategory(ScenarioCategory.FromDef).FirstOrDefault();
            }
        }
    }*/
