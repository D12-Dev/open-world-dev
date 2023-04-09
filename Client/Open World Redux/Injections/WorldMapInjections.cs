using HarmonyLib;
using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using Verse;
using Verse.Profile;
using Verse.AI;

namespace OpenWorldRedux
{
    public static class WorldMapInjections
    {
        //Modify Back Button At World Screen
        [HarmonyPatch(typeof(Page_SelectStartingSite), "DoCustomBottomButtons")]
        public static class DisconnectWhenBack
        {
            [HarmonyPrefix]
            public static bool ModifyPre(Page_SelectStartingSite __instance)
            {
                if (!BooleanCache.isConnectedToServer) return true;
                else
                {
                    if (!BooleanCache.worldSaved)
                    {
                        if (!BooleanCache.worldGenerated)
                        {

                            BooleanCache.isSaving = true;
                            GameDataSaveLoader.SaveGame("worldTemplate");
                            BooleanCache.worldGenerated = true;
                            BooleanCache.isSaving = false;
                        }

                        if (!BooleanCache.isSaving && BooleanCache.worldGenerated && !BooleanCache.worldSaved)
                        {
                            string filename = "worldTemplate";
                            string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + filename + ".rws";
                            string newFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + filename + ".zipx";
                            File.WriteAllBytes(newFilePath, SaveHandler.Zip(File.ReadAllBytes(filePath)));
                            string[] contents = new string[] { Convert.ToBase64String(File.ReadAllBytes(newFilePath)) };
                            Packet ClientSaveFilePacket = new Packet("RecieveBaseSaveFromClient", contents);
                            Network.SendData(ClientSaveFilePacket);
                            BooleanCache.worldSaved = true;
                            Log.Message("World Saved!");
                        }
                    }

                    

                    int num = TutorSystem.TutorialMode ? 4 : 5;
                    int num2 = (num < 4 || !((float)UI.screenWidth < 540f + (float)num * (150f + 10f))) ? 1 : 2;
                    int num3 = Mathf.CeilToInt((float)num / (float)num2);
                    float num4 = 150f * (float)num3 + 10f * (float)(num3 + 1);
                    float num5 = (float)num2 * 38f + 10f * (float)(num2 + 1);
                    Rect rect = new Rect(((float)UI.screenWidth - num4) / 2f, (float)UI.screenHeight - num5 - 4f, num4, num5);

                    WorldInspectPane worldInspectPane = Find.WindowStack.WindowOfType<WorldInspectPane>();
                    if (worldInspectPane != null && rect.x < InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f)
                    {
                        rect.x = InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f;
                    }

                    Widgets.DrawWindowBackground(rect);
                    float num6 = rect.xMin + 10f;
                    float num7 = rect.yMin + 10f;
                    Text.Font = GameFont.Small;

                    if (Widgets.ButtonText(new Rect(num6, num7, 150f, 38f), "Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent)
                    {
                        __instance.Close();
                        Network.DisconnectFromServer();
                    }

                    return true;
                }
            }
        }

        //Add Buttons To Planet View Side Panel
        [HarmonyPatch(typeof(WorldInspectPane), "SetInitialSizeAndPosition")]
        public static class AddFindButton
        {
            [HarmonyPrefix]
            public static bool ModifyPre(ref WITab[] ___TileTabs)
            {

                if (___TileTabs.Count() == 4) return true;
                else
                {
                    if (BooleanCache.isConnectedToServer)
                    {

                        ___TileTabs = new WITab[4]
                        {
                            new MP_WITabOnlinePlayers(),
                            new MP_WITabSettlementList(),
                            new WITab_Terrain(),
                            new WITab_Planet()
                        };
                    }

                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(Page_SelectStartingSite), "PreOpen")]
        public static class GetServerWorldDetails
        {
            [HarmonyPrefix]
            public static bool ModifyPre()
            {
                if (!BooleanCache.isGeneratingNewWorld) return true;
                else
                {
                    string[] contents = new string[]
                    {
                        Current.Game.World.info.seedString.ToString(),
                        Current.Game.World.info.planetCoverage.ToString(),
                        ((float)Current.Game.World.info.overallRainfall).ToString(),
                        ((float)Current.Game.World.info.overallTemperature).ToString(),
                        ((float)Current.Game.World.info.overallPopulation).ToString(),
                        ((float)Current.Game.World.info.pollution).ToString()
                    };

                    Packet NewServerDataPacket = new Packet("NewServerDataPacket", contents);
                    Network.SendData(NewServerDataPacket);
                    return true;
                }
            }
        }
    }
}
