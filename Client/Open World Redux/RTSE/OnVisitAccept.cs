using HarmonyLib;
using Multiplayer.Common;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions.Must;
using Verse;
using Verse.Noise;

namespace OpenWorldRedux.RTSE
{
    public static class OnVisitAccept
    {
        public static Map map = Find.AnyPlayerHomeMap;
        public static IntVec3 positionToPlace = new IntVec3(map.Size.x, map.Center.y, map.Center.z);

        public static IntVec3 GetRandomPosition(Map map)
        {
            Log.Message("Get random called");
            IntVec3 positionToPlace = IntVec3.Invalid;
            Log.Message("1");
            for (int attempts = 0; attempts < 1000; attempts++)
            {
                Log.Message("Checking");
                int x, z;
                if (Rand.Bool) // Randomly choose between top/bottom edge or left/right edge
                {
                    x = Rand.Bool ? 0 : map.Size.x - 1; // If it's top/bottom edge, choose either top or bottom
                    z = Rand.RangeInclusive(0, map.Size.z - 1);
                }
                else
                {
                    x = Rand.RangeInclusive(0, map.Size.x - 1);
                    z = Rand.Bool ? 0 : map.Size.z - 1; // If it's left/right edge, choose either left or right
                }

                positionToPlace = new IntVec3(x, 0, z);

                if (positionToPlace.InBounds(map) &&
                    !positionToPlace.Fogged(map) &&
                    positionToPlace.Standable(map) &&
                    map.roofGrid.RoofAt(positionToPlace) != RoofDefOf.RoofRockThick)
                {
                    return positionToPlace;
                }
            }
            Log.Message("returning");
            return positionToPlace; // Note: this could still be invalid if no valid location was found in 1000 attempts
        }
        public static void OnvisitAccept(string caravanitemsandpawns)
        {
            positionToPlace = GetRandomPosition(map);
            if (!positionToPlace.IsValid)
            {
                Log.Error("Failed to find a valid position");
                return;
            }

            if (positionToPlace.Walkable(map) == false || map.roofGrid.RoofAt(positionToPlace) == RoofDefOf.RoofRockThick)
            {
                positionToPlace = new IntVec3(map.Center.x, map.Size.y, map.Center.z);
            }
            if(positionToPlace.Walkable(map) == false ||  map.roofGrid.RoofAt(positionToPlace) == RoofDefOf.RoofRockThick)
            {
                positionToPlace = new IntVec3(0, map.Center.y, map.Size.z);
            }
            if (positionToPlace.Walkable(map) == false || map.roofGrid.RoofAt(positionToPlace) == RoofDefOf.RoofRockThick)
            {
                positionToPlace = new IntVec3(map.Center.x, 0, map.Size.z);
            }
            try
            {
                Zone z = map.zoneManager.AllZones.Find(zone => zone.label == "Trading" && zone.GetType() == typeof(Zone_Stockpile));
                positionToPlace = z.Position;
            }
            catch { }
            foreach (String itemorpawn in caravanitemsandpawns.Split(':'))
            {
                Pawn newPawn = stringtopawn(itemorpawn);
                if(newPawn != null)
                {
                    try { GenPlace.TryPlaceThing(newPawn, positionToPlace, map, ThingPlaceMode.Near); }
                    catch (Exception e)
                    {
                        Log.Message(e.ToString());
                    }
                }
                else
                {
                    //Log.Message("Pawn is null.");
                }

            }
            
        }

