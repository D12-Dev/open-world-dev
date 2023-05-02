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
using static OpenWorldRedux.NewStartingSiteOverride;

namespace OpenWorldRedux
{
    public class NewStartingSiteOverride
    {


        public static void NewStartingSite()
		{



			if (BooleanCache.newPlayer == true)
			{














               // System.Threading.Thread.Sleep(4000);

				//Log.Message("Is running starting site!");
				BooleanCache.newPlayer = false;
				//Current.Game.Scenario = WorldCache.scenario; // Set the scenario to the current selected one
				//Log.Message(WorldCache.scenario.ToString());
                //Current.Game.storyteller = WorldCache.StoryTeller;
                //Current.Game.Scenario.PreConfigure(); // Sets the scene for the scenario to take place
                Page_CustomStartingSite newSelectStartingSite = new Page_CustomStartingSite();
				Page_ChooseIdeoPreset newChooseIdeoPreset = new Page_ChooseIdeoPreset();
				Page_ConfigureStartingPawns newConfigureStartingPawns = new Page_ConfigureStartingPawns();
				//newSelectStartingSite.next = newConfigureStartingPawns;
				if (ModsConfig.IdeologyActive) {
                    newSelectStartingSite.next = newChooseIdeoPreset;
                    newChooseIdeoPreset.prev = newSelectStartingSite;
                    newChooseIdeoPreset.next = newConfigureStartingPawns;
					//newChooseIdeoPreset.nextAct = newConfigureStartingPawns;
                } 
				else {
					newConfigureStartingPawns.prev = newSelectStartingSite;
                    newSelectStartingSite.next = newConfigureStartingPawns;
                    
                }
                newConfigureStartingPawns.nextAct = PageUtility.InitGameStart;









                /*MemoryUtility.UnloadUnusedUnityAssets();
                Find.World.renderer.RegenerateAllLayersNow();*/
              //  Log.Message("Trying to boot up Starting Site...");
				Find.WindowStack.Add(newSelectStartingSite);
				//Log.Message("Should have loaded starting site.");
			}
		}
	}




	public sealed class Page_CustomStartingSite : Page
	{
		public static int OverrideStartingTile = -1;

		private const float GapBetweenBottomButtons = 10f;
		private static int pawnCount = 1;

        private const float UseTwoRowsIfScreenWidthBelowBase = 540f;

		private static List<Vector3> tmpTileVertices = new List<Vector3>();

		private int? tutorialStartTilePatch;
		public override string PageTitle => "SelectStartingSite".TranslateWithBackup("SelectLandingSite");

		public override Vector2 InitialSize => Vector2.zero;

		protected override float Margin => 0f;

