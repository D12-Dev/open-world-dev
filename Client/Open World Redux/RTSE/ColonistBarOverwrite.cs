using HarmonyLib;
using RimWorld.Planet;
using RimWorld;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using Verse;
using System.Linq;
using Multiplayer;
using Steamworks;
using OpenWorldRedux;
using System;
using Verse.AI;
using System.Reflection.Emit;
using Multiplayer;

namespace OpenWorldRedux.RTSE
{



    [HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
    public static class NoDraftButtonPatch
    {
        public static string concatedstring;
        static void Postfix(ref IEnumerable<Gizmo> __result, Pawn_DraftController __instance)
        {
            if (!ShouldRemoveDraftButton(__instance.pawn, ColonistBar_CheckRecacheEntries.savedlastcaravan.Split(':')))

            {
                List<Gizmo> gizmos = new List<Gizmo>(__result);
                Gizmo draftButton = gizmos.Find(g => g is Command_Toggle && ((Command_Toggle)g).defaultLabel == "Draft" ||  ((Command_Toggle)g).defaultLabel == "Undraft" );
                if (draftButton != null)
                {
                    gizmos.Remove(draftButton);
                }
                __result = gizmos;
            }
        }

        static bool ShouldRemoveDraftButton(Pawn pawn, string[] savedtostring)
        {
            if (Multiplayer.Client.Multiplayer.session == null)
            {
                return true;
            }


            concatedstring = "";

            if (savedtostring != null)
            {
                foreach (string s in savedtostring)
                {
                    if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                    {
                        concatedstring += int.Parse(s.Split('|')[5]) + ",";
                    }
                    else if (s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                    {
                        concatedstring += int.Parse(s.Split('|')[4]) + ",";
                    }
                }

            }

            if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
            {
                if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }




    [HarmonyPatch(typeof(MainTabWindow_PawnTable), "get_Pawns")]
    public static class Patch_MainTabWindow_PawnTable
    {
        public static string concatedstring;
        static void Postfix(ref IEnumerable<Pawn> __result)
        {
            List<Pawn> pawnList = new List<Pawn>(__result);
            pawnList.RemoveAll(ShouldHidePawn);
            __result = pawnList;
        }

        static bool ShouldHidePawn(Pawn pawn)
        {
            string savedtostring = ColonistBar_CheckRecacheEntries.savedlastcaravan;
            if (Multiplayer.Client.Multiplayer.session == null)
            {
                return false;
            }


            concatedstring = "";

            if (savedtostring != null)
            {
                foreach (string s in savedtostring.Split(':'))
                {
                    if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                    {
                        concatedstring += int.Parse(s.Split('|')[5]) + ",";
                    }
                    else if (s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                    {
                        concatedstring += int.Parse(s.Split('|')[4]) + ",";
                    }
                }

            }

            if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
            {
                if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }
    }


    [HarmonyPatch(typeof(MainButtonsRoot), "DoButtons")]
    public static class DoButtons_Patch
    {
        static void Prefix(ref List<MainButtonDef> ___allButtonsInOrder)
        {
            
            if (Multiplayer.Client.Multiplayer.session != null && !BooleanCache.ishostingrtseserver)
            {
                var researchButton = ___allButtonsInOrder.Find(def => def.defName == "Research");
                ___allButtonsInOrder.Remove(researchButton);
            }
        }
    }




    [HarmonyPatch(typeof(MainTabWindow_Animals), "get_Pawns")]
    public static class Patch_MainTabWindow_Animals
    {
        public static string concatedstring;
        static void Postfix(ref IEnumerable<Pawn> __result)
        {
            List<Pawn> pawnList = new List<Pawn>(__result);
            pawnList.RemoveAll(ShouldHidePawn);
            __result = pawnList;
        }

        static bool ShouldHidePawn(Pawn pawn)
        {
            string savedtostring = ColonistBar_CheckRecacheEntries.savedlastcaravan;
            if (Multiplayer.Client.Multiplayer.session == null)
            {
                return false;
            }


            concatedstring = "";

            if (savedtostring != null)
            {
                foreach (string s in savedtostring.Split(':'))
                {
                    if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                    {
                        concatedstring += int.Parse(s.Split('|')[5]) + ",";
                    }
                    else if (s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                    {
                        concatedstring += int.Parse(s.Split('|')[4]) + ",";
                    }
                }

            }

            if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
            {
                if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }
    }
}


[HarmonyPatch(typeof(FloatMenuMakerMap), "AddDraftedOrders")]
public static class FloatMenuMakerMap_Patch
{
    public static string concatedstring;
    static bool Prefix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
    {
        if (ShouldHidePawn(pawn))
        {
            return false;
        }

        return true;
    }

        static bool ShouldHidePawn(Pawn pawn)
        {
            string savedtostring = ColonistBar_CheckRecacheEntries.savedlastcaravan;
            if (Multiplayer.Client.Multiplayer.session == null)
            {
                return false;
            }


            concatedstring = "";

            if (savedtostring != null)
            {
                foreach (string s in savedtostring.Split(':'))
                {
                    if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                    {
                        concatedstring += int.Parse(s.Split('|')[5]) + ",";
                    }
                    else if (s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                    {
                        concatedstring += int.Parse(s.Split('|')[4]) + ",";
                    }
                }

            }

            if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
            {
                if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }
    
}




[HarmonyPatch(typeof(FloatMenuMakerMap), "AddJobGiverWorkOrders")]
public static class Patch_FloatMenuMakerMap
{
    public static string concatedstring;
    static void Postfix(ref List<FloatMenuOption> opts, Pawn pawn)
    {
        if (ShouldHidePawn(pawn))
        {
            // Remove all "Prioritize work" options from the float menu
            opts.RemoveAll(opt => true);
        }
    }

    static bool ShouldHidePawn(Pawn pawn)
    {
        string savedtostring = ColonistBar_CheckRecacheEntries.savedlastcaravan;
        if (Multiplayer.Client.Multiplayer.session == null)
        {
            return false;
        }


        concatedstring = "";

        if (savedtostring != null)
        {
            foreach (string s in savedtostring.Split(':'))
            {
                if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                {
                    concatedstring += int.Parse(s.Split('|')[5]) + ",";
                }
                else if (s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                {
                    concatedstring += int.Parse(s.Split('|')[4]) + ",";
                }
            }

        }

        if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
        {
            if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return true;
    }
}


[HarmonyPatch(typeof(MainTabWindow_Assign), "get_Pawns")]
public static class Patch_MainTabWindow_Assign
{
    public static string concatedstring;
    static void Postfix(ref IEnumerable<Pawn> __result)
    {
        List<Pawn> pawnList = new List<Pawn>(__result);
        //pawnList.RemoveAll(ShouldHidePawn);
        pawnList.RemoveAll(ShouldHidePawn);
        __result = pawnList;
    }

    static bool ShouldHidePawn(Pawn pawn)
    {
        string savedtostring = ColonistBar_CheckRecacheEntries.savedlastcaravan;
        if (Multiplayer.Client.Multiplayer.session == null)
        {
            return false;
        }


        concatedstring = "";

        if (savedtostring != null)
        {
            foreach (string s in savedtostring.Split(':'))
            {
                if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                {
                    concatedstring += int.Parse(s.Split('|')[5]) + ",";
                }
                else if (s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                {
                    concatedstring += int.Parse(s.Split('|')[4]) + ",";
                }
            }

        }

        if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
        {
            if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return true;
    }
}



[HarmonyPatch(typeof(ITab_Pawn_Gear), "CanControlColonist", MethodType.Getter)]
public static class ITab_Pawn_Gear_CanControlColonist_Patch
{
    public static string concatedstring;
    static bool Prefix(ITab_Pawn_Gear __instance, ref bool __result)
    {
        Pawn selPawnForGear = Traverse.Create(__instance).Property("SelPawnForGear").GetValue<Pawn>();
        if (ShouldHidePawn(selPawnForGear))
        {
            __result = false;
            return false; // Skip original method
        }
        return true; // Continue to original method
    }


    static bool ShouldHidePawn(Pawn pawn)
    {
        string savedtostring = ColonistBar_CheckRecacheEntries.savedlastcaravan;
        if (Multiplayer.Client.Multiplayer.session == null)
        {
            return false;
        }


        concatedstring = "";

        if (savedtostring != null)
        {
            foreach (string s in savedtostring.Split(':'))
            {
                if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                {
                    concatedstring += int.Parse(s.Split('|')[5]) + ",";
                }
                else if (s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                {
                    concatedstring += int.Parse(s.Split('|')[4]) + ",";
                }
            }

        }
        // Log.Message(concatedstring);

        if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
        {
            if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return true;
    }
}



[HarmonyPatch(typeof(ITab_Pawn_Gear), "ShouldShowInventory")]
public static class ITab_Pawn_Gear_ShouldShowInventory_Patch
{
    public static string concatedstring;
    static bool Prefix(Pawn p, ref bool __result)
    {
        if (ShouldHidePawn(p))
        {
            __result = false;
            return false; // Skip original method
        }
        return true; // Continue to original method
    }
    static bool ShouldHidePawn(Pawn pawn)
    {
        string savedtostring = ColonistBar_CheckRecacheEntries.savedlastcaravan;
        if (Multiplayer.Client.Multiplayer.session == null)
        {
            return false;
        }


        concatedstring = "";

        if (savedtostring != null)
        {
            foreach (string s in savedtostring.Split(':'))
            {
                if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                {
                    concatedstring += int.Parse(s.Split('|')[5]) + ",";
                }
                else if (s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                {
                    concatedstring += int.Parse(s.Split('|')[4]) + ",";
                }
            }

        }
        // Log.Message(concatedstring);

        if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
        {
            if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return true;
    }
}




[HarmonyPatch(typeof(ColonistBar))]
    [HarmonyPatch("CheckRecacheEntries")]
    public static class ColonistBar_CheckRecacheEntries
    {

        public static string savedlastcaravan = "";
        public static int mainstringsaved = -1;
        public static Caravan savedcaravan;
        public static string concatedstring;
        private static List<Pawn> Purge(List<Pawn> pawns)
        {
            if (Find.CurrentMap == null)
            {
                Log.Message("Current map not found.");
                return new List<Pawn>();
            }

            if (Find.CurrentMap.mapPawns == null)
            {
                Log.Message("Map pawns not found.");
                return new List<Pawn>();
            }
            Map currentMap = Find.CurrentMap;
            List<Pawn> filteredPawns = new List<Pawn>();
            savedcaravan = FocusCache.focusedCaravan;

            foreach (Pawn pawn in currentMap.mapPawns.AllPawns)
            {
                if (pawn != null && pawn.IsColonist && ShouldAddPawn(pawn, savedlastcaravan.Split(':')))
                {
                    filteredPawns.Add(pawn);
                }
            }
            return filteredPawns;
        }

        public static bool ShouldAddPawn(Pawn pawn, string[] savedtostring)
        {

            if (Multiplayer.Client.Multiplayer.session == null)
            {
                return true;
            }


            concatedstring = "";

            if (savedtostring != null)
            {
                foreach (string s in savedtostring)
                {
                    if (s.Split('|').Length > 6 && s.Split('|')[6] == "Pawn")
                    {
                        concatedstring += int.Parse(s.Split('|')[5]) + ",";
                    }
                    else if(s.Split('|').Length > 4 && s.Split('|')[0] != "Human")
                    {
                        concatedstring += int.Parse(s.Split('|')[4]) + ",";
                    }
                }

            }

            if (BooleanCache.isConnectedToServer && BooleanCache.ishostingrtseserver)
            {
                if (!concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (concatedstring.Contains(pawn.thingIDNumber.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }





        public static bool Prefix(ColonistBar __instance, ref bool ___entriesDirty, ref List<ColonistBar.Entry> ___cachedEntries, ref List<Map> ___tmpMaps, ref List<Pawn> ___tmpPawns, ref List<Caravan> ___tmpCaravans, ref List<int> ___cachedReorderableGroups, ref ColonistBarDrawLocsFinder ___drawLocsFinder, ref List<Vector2> ___cachedDrawLocs, ref float ___cachedScale)
        {
            if (!___entriesDirty)
            {
                return false;
            }
            ___entriesDirty = false;
            ___cachedEntries.Clear();
            int num = 0;
            if (Find.PlaySettings.showColonistBar)
            {
                ___tmpMaps.Clear();
                ___tmpMaps.AddRange(Find.Maps);
                ___tmpMaps.SortBy((Map x) => !x.IsPlayerHome, (Map x) => x.uniqueID);
                for (int i = 0; i < ___tmpMaps.Count; i++)
                {
                    ___tmpPawns.Clear();



                    ___tmpPawns.AddRange(Purge(___tmpMaps[i].mapPawns.FreeColonists));



                    List<Thing> list = ___tmpMaps[i].listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (!list[j].IsDessicated())
                        {
                            Pawn innerPawn = ((Corpse)list[j]).InnerPawn;
                            if (innerPawn != null && innerPawn.IsColonist)
                            {
                                ___tmpPawns.Add(innerPawn);
                            }
                        }
                    }
                    List<Pawn> allPawnsSpawned = ___tmpMaps[i].mapPawns.AllPawnsSpawned;
                    for (int k = 0; k < allPawnsSpawned.Count; k++)
                    {
                        if (allPawnsSpawned[k].carryTracker.CarriedThing is Corpse corpse && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist)
                        {
                            ___tmpPawns.Add(corpse.InnerPawn);
                        }
                    }
                    PlayerPawnsDisplayOrderUtility.Sort(___tmpPawns);
                    for (int l = 0; l < ___tmpPawns.Count; l++)
                    {
                        ___cachedEntries.Add(new ColonistBar.Entry(___tmpPawns[l], ___tmpMaps[i], num));
                    }
                    if (!___tmpPawns.Any())
                    {
                        ___cachedEntries.Add(new ColonistBar.Entry(null, ___tmpMaps[i], num));
                    }
                    num++;
                }
                ___tmpCaravans.Clear();
                ___tmpCaravans.AddRange(Find.WorldObjects.Caravans);
                ___tmpCaravans.SortBy((Caravan x) => x.ID);
                for (int m = 0; m < ___tmpCaravans.Count; m++)
                {
                    if (!___tmpCaravans[m].IsPlayerControlled)
                    {
                        continue;
                    }
                    ___tmpPawns.Clear();
                    ___tmpPawns.AddRange(___tmpCaravans[m].PawnsListForReading);
                    PlayerPawnsDisplayOrderUtility.Sort(___tmpPawns);
                    for (int n = 0; n < ___tmpPawns.Count; n++)
                    {
                        if (___tmpPawns[n].IsColonist)
                        {
                            ___cachedEntries.Add(new ColonistBar.Entry(___tmpPawns[n], null, num));
                        }
                    }
                    num++;
                }
            }
            ___cachedReorderableGroups.Clear();
            foreach (ColonistBar.Entry cachedEntry in ___cachedEntries)
            {
                _ = cachedEntry;
                ___cachedReorderableGroups.Add(-1);
            }
            __instance.drawer.Notify_RecachedEntries();
            ___tmpPawns.Clear();
            ___tmpMaps.Clear();
            ___tmpCaravans.Clear();
            ___drawLocsFinder.CalculateDrawLocs(___cachedDrawLocs, out ___cachedScale, num);

            return false;
        }
    }