        public static Pawn stringtopawn(string pawnstring)
        {
            Log.Message("String to pawn called: " + pawnstring);
            if (pawnstring.Split('|')[0] == "Human")
            {
                Pawn newPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);
                newPawn.Name = NameTriple.FromString(pawnstring.Split('|')[1]);
                int.TryParse(pawnstring.Split('|')[2], out int ageBiologicalTicks);
                int.TryParse(pawnstring.Split('|')[3], out int ageChronologicalTicks);
                newPawn.ageTracker.AgeBiologicalTicks = ageBiologicalTicks;
                newPawn.ageTracker.AgeChronologicalTicks = ageChronologicalTicks;
                string gender = pawnstring.Split('|')[4];
                if (Enum.TryParse(gender, true, out Gender finalGender))
                    newPawn.gender = finalGender;
                string[] backstories = pawnstring.Split('*')[1].Split('|');
                if (backstories.Length >= 2)
                {
                    string childhoodIdentifier = backstories[0];
                    string adulthoodIdentifier = backstories[1];
                    BackstoryDef childhoodbackstoryDef = DefDatabase<BackstoryDef>.GetNamed(childhoodIdentifier, false);
                    BackstoryDef adulthoodbackstoryDef = DefDatabase<BackstoryDef>.GetNamed(adulthoodIdentifier, false);

                    if (childhoodbackstoryDef != null)
                    {
                        newPawn.story.Childhood = childhoodbackstoryDef;

                    }
                    if (adulthoodbackstoryDef != null)
                    {
                        newPawn.story.Adulthood = adulthoodbackstoryDef;
                    }
                }

                if (pawnstring.Split('|')[6] != null && pawnstring.Split('|')[6] != "" && pawnstring.Split('|')[6] != " " && pawnstring.Split('|')[6] != "*")
                {
                    newPawn.genes.xenotypeName = pawnstring.Split('|')[6];
                }

                if (pawnstring.Split('|').Length > 8)
                {
                    if (pawnstring.Split('|')[8] != null && pawnstring.Split('|')[8] != "" && pawnstring.Split('|')[8] != " " && pawnstring.Split('|')[8] != "*")
                    {
                        newPawn.genes.xenotypeName = pawnstring.Split('|')[8];
                    }

                }

                if (pawnstring.Split('|')[7] != null)
                {
                    string favcolorString = pawnstring.Split('|')[7];

                    string[] favcolorComponents = favcolorString.Replace("RGBA(", "").Replace(")", "").Split(',');
                    if (favcolorComponents.Length == 4)
                    {
                        float r, g, b, a;
                        if (float.TryParse(favcolorComponents[0], out r) &&
                            float.TryParse(favcolorComponents[1], out g) &&
                            float.TryParse(favcolorComponents[2], out b) &&
                            float.TryParse(favcolorComponents[3], out a))
                        {
                            UnityEngine.Color favhairColor = new UnityEngine.Color(r, g, b, a);
                            newPawn.story.favoriteColor = favhairColor;
                        }
                    }
                }
                List<string> skillList = pawnstring.Split('*')[2].Split('|').ToList();
                for (int i = 0; i < 12; i++)
                {
                    int.TryParse(skillList[i].Split('-')[0], out int level);
                    int.TryParse(skillList[i].Split('-')[1], out int passion);

                    newPawn.skills.skills[i].levelInt = level;
                    newPawn.skills.skills[i].passion = (Passion)passion;

                }

                List<string> traitList = pawnstring.Split('*')[3].Split('|').ToList();
                newPawn.story.traits.allTraits.Clear();
                foreach (string traitS in traitList)
                {
                    bool traitFound = false;

                    foreach (TraitDef def in DefDatabase<TraitDef>.AllDefs)
                    {
                        foreach (TraitDegreeData degreeData in def.degreeDatas)
                        {

                            if (degreeData.label == traitS)
                            {
                                Trait trait = new Trait(def, degreeData.degree);
                                newPawn.story.traits.GainTrait(trait);

                                traitFound = true;
                                break;
                            }
                        }
                        if (traitFound)
                        {
                            break;
                        }
                    }
                }

                newPawn.health.RemoveAllHediffs();
                newPawn.health.Reset();
                foreach (string healthissues in pawnstring.Split('*')[5].Split('|'))
                {
                    string hediffDefName;
                    string bodyPartLabel;
                    float severity;
                    bool scar;
                    if (healthissues is null || healthissues == "" || healthissues == " ")
                    {
                        continue;
                    }

                    hediffDefName = healthissues.Split(';')[0];
                    severity = float.Parse(healthissues.Split(';')[2]);
                    HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed(hediffDefName);
                    bodyPartLabel = "null";
                    try
                    {
                        bodyPartLabel = healthissues.Split(';')[1];
                    }
                    catch
                    {
                        Log.Message("Error bruh");
                    }

                    BodyPartRecord bodyPart = bodyPartLabel == "null" ? null : newPawn.RaceProps.body.AllParts.FirstOrDefault(bp => bp.Label == bodyPartLabel);
                    Hediff hediff = HediffMaker.MakeHediff(hediffDef, newPawn, bodyPart);
                    hediff.Severity = severity;
                    bool.TryParse(healthissues.Split(';')[3], out scar);
                    try
                    {
                        if (scar)
                        {
                            HediffComp_GetsPermanent hediffComp = hediff.TryGetComp<HediffComp_GetsPermanent>();
                            hediffComp.IsPermanent = true;
                        }

                    }
                    catch
                    {
                        Log.Message("Error bruh");
                    }
                    newPawn.health.AddHediff(hediff);
                }

                newPawn.apparel.DestroyAll();
                newPawn.apparel.DropAllOrMoveAllToInventory();


                foreach (string apparel in pawnstring.Split('*')[6].Split('|'))
                {
                    if (apparel is null || apparel == "" || apparel == " " || apparel == "null")
                    {
                        continue;
                    }
                    string apparellabel = apparel.Split(';')[0];
                    string hitpoints = apparel.Split(';')[1];
                    if (Enum.TryParse(apparel.Split(';')[2], out RimWorld.QualityCategory quality))
                    {
                        if (bool.TryParse(apparel.Split(';')[3], out bool wornbycorpse))
                        {
                            ThingDef apparelDef = DefDatabase<ThingDef>.GetNamed(apparellabel);
                            newPawn.equipment.DestroyAllEquipment();
                            string stuffLabel = apparel.Split(';')[4];
                            ThingDef stuffDef = DefDatabase<ThingDef>.AllDefsListForReading.FirstOrDefault(def => def.IsStuff && def.defName == stuffLabel);
                            if (stuffDef != null)
                            {
                                Apparel newApparel = (Apparel)ThingMaker.MakeThing(apparelDef, stuffDef);
                                newApparel.HitPoints = int.Parse(hitpoints);
                                newApparel.TryGetComp<CompQuality>().SetQuality(quality, ArtGenerationContext.Outsider); ;
                                if (wornbycorpse)
                                {
                                    newApparel.WornByCorpse.MustBeTrue();
                                }
                                else
                                {
                                    newApparel.WornByCorpse.MustBeFalse();
                                }
                                // Equip the apparel
                                if (newPawn.apparel != null)
                                {
                                    newPawn.apparel.Wear(newApparel);
                                }
                                else
                                {
                                    Log.Message("Pawn has no apparel tracker.");
                                }
                            }


                            else
                            {
                                Log.Message($"Could not find stuff with label '{stuffLabel}'");
                                if (stuffLabel is null || stuffLabel == "")
                                {

                                }
                                Apparel newApparel = (Apparel)ThingMaker.MakeThing(apparelDef);
                                newApparel.HitPoints = int.Parse(hitpoints);
                                newApparel.TryGetComp<CompQuality>().SetQuality(quality, ArtGenerationContext.Outsider); ;
                                if (wornbycorpse)
                                {
                                    newApparel.WornByCorpse.MustBeTrue();
                                }
                                else
                                {
                                    newApparel.WornByCorpse.MustBeFalse();
                                }
                                // Equip the apparel
                                if (newPawn.apparel != null)
                                {
                                    newPawn.apparel.Wear(newApparel);
                                }
                                else
                                {
                                    Log.Message("Pawn has no apparel tracker.");
                                }
                            }
                        }
                    }
                }

                //Log.Message("Starting bruh");
                if (pawnstring.Split('*').Length >= 7)
                {
                    string weaponData = pawnstring.Split('*')[7];
                    Log.Message(weaponData);
                    if (weaponData.Split(';').Length > 2 && weaponData != "NoWeaponEquipped")
                    {
                        Log.Message("called1");
                        if (weaponData.Split(';')[0] == null || weaponData.Split(';')[0] == "" || weaponData.Split(';')[0] == " " || weaponData.Split(';')[1] == null || weaponData.Split(';')[1] == "" || weaponData.Split(';')[1] == " " || weaponData.Split(';')[2] == null || weaponData.Split(';')[2] == "" || weaponData.Split(';')[2] == " ")
                        {

                        }
                        else
                        {
                            string weapondef = weaponData.Split(';')[0];
                            string weaponhitpoints = weaponData.Split(';')[1];
                            if (Enum.TryParse(weaponData.Split(';')[2], out RimWorld.QualityCategory weaponquality))
                            {
                                ThingDef weaponDef = DefDatabase<ThingDef>.GetNamed(weapondef);
                                ThingWithComps newWeapon = (ThingWithComps)ThingMaker.MakeThing(weaponDef, null);
                                newWeapon.HitPoints = int.Parse(weaponhitpoints);
                                newWeapon.TryGetComp<CompQuality>().SetQuality(weaponquality, ArtGenerationContext.Outsider);
                                //Log.Message("called7");
                                newPawn.equipment.AddEquipment(newWeapon);
                            }
                        }

                    }
                }







                string hairDefIdentifier = pawnstring.Split('*')[4].Split('|')[0];
                string beardDefIdentifier = pawnstring.Split('*')[4].Split('|')[4];
                // Search for the HairDef in the game's database using the identifier
                RimWorld.HairDef hairDef = DefDatabase<RimWorld.HairDef>.GetNamed(hairDefIdentifier, true);
                newPawn.story.hairDef = hairDef;
                RimWorld.BeardDef beardDef = DefDatabase<RimWorld.BeardDef>.GetNamed(beardDefIdentifier, true);
                newPawn.style.beardDef = beardDef;
                //set the pawns beard.

                string colorString = pawnstring.Split('*')[4].Split('|')[1];
                string[] colorComponents = colorString.Replace("RGBA(", "").Replace(")", "").Split(',');
                if (colorComponents.Length == 4)
                {
                    float r, g, b, a;
                    if (float.TryParse(colorComponents[0], out r) &&
                        float.TryParse(colorComponents[1], out g) &&
                        float.TryParse(colorComponents[2], out b) &&
                        float.TryParse(colorComponents[3], out a))
                    {
                        UnityEngine.Color hairColor = new UnityEngine.Color(r, g, b, a);
                        newPawn.story.HairColor = hairColor;
                        newPawn.style.nextHairColor = hairColor;
                    }
                }
                string headDeftypeIdentifier = pawnstring.Split('*')[4].Split('|')[2];
                string skincolorString = pawnstring.Split('*')[4].Split('|')[3];
                string[] skincolorComponents = skincolorString.Replace("RGBA(", "").Replace(")", "").Split(',');
                if (skincolorComponents.Length == 4)
                {
                    float r, g, b, a;
                    try
                    {
                        if (float.TryParse(skincolorComponents[0], out r) &&
float.TryParse(skincolorComponents[1], out g) &&
float.TryParse(skincolorComponents[2], out b) &&
float.TryParse(skincolorComponents[3], out a))

                        {
                            UnityEngine.Color skinColor = new UnityEngine.Color(r, g, b, a);
                            newPawn.story.SkinColorBase = skinColor;
                            newPawn.story.skinColorOverride = skinColor;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Message("Error with skincolor: " + e.ToString());
                    }
                    string bodytype = pawnstring.Split('*')[4].Split('|')[5];
                    newPawn.story.bodyType = (BodyTypeDef)DefDatabase<BodyTypeDef>.GetNamed(bodytype);
                    return newPawn;
                }

            }

            return null;
        }