		public Page_CustomStartingSite()
		{
			absorbInputAroundWindow = false;
			shadowAlpha = 0f;
			preventCameraMotion = false;
/*			GameInitData initData = new GameInitData
            {
                playerFaction = Find.FactionManager.OfPlayer
            };*/
            //Current.Game.InitData = initData;
            //Current.Game.Scenario.PreConfigure();
           
			//Log.Message("SCEN PARTS:");
/*			foreach (ScenPart X in Current.Game.Scenario.AllParts) {
				Log.Message(X.ToString());
                

            }*/
           //Current.Game.InitData.startingPawnCount = (scenPart as ScenPart_ConfigPage_ConfigureStartingPawns).pawnCount;
           // Log.Message("END OF SCEN PARTS");
            ScenPart scenPart = Current.Game.Scenario.AllParts.Where((ScenPart p) => p is ScenPart_ConfigPage_ConfigureStartingPawns).FirstOrDefault();
            ScenPart scenPart2 = Current.Game.Scenario.AllParts.Where((ScenPart p) => p is ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs).FirstOrDefault();
			int PawnCount = 0;
            //Log.Message("Pawn Counts:");
			try
			{
			//	Log.Message((scenPart as ScenPart_ConfigPage_ConfigureStartingPawns).pawnCount.ToString());
                PawnCount = PawnCount + (scenPart as ScenPart_ConfigPage_ConfigureStartingPawns).pawnCount;
            }
			catch (Exception) {
				//Log.Message("No Reg Pawns");
			}
			try
			{
				foreach (PawnKindCount X in (scenPart2 as ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs).kindCounts) {
					//Log.Message("Running Marathon");
					//Log.Message(X.ToString());
					//Log.Message(X.count.ToString());
					PawnCount = PawnCount + X.count;


                }
				//Log.Message((scenPart2 as ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs).kindCounts.ToString());
			}
			catch (Exception ex) {
               // Log.Message("No Mech Pawns");
				//Log.Message(ex.ToString());
            }
            try
            {
                foreach (XenotypeCount X in (scenPart2 as ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes).xenotypeCounts)
                {
               //     Log.Message("Running Marathon");
                    //Log.Message(X.ToString());
                 //   Log.Message(X.count.ToString());

                }
                 //Log.Message((scenPart2 as ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes).xenotypeCounts.ToString());
            }
            catch (Exception)
            {
            //    Log.Message("No Xeno Pawns");
            }
          //  Log.Message("END OF Pawn Counts");
			//Log.Message(PawnCount.ToString());
			pawnCount = PawnCount;
            //Current.Game.InitData.
            // GenerateStartingPawns();
            
        }

        private static void GenerateStartingPawns()
        {
            int num = 0;
            do
            {
                StartingPawnUtility.ClearAllStartingPawns();
                for (int i = 0; i < pawnCount; i++)
                {
                    StartingPawnUtility.AddNewPawn();
                }
                num++;
            }
            while (num <= 20 && !StartingPawnUtility.WorkTypeRequirementsSatisfied());
        }

        public override void PreOpen()
		{
			OverrideStartingTile = -1;

		}

		public override void PostOpen()
		{

		//	Log.Message("Page ran!");
			base.PostOpen();
			Find.GameInitData.ChooseRandomStartingTile();
			LessonAutoActivator.TeachOpportunity(ConceptDefOf.WorldCameraMovement, OpportunityType.Important);
			TutorSystem.Notify_Event("PageStart-SelectStartingSite");
			tutorialStartTilePatch = null;
			if (!TutorSystem.TutorialMode || Find.Tutor.activeLesson == null || Find.Tutor.activeLesson.Current == null || Find.Tutor.activeLesson.Current.Instruction != InstructionDefOf.ChooseLandingSite)
			{
				return;
			}
			Find.WorldCameraDriver.ResetAltitude();
			Find.WorldCameraDriver.Update();
			List<int> list = new List<int>();
			float[] array = new float[Find.WorldGrid.TilesCount];
			WorldGrid worldGrid = Find.WorldGrid;
			Vector2 a = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
			float num = Vector2.Distance(a, Vector2.zero);
			for (int i = 0; i < worldGrid.TilesCount; i++)
			{
				Tile tile = worldGrid[i];
				if (TutorSystem.AllowAction("ChooseBiome-" + tile.biome.defName + "-" + tile.hilliness))
				{
					tmpTileVertices.Clear();
					worldGrid.GetTileVertices(i, tmpTileVertices);
					Vector3 zero = Vector3.zero;
					for (int j = 0; j < tmpTileVertices.Count; j++)
					{
						zero += tmpTileVertices[j];
					}
					zero /= (float)tmpTileVertices.Count;
					Vector3 vector = Find.WorldCamera.WorldToScreenPoint(zero) / Prefs.UIScale;
					vector.y = (float)UI.screenHeight - vector.y;
					vector.x = Mathf.Clamp(vector.x, 0f, UI.screenWidth);
					vector.y = Mathf.Clamp(vector.y, 0f, UI.screenHeight);
					float num2 = 1f - Vector2.Distance(a, vector) / num;
					Vector3 normalized = (zero - Find.WorldCamera.transform.position).normalized;
					float num3 = Vector3.Dot(Find.WorldCamera.transform.forward, normalized);
					array[i] = num2 * num3;
				}
				else
				{
					array[i] = float.NegativeInfinity;
				}
			}
			for (int k = 0; k < 16; k++)
			{
				for (int l = 0; l < array.Length; l++)
				{
					list.Clear();
					worldGrid.GetTileNeighbors(l, list);
					float num4 = array[l];
					if (num4 < 0f)
					{
						continue;
					}
					for (int m = 0; m < list.Count; m++)
					{
						float num5 = array[list[m]];
						if (!(num5 < 0f))
						{
							num4 += num5;
						}
					}
					array[l] = num4 / (float)list.Count;
				}
			}
			float num6 = float.NegativeInfinity;
			int num7 = -1;
			for (int n = 0; n < array.Length; n++)
			{
				if (array[n] > 0f && num6 < array[n])
				{
					num6 = array[n];
					num7 = n;
				}
			}
			if (num7 != -1)
			{
				tutorialStartTilePatch = num7;
			}
		}

