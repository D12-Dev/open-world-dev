using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using OpenWorldRedux;
using RimWorld;
using UnityEngine;

namespace OpenWorldRedux
{
    [HarmonyPatch(typeof(ScenPart_GameStartDialog), "PostGameStart")]
    public static class AllowMenuVis
    {
        [HarmonyPostfix]
        public static void PostGameStart() {

            BooleanCache.BottomBarVis = true;
        }
    }

}
