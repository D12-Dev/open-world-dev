using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldRedux
{
    [HarmonyPatch(typeof(MainButtonsRoot))]
    [HarmonyPatch("MainButtonsOnGUI")]
    [HarmonyPriority(100)]
    public class OW_MainButtonsRoot_MainButtonsOnGUI
    {

        static bool Prefix(ScenPart_PlayerFaction __instance)
        {
            if (!BooleanCache.BottomBarVis)
            {
                return false;

            }
            return true;

        }

    }
}
