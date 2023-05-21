using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldRedux
{
    class EnforcementInjections
    {
		//DisableDevOptions
		[HarmonyPatch(typeof(Dialog_Options), "DoWindowContents")]
		public static class PreventOptions
		{
			[HarmonyPostfix]
			public static void ModifyPost()
			{
                if (!BooleanCache.isConnectedToServer) return;
                else RimworldHandler.ToggleDevOptions();
			}
		}

		//Enforce Difficulty Tweaks
		[HarmonyPatch(typeof(Page_SelectStorytellerInGame), "DoWindowContents")]
		public static class EnforceDifficultyTweaks
		{
			[HarmonyPostfix]
			public static void ModifyPost()
			{
				if (!BooleanCache.isConnectedToServer) return;
                else RimworldHandler.EnforceDificultyTweaks();
            }
		}
	}
}