        public static Thing stringtoitem(string item)
        {
            if(item != null)
            {
                Log.Message("string to item: " + item);
                Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);

                foreach (ThingDef itemdef in DefDatabase<ThingDef>.AllDefs)
                {
                    if (item.Split('|')[0].Contains("minified-") && itemdef.defName == item.Split('|')[0].Replace("minified-", ""))
                    {
                        thing = ThingMaker.MakeThing(itemdef);
                        Thing miniThing = thing.TryMakeMinified();
                        thing = miniThing;

                        CompQuality compQuality = thing.TryGetComp<CompQuality>();
                        if (compQuality != null)
                        {
                            //QualityCategory q = (QualityCategory)itemQuality;
                            //compQuality.SetQuality(q, ArtGenerationContext.Colony);
                        }

                        foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
                        {
                            if (def.defName == item.Split('|')[2])
                            {
                                thing.SetStuffDirect(def);
                            }
                        }

                        break;
                    }

                    else if (itemdef.defName == item.Split('|')[0])
                    {
                        thing = ThingMaker.MakeThing(itemdef);

                        CompQuality compQuality = thing.TryGetComp<CompQuality>();
                        if (compQuality != null)
                        {
                            //QualityCategory q = (QualityCategory)itemQuality;
                            //compQuality.SetQuality(q, ArtGenerationContext.Colony);
                        }

                        foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
                        {
                            if (def.defName == item.Split('|')[2])
                            {
                                thing.SetStuffDirect(def);
                            }
                        }

                        break;
                    }
                }

                thing.stackCount = int.Parse(item.Split('|')[1]);
                return thing;
            }
            return null;
            //itemtostring += item.def.defName + "|" + item.stackCount + "|" + item.def.MadeFromStuff + "|" + item.HitPoints;
        }