		public override void PostClose()
		{
			base.PostClose();
			Find.World.renderer.wantedMode = WorldRenderMode.None;
		}

		public override void DoWindowContents(Rect rect)
		{
			if (Find.WorldInterface.SelectedTile >= 0)
			{
				//Log.Message("Trying to set Starting Tile:" + Find.GameInitData.startingTile);
				//Log.Message("to Selected Tile:" + Find.WorldInterface.SelectedTile);
				Find.GameInitData.startingTile = Find.WorldInterface.SelectedTile;
			}
			else if (Find.WorldSelector.FirstSelectedObject != null)
			{
				Find.GameInitData.startingTile = Find.WorldSelector.FirstSelectedObject.Tile;
			}
		}

		public override void ExtraOnGUI()
		{
			base.ExtraOnGUI();
			Text.Anchor = TextAnchor.UpperCenter;
			DrawPageTitle(new Rect(0f, 5f, UI.screenWidth, 300f));
			Text.Anchor = TextAnchor.UpperLeft;
			DoCustomBottomButtons();
			/*if (tutorialStartTilePatch.HasValue)
			{
				tmpTileVertices.Clear();
				Find.WorldGrid.GetTileVertices(tutorialStartTilePatch.Value, tmpTileVertices);
				Vector3 zero = Vector3.zero;
				for (int i = 0; i < tmpTileVertices.Count; i++)
				{
					zero += tmpTileVertices[i];
				}
				Color color = GUI.color;
				GUI.color = Color.white;
				GenUI.DrawArrowPointingAtWorldspace(zero / tmpTileVertices.Count, Find.WorldCamera);
				GUI.color = color;
			}*/
		}

