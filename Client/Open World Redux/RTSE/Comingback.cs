using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux.RTSE
{
    public static class Comingback
    {
        public static bool iscomingbackfromsettlement = false;
        public static List<Pawn> caravanPawns;
        public static List<Thing> caravanitems;
        public static void Comingbackfromsettlement()
        {

            BooleanCache.ishostingrtseserver = false;
            Log.Message("Is coming back fired!");
            Log.Message("Saved last caravan: " + ColonistBar_CheckRecacheEntries.savedlastcaravan);

            if (ColonistBar_CheckRecacheEntries.savedlastcaravan is null) return;
            if (ColonistBar_CheckRecacheEntries.savedcaravan is null) return;
            caravanPawns = CreatePawnListFromStrings(ColonistBar_CheckRecacheEntries.savedlastcaravan.Split(':'));
            caravanitems = Createitemlistfromstrings(ColonistBar_CheckRecacheEntries.savedlastcaravan.Split(':'));
            Caravan newcaravan = CaravanMaker.MakeCaravan(caravanPawns, Faction.OfPlayer, ColonistBar_CheckRecacheEntries.savedcaravan.Tile, true);
            foreach(Thing item in caravanitems)
            {
                newcaravan.AddPawnOrItem(item, true);
            }
            Multiplayer.Client.Multiplayer.session = null;
            iscomingbackfromsettlement = false;
            caravanitems = null;
            caravanPawns = null;

            // this may cause an issue. Keep an eye on it.
            //ColonistBar_CheckRecacheEntries.savedlastcaravan = null;
            //

            ColonistBar_CheckRecacheEntries.savedcaravan = null;

            FactionsCache.FindOnlineFactionsInWorld();

            WorldHandler.PrepareWorld();

            WorldHandler.TryPatchOldWorlds();

            RimworldHandler.ToggleDevOptions();

            RimworldHandler.EnforceDificultyTweaks();


            return;
        }

        public static List<Pawn> CreatePawnListFromStrings(string[] pawnStrings)
        {
            List<Pawn> pawnList = new List<Pawn>();

            foreach (string str in pawnStrings)
            {
                if (string.IsNullOrWhiteSpace(str)) continue;
                string itemDefName = str.Split('|')[0];
                if (itemDefName == "Human" || str.Split('|').Length > 6 && str.Split('|')[6] == "Pawn")
                {
                    Pawn newPawn = OnVisitAccept.stringtopawn(str);
                    //Log.Message("18");
                    pawnList.Add(newPawn);
                }
            }

            return pawnList;
        }

        public static List<Thing> Createitemlistfromstrings(string[] itemStrings) {

            List<Thing> itemList = new List<Thing>();

            foreach (string str in itemStrings)
            {
                if (string.IsNullOrWhiteSpace(str)) continue;
                string itemDefName = str.Split('|')[0];
                if (itemDefName != "Human" && str.Split('|').Length > 6 && str.Split('|')[6] != "Pawn")
                {
                    Thing newitem = OnVisitAccept.stringtoitem(str);
                    //Log.Message("18");
                    itemList.Add(newitem);
                }
            }

            return itemList;
        }


    }

    }
