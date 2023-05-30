using HarmonyLib;
using Multiplayer.Client;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux.RTSE
{
    public static class OnSendVisitRequest
    {

        public static string visitrequeststringfinal;

        public static string Pawnstostring(List<Pawn> Pawns = null, Caravan caravan = null)
        {
            if (caravan != null)
            {
                foreach (Pawn pawn in caravan.PawnsListForReading)
                {
                    visitrequeststringfinal += pawntostring(pawn);
                }
                return visitrequeststringfinal;
            }
            else if (Pawns != null)
            {
                foreach (Pawn pawn in Pawns)
                {
                    visitrequeststringfinal += pawntostring(pawn);
                }
                return visitrequeststringfinal;
            }
            return null;
        }

        public static string pawntostring(Pawn sentPawn)
        {
            string visitrequeststring = "";
            if (sentPawn != null)
            {
                if (sentPawn.def.defName == "Human")
                {
                    Log.Message("Pawn to string called 1: " + sentPawn.Name.ToString());
                    visitrequeststring += sentPawn.def.defName.ToString() + "|";
                    visitrequeststring += sentPawn.Name.ToString() + "|";
                    visitrequeststring += sentPawn.ageTracker.AgeBiologicalTicks.ToString() + "|";
                    visitrequeststring += sentPawn.ageTracker.AgeChronologicalTicks.ToString() + "|";
                    visitrequeststring += sentPawn.gender.ToString() + "|";
                    visitrequeststring += sentPawn.thingIDNumber.ToString() + "|";
                    visitrequeststring += "Pawn" + "|";
                    if (sentPawn.genes.Xenotype != null)
                    {
                        visitrequeststring += sentPawn.genes.Xenotype.ToString();
                    }
                    visitrequeststring += "|";
                    if (sentPawn.story.favoriteColor != null)
                    {
                        visitrequeststring += sentPawn.story.favoriteColor.ToString();
                    }
                    visitrequeststring += "|";
                    if (sentPawn.genes.CustomXenotype != null)
                    {
                        visitrequeststring += sentPawn.genes.xenotypeName.ToString();
                    }
                    visitrequeststring += "|";

                    visitrequeststring += "*";

                    if (sentPawn.story.Childhood != null)
                    {
                        visitrequeststring += sentPawn.story.Childhood.ToString() + "|";
                    }
                    if(sentPawn.story.Adulthood != null)
                    {
                        visitrequeststring += sentPawn.story.Adulthood.ToString() + "|";
                    }
                    

                    visitrequeststring += "*";

                    foreach (SkillRecord s in sentPawn.skills.skills)
                    {
                        visitrequeststring += s.levelInt + "-";
                        visitrequeststring += (int)s.passion + "|";
                    }
                    visitrequeststring += "*";

                    foreach (Trait t in sentPawn.story.traits.allTraits)
                    {
                        visitrequeststring += t.Label + "|";
                    }

                    visitrequeststring += "*";

                    visitrequeststring += sentPawn.story.hairDef.ToString() + "|";
                    visitrequeststring += sentPawn.story.HairColor + "|";
                    visitrequeststring += sentPawn.story.headType.ToString() + "|";
                    visitrequeststring += sentPawn.story.SkinColor + "|";
                    visitrequeststring += sentPawn.style.beardDef.defName + "|";
                    visitrequeststring += sentPawn.story.bodyType + "|";

                    visitrequeststring += "*";

                    StringBuilder healthStateBuilder = new StringBuilder();

                    foreach (Hediff hediff in sentPawn.health.hediffSet.hediffs)
                    {
                        visitrequeststring += hediff.def.defName + ";" + hediff.Part?.Label + ";" + hediff.Severity + ";" + hediff.IsPermanent() + ";" + "|";
                    }
                    visitrequeststring += "*";

                    foreach (Apparel apparel in sentPawn.apparel.WornApparel)
                    {
                        string apparelMaterialLabel = null;
                        QualityCategory quality;
                        apparel.TryGetQuality(out quality);
                        if (apparel.Stuff != null)
                        {
                            apparelMaterialLabel = apparel.Stuff.defName;
                        }
                        visitrequeststring += apparel.def.defName + ";" + apparel.HitPoints + ";" + quality + ";" + apparel.WornByCorpse.ToString() + ";" + apparelMaterialLabel + ";" + apparel.MaxHitPoints + " ; " + "|";
                    }

                    visitrequeststring += "*";

                    if (sentPawn.equipment.Primary != null)
                    {
                        var equippedWeapon = sentPawn.equipment.Primary;
                        QualityCategory weaponQuality;
                        equippedWeapon.TryGetQuality(out weaponQuality);
                        string weaponStuffLabel = null;

                        visitrequeststring += equippedWeapon.def.defName + ";" + equippedWeapon.HitPoints + ";" + weaponQuality + ";" + equippedWeapon.MaxHitPoints + ";" + "|";
                    }
                    visitrequeststring += "*";

                    visitrequeststring += ":";



                }
                else
                {
                    Log.Message("Pawn to string called 2: " + sentPawn.Name.ToString());
                    visitrequeststring += sentPawn.def.defName.ToString() + "|";
                    visitrequeststring += sentPawn.Name.ToString() + "|";
                    visitrequeststring += sentPawn.ageTracker.AgeBiologicalTicks.ToString() + "|";
                    visitrequeststring += sentPawn.ageTracker.AgeChronologicalTicks.ToString() + "|";
                    visitrequeststring += sentPawn.gender.ToString() + "|";
                    visitrequeststring += sentPawn.thingIDNumber.ToString() + "|";
                    visitrequeststring += "Pawn" + "|"; 

                    visitrequeststring += "*";

                    StringBuilder healthStateBuilder = new StringBuilder();

                    foreach (Hediff hediff in sentPawn.health.hediffSet.hediffs)
                    {
                        visitrequeststring += hediff.def.defName + ";" + hediff.Part?.Label + ";" + hediff.Severity + ";" + hediff.IsPermanent() + ";" + "|";
                    }
                    visitrequeststring += "*";
                    StringBuilder trainingInfoBuilder = new StringBuilder();

                    foreach (TrainableDef trainable in DefDatabase<TrainableDef>.AllDefsListForReading)
                    {
                        bool canTrain = sentPawn.training.CanAssignToTrain(trainable).Accepted;
                        bool hasLearned = sentPawn.training.HasLearned(trainable);
                        bool isDisabled = sentPawn.training.GetWanted(trainable);
                        trainingInfoBuilder.Append($"{trainable.defName};{canTrain};{hasLearned};{isDisabled}|");
                    }

                    string trainingInfo = trainingInfoBuilder.ToString();

                    if (trainingInfo.Length > 0)
                    {
                        // Remove the trailing '|' from the final string.
                        trainingInfo = trainingInfo.Substring(0, trainingInfo.Length - 1);
                    }

                    visitrequeststring += trainingInfo;
                    visitrequeststring += "*";
                }
                visitrequeststring += ":";

            }
            return visitrequeststring;
        }

        public static string itemstostring(List<Thing> Things = null, Caravan caravan = null)
        {
            visitrequeststringfinal = "";
            if (caravan != null)
            {

                foreach (Thing thing in caravan.AllThings)
                {
                    if(thing != null && !(thing is Pawn))
                    {
                        visitrequeststringfinal += itemtostring(thing);
                    }
                }
                return visitrequeststringfinal;
            }
            else if (Things != null)
            {
                foreach (Thing thing in Things)
                {
                    if (thing != null && !(thing is Pawn))
                    {
                        visitrequeststringfinal += itemtostring(thing);
                    }
                }
                return visitrequeststringfinal;
            }
            return null;
        }

        public static string itemtostring(Thing item, int counttotransfer = -1)
        {
            Log.Message("item to string called:" + item.def.defName);
            if(counttotransfer == -1)
            {
                counttotransfer = item.stackCount;
            }
            string itemtostring = "";
            if(item != null && item.def.defName != "Human")
            {
                if (item != null)
                {
                    itemtostring += item.def.defName + "|" + item.stackCount + "|" + item.def.MadeFromStuff + "|" + item.HitPoints + "|" + item.thingIDNumber +  "|" + "Item" +  ":";

                    return itemtostring;
                }
            }

            return null;
        }

        public static void removetakenitems(string items)
        {
            Map map = Find.CurrentMap;

            List<Thing> mapItems = map.listerThings.AllThings;
            Log.Message("Remove taken items called: " + items);
            foreach (string item in items.Split(':')) {
                if (item != null && item != "")
                {
                    Log.Message(item.Split('|')[0]);
                    if (item.Split('|')[0] == "Human")
                    {
                        Log.Message("Human found");
                        try
                        {
                            foreach (Thing thing in mapItems)
                            {
                                if (thing is Pawn pawn) // Check if the Thing is a Pawn before casting
                                {
                                    Log.Message("Checking:" + pawn.Name);
                                    if (pawn.thingIDNumber == int.Parse(item.Split('|')[5]))
                                    {
                                        Log.Message("Found, destroying:" + pawn.Name);
                                        pawn.Destroy();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Message("Error:" + e.ToString());
                        }

                    }
                    else
                    {
                        try
                        {
                            foreach (Thing thing in mapItems)
                            {
                                    if (thing.thingIDNumber == int.Parse(item.Split('|')[4]))
                                    {
                                        thing.Destroy();
                                    }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Message("Error:" + e.ToString());
                        }
                    }
                }

            }
           // HostUtil.SetAllUniqueIds(Multiplayer.Client.Multiplayer.GlobalIdBlock.Current);
            //Multiplayer.Client.Multiplayer.StopMultiplayer();
            //Multiplayer.Client.Multiplayer.session = null;

                try
                {
                    const string suffix = "-preconvert";
                    var saveName = $"{GenFile.SanitizedFileName(Multiplayer.Client.Multiplayer.session.gameName)}{suffix}";

                    new FileInfo(Path.Combine(Multiplayer.Client.Multiplayer.ReplaysDir, saveName + ".zip")).Delete();
                    Replay.ForSaving(saveName).WriteCurrentData();
                }
                catch (Exception e)
                {
                    Log.Warning($"Convert to singleplayer failed to write pre-convert file: {e}");
                }

                Find.GameInfo.permadeathMode = false;
                HostUtil.SetAllUniqueIds(Multiplayer.Client.Multiplayer.GlobalIdBlock.Current);

                Multiplayer.Client.Multiplayer.StopMultiplayer();

            BooleanCache.ishostingrtseserver = false;
            GameDataSaveLoader.SaveGame("Open World Server Save " + OW_MPLogin.myusername);
            SaveHandler.SendSaveToServer("Open World Server Save " + OW_MPLogin.myusername);
            MemoryUtility.ClearAllMapsAndWorld();
            GenScene.GoToMainMenu();
            string[] contents = new string[] { };
            Packet ClientSaveFilePacket = new Packet("Requestsave", contents);
            Network.SendData(ClientSaveFilePacket);
            //GameDataSaveLoader.LoadGame("Last Hosted Open World Save");
             

            // MemoryUtility.ClearAllMapsAndWorld();
            // GenScene.GoToMainMenu();

        }

    }
}

   
