using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux
{
    public static class InGameMenuInjections
    {
        //Modify Save Button On ESC Menu
        [HarmonyPatch(typeof(MainMenuDrawer), "DoMainMenuControls")]
        public static class RenderStuffInEscMenu
        {
            [HarmonyPrefix]
            public static bool ModifyPre()
            {
                if (Current.ProgramState != ProgramState.Playing) return true;
                if (BooleanCache.isGeneratingWorldFromPacket) return true;
                if (!BooleanCache.isConnectedToServer) return true;
                else
                {
                    Vector2 buttonSize = new Vector2(170f, 45f);
                    if (Widgets.ButtonText(new Rect(0, (buttonSize.y + 7) * 2, buttonSize.x, buttonSize.y), ""))
                    {
                        GameDataSaveLoader.SaveGame(("Open World Server Save"));
                        Network.DisconnectFromServer();
                    }
                    Vector2 buttonSize2 = new Vector2(170f, 45f);
                    if (Widgets.ButtonText(new Rect(0, (buttonSize2.y + 58) * 2, buttonSize2.x, buttonSize2.y), "Reset Progress"))
                    {
                        ModStuff.ResetServerProgress();
                    }
                    return true;
                }
            }
        }
    }
}