		protected override bool CanDoNext()
		{
			//Log.Message("Running this can do next");
			if (!base.CanDoNext())
			{
				return false;
			}
			int selectedTile = Find.WorldInterface.SelectedTile;
			if (selectedTile < 0)
			{
				Messages.Message("MustSelectStartingSite".TranslateWithBackup("MustSelectLandingSite"), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!TileFinder.IsValidTileForNewSettlement(selectedTile, stringBuilder))
			{
              //  Log.Message("Is Invalid palce");
                Messages.Message(stringBuilder.ToString(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			Tile tile = Find.WorldGrid[selectedTile];
			if (!TutorSystem.AllowAction("ChooseBiome-" + tile.biome.defName + "-" + tile.hilliness))
			{
				return false;
			}
			return true;
		}

		protected override void DoNext()
		{
			//Log.Message("Do next running");
			if (!CanDoNext()){ return; };
			int selTile = Find.WorldInterface.SelectedTile;
			SettlementProximityGoodwillUtility.CheckConfirmSettle(selTile, delegate
			{
				OverrideStartingTile = selTile;
				PostClose();
				base.DoNext();

			});
		}


		private void DoCustomBottomButtons()
		{
			int num = ((!TutorSystem.TutorialMode) ? 5 : 4);
			int num2 = ((num < 4 || !((float)UI.screenWidth < 1340f)) ? 1 : 2);
			int num3 = Mathf.CeilToInt((float)num / (float)num2);
			float num4 = Page.BottomButSize.x * (float)num3 + 10f * (float)(num3 + 1);
			float num5 = num2;
			Vector2 bottomButSize = Page.BottomButSize;
			float num6 = num5 * bottomButSize.y + 10f * (float)(num2 + 1);
			Rect rect = new Rect(((float)UI.screenWidth - num4) / 2f, (float)UI.screenHeight - num6 - 4f, num4, num6);
			WorldInspectPane worldInspectPane = Find.WindowStack.WindowOfType<WorldInspectPane>();
			if (worldInspectPane != null && rect.x < InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f)
			{
				rect.x = InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f;
			}
			Widgets.DrawWindowBackground(rect);
			float num7 = rect.xMin + 10f;
			float num8 = rect.yMin + 10f;
			Text.Font = GameFont.Small;
			_ = Page.BottomButSize;
			_ = Page.BottomButSize;
			float num9 = num7;
			Vector2 bottomButSize2 = Page.BottomButSize;
			num7 = num9 + (bottomButSize2.x + 10f);
			if (!TutorSystem.TutorialMode)
			{
				float x = num7;
				float y = num8;
				float x2 = Page.BottomButSize.x;
				Vector2 bottomButSize3 = Page.BottomButSize;
				if (Widgets.ButtonText(new Rect(x, y, x2, bottomButSize3.y), "Advanced".Translate()))
				{
					Find.WindowStack.Add(new Dialog_AdvancedGameConfig(Find.WorldInterface.SelectedTile));
				}
				float num10 = num7;
				Vector2 bottomButSize4 = Page.BottomButSize;
				num7 = num10 + (bottomButSize4.x + 10f);
			}
			float x3 = num7;
			float y2 = num8;
			float x4 = Page.BottomButSize.x;
			Vector2 bottomButSize5 = Page.BottomButSize;
			if (Widgets.ButtonText(new Rect(x3, y2, x4, bottomButSize5.y), "SelectRandomSite".Translate()))
			{
				SoundDefOf.Click.PlayOneShotOnCamera();
				Find.WorldInterface.SelectedTile = TileFinder.RandomStartingTile();
				Find.WorldCameraDriver.JumpTo(Find.WorldGrid.GetTileCenter(Find.WorldInterface.SelectedTile));
			}
			float num11 = num7;
			Vector2 bottomButSize6 = Page.BottomButSize;
			num7 = num11 + (bottomButSize6.x + 10f);
			if (num2 == 2)
			{
				num7 = rect.xMin + 10f;
				float num12 = num8;
				Vector2 bottomButSize7 = Page.BottomButSize;
				num8 = num12 + (bottomButSize7.y + 10f);
			}
			float x5 = num7;
			float y3 = num8;
			float x6 = Page.BottomButSize.x;
			Vector2 bottomButSize8 = Page.BottomButSize;
			if (Widgets.ButtonText(new Rect(x5, y3, x6, bottomButSize8.y), "WorldFactionsTab".Translate()))
			{
				Find.WindowStack.Add(new Dialog_FactionDuringLanding());
			}
			float num13 = num7;
			Vector2 bottomButSize9 = Page.BottomButSize;
			num7 = num13 + (bottomButSize9.x + 10f);
			float x7 = num7;
			float y4 = num8;
			float x8 = Page.BottomButSize.x;
			Vector2 bottomButSize10 = Page.BottomButSize;
			if (Widgets.ButtonText(new Rect(x7, y4, x8, bottomButSize10.y), "Next".Translate()))
			{
				DoNext();
			}
			float num14 = num7;
			Vector2 bottomButSize11 = Page.BottomButSize;
			num7 = num14 + (bottomButSize11.x + 10f);
			GenUI.AbsorbClicksInRect(rect);
		}
	}
}
