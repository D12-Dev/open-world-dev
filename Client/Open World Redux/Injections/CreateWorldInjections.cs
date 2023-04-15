using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux
{
    public static class CreateWorldInjections
    {
        //Scenario page
        [HarmonyPatch(typeof(Page_CustomSelectScenario), "DoWindowContents")]
        public static class ModifyScenarioPage
        {
            [HarmonyPrefix]
            public static bool ModifyPre(Rect rect, Page_CreateWorldParams __instance)
            {
                Vector2 buttonSize = new Vector2(150f, 38f);

                Vector2 buttonLocation = new Vector2(rect.xMin, rect.yMax - buttonSize.y);
                if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), ""))
                {
                    __instance.Close();
                    Network.DisconnectFromServer();
                }

                return true;
            }

            [HarmonyPostfix]
            public static void ModifyPost(Rect rect)
            {
                Vector2 buttonSize = new Vector2(150f, 38f);

                Text.Font = GameFont.Small;
                Vector2 buttonLocation = new Vector2(rect.xMin, rect.yMax - buttonSize.y);
                if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), "Disconnect"))
                {
                    //Do nothing since it's a dummy
                }
            }
        }

        //Storyteller page
       [HarmonyPatch(typeof(Page_SelectStoryteller), "DoWindowContents")]
        public static class ModifyStorytellerPage
        {
            [HarmonyPrefix]
            public static bool ModifyPre(Page_SelectStoryteller __instance)
            {
/*                if (BooleanCache.isGeneratingWorldFromPacket)
                {
                    Log.Message("Storyteller post fix applied");
                    __instance.next = null;
                    __instance.nextAct = NewStartingSiteOverride.NewStartingSite; // WorldHandler.SendWorldDataRequest; /// Do select site LoadFromWorldGen(Packet receivedPacket)
                    Find.GameInitData.permadeathChosen = true;
                    Find.GameInitData.permadeath = true;
                    return true;

                }*/

                if (BooleanCache.isGeneratingNewWorld)
                {
                    Find.GameInitData.permadeathChosen = true;
                    Find.GameInitData.permadeath = true;
                    return true;
                }

                else return true;
            }

            [HarmonyPostfix]
            public static void ModifyPost(Rect rect)
            {
                if (!BooleanCache.isGeneratingWorldFromPacket) return;
                else
                {
                    Vector2 buttonSize = new Vector2(150f, 38f);

                    Text.Font = GameFont.Small;
                    Vector2 buttonLocation = new Vector2(rect.xMax - buttonSize.x, rect.yMax - buttonSize.y);
                    if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), "Join"))
                    {
                        //Do nothing since it's a dummy
                    }
                }
            }
        }

        //World parameters page
        [HarmonyPatch(typeof(Page_CreateWorldParams), "DoWindowContents")]
        public static class ModifyWorldPage
        {
            [HarmonyPrefix]
            public static bool ModifyPre(Rect rect, Page_CreateWorldParams __instance, string ___seedString, float ___planetCoverage, OverallRainfall ___rainfall, OverallTemperature ___temperature, OverallPopulation ___population, float ___pollution)
            {
                if (!BooleanCache.isGeneratingNewWorld) return true;
                else
                {
                    Vector2 buttonSize = new Vector2(150f, 38f);

                    Vector2 buttonLocation = new Vector2(rect.xMax - buttonSize.x, rect.yMax - buttonSize.y);
                    if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), ""))
                    {
                        WorldCache.planetCoverage = ___planetCoverage;
                        WorldCache.seedString = ___seedString;
                        WorldCache.overallRainfall = ___rainfall;
                        WorldCache.overallTemperature = ___temperature;
                        WorldCache.overallPopulation= ___population;
                        WorldCache.pollution = ___pollution;

                        WorldGeneratorOverride.TriggerWorldGeneration();

                        __instance.Close();
                    }

                    return true;
                }
            }

            [HarmonyPostfix]
            public static void ModifyPost(Rect rect)
            {
                if (!BooleanCache.isGeneratingNewWorld) return;
                else
                {
                    Vector2 buttonSize = new Vector2(150f, 38f);

                    Vector2 buttonLocation = new Vector2(rect.xMax - buttonSize.x, rect.yMax - buttonSize.y);
                    if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), "Generate"))
                    {
                        //Do nothing since it's a dummy
                    }
                }
            }
        }

        

        //World factions page UI
       [HarmonyPatch(typeof(WorldFactionsUIUtility), "DoWindowContents")]
        public static class ModifyWorldFactionsUI
        {
            
            [HarmonyPrefix]
            public static bool ModifyPre(Rect rect)
            {
                if (!BooleanCache.isGeneratingNewWorld) return true;
                else
                {

                    //IEnumerable<Faction> allfactionsava = AllFactionsVisible;
                    //List<FactionDef> AllFac = DefDatabase<FactionDef>.AllDefs.ToList(); /// Gets all faction Defs




                    /*Log.Message("Factions Being Reset");*/

                    /*List<FactionDef> VisFac = new List<FactionDef>();
                    foreach (FactionDef X in AllFac)
                    {
                        if (!X.hidden)
                        {
                            VisFac.Add(X);
                        }
                    }*/

                    // FactionManager.
                    WorldFactionUIOverride.DoWindowContents(rect, WorldCache.factions); // Parses all current visable factions to the ui override.
                    return false;
                }
            }
        }
    }
}
