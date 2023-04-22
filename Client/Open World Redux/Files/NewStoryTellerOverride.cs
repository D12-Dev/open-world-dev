using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using Verse.Profile;
using HarmonyLib;


namespace OpenWorldRedux
{
    public class NewSelectStoryTellerOverride
    {


        public static void NewStoryTellerPage()
		{
         //   Log.Message("ran new storyteller page");


			if (BooleanCache.newPlayer == true)
			{














                //System.Threading.Thread.Sleep(4000);

				//Log.Message("Is running!");
				BooleanCache.newPlayer = false;
				//Current.Game.Scenario = WorldCache.scenario; // Set the scenario to the current selected one
				//Log.Message(WorldCache.scenario.ToString());
                //Current.Game.storyteller = WorldCache.StoryTeller;
                //Current.Game.Scenario.PreConfigure(); // Sets the scene for the scenario to take place
                Page_CustomStoryteller newSelectStoryTeller = new Page_CustomStoryteller();

               // newSelectStoryTeller.nextAct = NewStartingSiteOverride.NewStartingSite;
                //newSelectStoryTeller.next = null;
              //  Log.Message("Set Next Act to new starting site!");















                Page_CustomStartingSite newSelectStartingSite = new Page_CustomStartingSite();

                

             
               
                Page_ConfigureStartingPawns newConfigureStartingPawns = new Page_ConfigureStartingPawns();
                //newSelectStartingSite.next = newConfigureStartingPawns;
                if (ModsConfig.IdeologyActive)
                {
                    Page_ChooseIdeoPreset newChooseIdeoPreset = new Page_ChooseIdeoPreset();
                    newSelectStartingSite.next = newChooseIdeoPreset;
                    newChooseIdeoPreset.prev = newSelectStartingSite;
                    newChooseIdeoPreset.next = newConfigureStartingPawns;
                    //newChooseIdeoPreset.nextAct = newConfigureStartingPawns;
                }
                else
                {
                    newConfigureStartingPawns.prev = newSelectStartingSite;
                    newSelectStartingSite.next = newConfigureStartingPawns;
                    Find.Scenario.PostIdeoChosen();
                }
                newConfigureStartingPawns.nextAct = PageUtility.InitGameStart;




                newSelectStoryTeller.next = newSelectStartingSite;











                //base.nextAct = NewStartingSiteOverride.NewStartingSite; 





                // MemoryUtility.UnloadUnusedUnityAssets();
                //Find.World.renderer.RegenerateAllLayersNow();
             //   Log.Message("Trying to boot up StoryTeller Page...");
				Find.WindowStack.Add(newSelectStoryTeller);
			//	Log.Message("Should have loaded StoryTeller Page.");
			}
		}
	}




    public sealed class Page_CustomStoryteller : Page
    {
        private StorytellerDef storyteller;

        private DifficultyDef difficulty;

        private Difficulty difficultyValues = new Difficulty();

        private Listing_Standard selectedStorytellerInfoListing = new Listing_Standard();

        public override string PageTitle => "ChooseAIStoryteller".Translate();


/*        public Page_CustomStoryteller()
        {
            Log.Message("I also ran this page storyteller");
            absorbInputAroundWindow = false;
            shadowAlpha = 0f;
            preventCameraMotion = false;
        }*/
        public override void PreOpen()
        {
           // Log.Message("SENT HEREEEEE");
            base.PreOpen();
            if (storyteller == null)
            {
                storyteller = (from d in DefDatabase<StorytellerDef>.AllDefs
                               where d.listVisible
                               orderby d.listOrder
                               select d).First();
            }
            StorytellerUI.ResetStorytellerSelectionInterface();

            Find.GameInitData.permadeathChosen = true;
            Find.GameInitData.permadeath = true;
        }

        public override void DoWindowContents(Rect rect)
        {
            DrawPageTitle(rect);
            StorytellerUI.DrawStorytellerSelectionInterface(GetMainRect(rect), ref storyteller, ref difficulty, ref difficultyValues, selectedStorytellerInfoListing);
            DoBottomButtons(rect);
            Rect rect2 = new Rect(rect.xMax - Page.BottomButSize.x - 200f - 6f, rect.yMax - Page.BottomButSize.y, 200f, Page.BottomButSize.y);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect2, "CanChangeStorytellerSettingsDuringPlay".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
        }

        [Obsolete]
        private void OpenDifficultyUnlockConfirmation()
        {
        }

        protected override bool CanDoNext()
        {
            if (!base.CanDoNext())
            {
                return false;
            }
           // Find.GameInitData.permadeathChosen = true;
          //  Find.GameInitData.permadeath = true;
            if (difficulty == null)
            {
                if (!Prefs.DevMode)
                {
                    Messages.Message("MustChooseDifficulty".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                    return false;
                }
                Messages.Message("Difficulty has been automatically selected (debug mode only)", MessageTypeDefOf.SilentInput, historical: false);
                difficulty = DifficultyDefOf.Rough;
                difficultyValues = new Difficulty(difficulty);
            }
            if (!Find.GameInitData.permadeathChosen)
            {
                if (!Prefs.DevMode)
                {
                    Messages.Message("MustChoosePermadeath".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                    return false;
                }
                Messages.Message("Reload anytime mode has been automatically selected (debug mode only)", MessageTypeDefOf.SilentInput, historical: false);
                Find.GameInitData.permadeathChosen = true;
                Find.GameInitData.permadeath = false;
            }
            Current.Game.storyteller = new Storyteller(storyteller, difficulty, difficultyValues);
            return true;
        }
        }
    }