        public static void UpdateTradeableItems(string pawnstring)
        {
            Log.Message("Update tradeables called 1");
            map = Find.AnyPlayerHomeMap;
            if (map == null)
            {
                Log.Error("No player home map found");
                return;
            }
            positionToPlace = GetRandomPosition(map);
            if (!positionToPlace.IsValid)
            {
                Log.Error("Failed to find a valid position");
                return;
            }
            Log.Message("Update tradeableitems called " + pawnstring);
            try
            {
                Zone z = map.zoneManager.AllZones.Find(zone => zone.label == "Trading" && zone.GetType() == typeof(Zone_Stockpile));
                positionToPlace = z.Position;
            }
            catch { }
            foreach(string str in pawnstring.Split(':'))
            {
                if(str != null && str != "" && str != " ")
                {
                    Log.Message("For each string in updatetradbles called:" + str);
                    string itemDefName = str.Split('|')[0];
                    if (itemDefName == "Human")
                    {
                        //Log.Message("Original ID:" + pawnstring.Split(':')[index].Split('|')[5]);
                        Pawn newPawn = stringtopawn(str);
                        int originalid = int.Parse(str.Split('|')[5]);
                        int newid = newPawn.thingIDNumber;
                        string[] pawnstringArray = pawnstring.Split(':');
                        int indexToUpdate = Array.FindIndex(pawnstringArray, s => s.Contains(str));
                        if (indexToUpdate != -1)
                        {
                            string[] pawnDetails = pawnstringArray[indexToUpdate].Split('|');
                            pawnDetails[5] = newPawn.thingIDNumber.ToString();
                            pawnstringArray[indexToUpdate] = string.Join("|", pawnDetails);
                        }
                        string updatedPawnstring = string.Join(":", pawnstringArray);
                        pawnstring = updatedPawnstring;
                        Log.Message(updatedPawnstring);
                        try
                        {
                            // string[] splitStr = null;
                            // if (index < pawnstring.Split(';').Length)
                            //{
                            // splitStr = pawnstring.Split(';')[index].ToString().Split('|');
                            // Rest of the code
                            //}
                            //  else
                            // {
                            //     Log.Message("Index out of range. Skipping this iteration.");
                            // }
                            //Log.Message(splitStr[5]);

                            //splitStr[5] = newid.ToString();
                            //Log.Message(splitStr[5]);

                            // pawnstring.Split(':')[index] = string.Join("|", splitStr);
                            //Log.Message(pawnstring.Split('|')[index]);
                        }
                        catch (Exception e)
                        {
                            Log.Message("Error while setting ID. " + e.Message);
                        }

                        //Log.Message("Set new ID: " + pawnstring.Split(':')[index].Split('|')[5]);
                        try { GenPlace.TryPlaceThing(newPawn, OnVisitAccept.positionToPlace, OnVisitAccept.map, ThingPlaceMode.Near); }
                        catch { }
                    }
                    else
                    {
                        Log.Message("Called getting item");
                        Thing item = stringtoitem(str);
                        Log.Message("Got item");
                        int originalid = int.Parse(str.Split('|')[4]);
                        Log.Message("Getting orignal id");
                        int newid = item.thingIDNumber;
                        Log.Message("set new id");
                        string[] itemstringArray = pawnstring.Split(':');
                        Log.Message("splitting pawnstring");
                        int indexToUpdate = Array.FindIndex(itemstringArray, s => s.Contains(str));
                        Log.Message("1");
                        if (indexToUpdate != -1)
                        {
                            Log.Message("2");
                            string[] itemDetails = itemstringArray[indexToUpdate].Split('|');
                            itemDetails[4] = item.thingIDNumber.ToString();
                            Log.Message("3");
                            itemstringArray[indexToUpdate] = string.Join("|", itemDetails);
                        }
                        string updateditemstring = string.Join(":", itemstringArray);
                        pawnstring = updateditemstring;



                        try { GenPlace.TryPlaceThing(item, OnVisitAccept.positionToPlace, OnVisitAccept.map, ThingPlaceMode.Near); }
                        catch {  }
                    }

            }
            ColonistBar_CheckRecacheEntries.savedlastcaravan = pawnstring;

            Log.Message("BROOOO DO THIS LOOK: " + pawnstring);
            }
        }
    }

}
