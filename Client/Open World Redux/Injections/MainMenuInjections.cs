using HarmonyLib;
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
    public static class MainMenuInjections
    {
        //Render Multiplayer Button
        [HarmonyPatch(typeof(MainMenuDrawer), "DoMainMenuControls")]
        public static class InjectMultiplayerButton
        {
            [HarmonyPrefix]
            public static bool ModifyPre(Rect rect)
            {
                if (!(Current.ProgramState == ProgramState.Entry)) return true;
                else
                {
                    Vector2 buttonLocation = new Vector2(rect.x, rect.y);
                    Vector2 buttonSize = new Vector2(170f, 45f);
                    if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), ""))
                    {
                        Find.WindowStack.Add(new OW_MPConnect());
                    }

                    return true;
                }
            }

            [HarmonyPostfix]
            public static void ModifyPost(Rect rect)
            {
                if (!(Current.ProgramState == ProgramState.Entry)) return;
                else
                {
                    Vector2 buttonLocation = new Vector2(rect.x, rect.y);
                    Vector2 buttonSize = new Vector2(170f, 45f);
                    if (Widgets.ButtonText(new Rect(buttonLocation.x, buttonLocation.y, buttonSize.x, buttonSize.y), "Multiplayer"))
                    {
                        //Do nothing since it's a dummy
                    }
                }
            }
        }
    }
}
