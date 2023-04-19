using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Profile;
using static System.Net.Mime.MediaTypeNames;

namespace OpenWorldRedux
{
    public static class WorldGeneratorOverride
    {
        private static List<WorldGenStepDef> tmpGenSteps = new List<WorldGenStepDef>();

        public static IEnumerable<WorldGenStepDef> GenStepsInOrder => from x in DefDatabase<WorldGenStepDef>.AllDefs
                                                                      where x.defName != "Roads"
                                                                      orderby x.order, x.index
                                                                      select x;

        public static void TriggerWorldGeneration()
        {
            LongEventHandler.QueueLongEvent(delegate
            {
                Find.GameInitData.ResetWorldRelatedMapInitData();
               Current.Game.World = GenerateWorldFromScratch();
               LongEventHandler.ExecuteWhenFinished(delegate
                {
                    Page_SelectStartingSite newSelectStartingSite = new Page_SelectStartingSite();
                    Page_ChooseIdeoPreset newChooseIdeoPreset = new Page_ChooseIdeoPreset();
                    Page_ConfigureStartingPawns newConfigureStartingPawns = new Page_ConfigureStartingPawns();

                    if (ModsConfig.IdeologyActive) newSelectStartingSite.next = newChooseIdeoPreset;
                    else newSelectStartingSite.next = newConfigureStartingPawns;

                    if (ModsConfig.IdeologyActive)
                    {
                        newChooseIdeoPreset.prev = newSelectStartingSite;
                        newChooseIdeoPreset.next = newConfigureStartingPawns;
                    }
                    else newConfigureStartingPawns.prev = newSelectStartingSite;

                    newConfigureStartingPawns.nextAct = PageUtility.InitGameStart;

                    Find.WindowStack.Add(newSelectStartingSite);

                    MemoryUtility.UnloadUnusedUnityAssets();
                    Find.World.renderer.RegenerateAllLayersNow();
                });
            }, "GeneratingWorld", doAsynchronously: true, null);
        }

        public static World GenerateWorldFromScratch()
        {
            Rand.PushState();
            Rand.Seed = GenText.StableStringHash(WorldCache.seedString);
            if (BooleanCache.isGeneratingWorldFromPacket)
            {
                try
                {
                    Log.Message("Generating World From packet!");

                    Current.CreatingWorld = new World();
                    Current.CreatingWorld = new World();
                    Current.CreatingWorld.info.seedString = WorldCache.seedString;
                    Current.CreatingWorld.info.planetCoverage = WorldCache.planetCoverage;
                    Current.CreatingWorld.info.overallRainfall = WorldCache.overallRainfall;
                    Current.CreatingWorld.info.overallTemperature = WorldCache.overallTemperature;
                    Current.CreatingWorld.info.overallPopulation = WorldCache.overallPopulation;
                    Current.CreatingWorld.info.name = NameGenerator.GenerateName(RulePackDefOf.NamerWorld);
                    Current.CreatingWorld.info.factions = WorldCache.factions;
                    Current.CreatingWorld.info.pollution = WorldCache.pollution;

                    tmpGenSteps.Clear();
                    tmpGenSteps.AddRange(GenStepsInOrder);

                    for (int i = 0; i < tmpGenSteps.Count; i++)
                    {
                        try { tmpGenSteps[i].worldGenStep.GenerateFresh(WorldCache.seedString); }
                        catch (Exception ex) { Log.Error("Error in WorldGenStep: " + ex); }
                    }

                    Current.CreatingWorld.grid.StandardizeTileData();
                    Current.CreatingWorld.FinalizeInit();
                    Find.Scenario.PostWorldGenerate();
                    if (!ModsConfig.IdeologyActive) Find.Scenario.PostIdeoChosen();
                    return Current.CreatingWorld;
                }
                finally
                {
                    Rand.PopState();
                    Current.CreatingWorld = null;
                }
            }
            else
            {
                Log.Message("Generating World From Normal Generator!");
                try

                {

 




                    Current.CreatingWorld = new World();
                    Current.CreatingWorld.info.seedString = WorldCache.seedString;
                    Current.CreatingWorld.info.planetCoverage = WorldCache.planetCoverage;
                    Current.CreatingWorld.info.overallRainfall = WorldCache.overallRainfall;
                    Current.CreatingWorld.info.overallTemperature = WorldCache.overallTemperature;
                    Current.CreatingWorld.info.overallPopulation = WorldCache.overallPopulation;
                    Current.CreatingWorld.info.name = NameGenerator.GenerateName(RulePackDefOf.NamerWorld);
                    Current.CreatingWorld.info.factions = WorldCache.factions;
                    Current.CreatingWorld.info.pollution = WorldCache.pollution;

                    tmpGenSteps.Clear();
                    tmpGenSteps.AddRange(GenStepsInOrder);

                    for (int i = 0; i < tmpGenSteps.Count; i++)
                    {
                        try { tmpGenSteps[i].worldGenStep.GenerateFresh(WorldCache.seedString); }
                        catch (Exception ex) { Log.Error("Error in WorldGenStep: " + ex); }
                    }

                    Current.CreatingWorld.grid.StandardizeTileData();
                    Current.CreatingWorld.FinalizeInit();
                    Find.Scenario.PostWorldGenerate();
                    if (!ModsConfig.IdeologyActive) Find.Scenario.PostIdeoChosen();
                    return Current.CreatingWorld;
                }

                finally
                {
                    Rand.PopState();
                    Current.CreatingWorld = null;
                }
            }
        }
    }
}
