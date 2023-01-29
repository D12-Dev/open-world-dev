using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Metadata;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux
{
    public static class MPGame
    {
        public static void SendPlayerSettlementData(Game __instance)
        {
			string[] contents = new string[]
			{
				__instance.CurrentMap.Tile.ToString()
            };

			Packet AddNewSettlementPacket = new Packet("AddNewSettlementPacket", contents);
			Network.SendData(AddNewSettlementPacket);

            Injections.thingsToDoInUpdate.Add(ForceSave);
        }

        public static void SendPlayerSettlementDataFromCaravan(Caravan caravan)
        {
            string[] contents = new string[]
            {
                caravan.Tile.ToString()
            };

            Packet AddNewSettlementPacket = new Packet("AddNewSettlementPacket", contents);
            Network.SendData(AddNewSettlementPacket);
        }

		public static void SendPlayerSettlementAbandonData(Settlement settlement)
		{
            string[] contents = new string[]
			{
                settlement.Tile.ToString()
			};

            Packet AddNewSettlementPacket = new Packet("RemoveSettlementPacket", contents);
            Network.SendData(AddNewSettlementPacket);

            Injections.thingsToDoInUpdate.Add(ForceSave);
        }

        public static void ForceSave()
        {
            if (BooleanCache.isSaving) return;
            else
            {
                BooleanCache.isSaving = true;

                Find.WindowStack.Add(new OW_SavingDialog());
                FieldInfo FticksSinceSave = AccessTools.Field(typeof(Autosaver), "ticksSinceSave");
                FticksSinceSave.SetValue(Current.Game.autosaver, 0);
                Current.Game.autosaver.DoAutosave();
                FocusCache.saveWindowInstance.Close();

                BooleanCache.isSaving = false;
            }
        }
    }
}