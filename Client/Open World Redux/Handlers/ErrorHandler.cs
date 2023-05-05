using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Profile;

namespace OpenWorldRedux
{
    public static class ErrorHandler
    {
        public static void AlreadyRegisteredHandle()
        {
            string[] errorChain = new string[]
            {
                "This account is already registered in the server",
                "If this is your account please login normally"
            };

            Find.WindowStack.Add(new OW_ChainErrorDialog(errorChain));
            FocusCache.waitWindowInstance.Close();
        }

        public static void IncorrectLoginHandle()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("The provided login information was wrong"));
            FocusCache.waitWindowInstance.Close();
        }

        public static void FactionAlreadyExistsHandle()
        {
            string[] errorChain = new string[]
            {
                "A faction with that name already exists",
                "Please try again with a different name"
            };

            Find.WindowStack.Add(new OW_ChainErrorDialog(errorChain));
        }

        public static void InAnotherFactionErrorHandle()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("The requested player is in another faction"));
        }

        public static void NotInFactionErrorHandle()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("The requested player is not in your faction"));
        }

        public static void NotEnoughPowerErrorHandle()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("You don't have enough power for this action"));
        }

        public static void PlayerNotAvailableHandle()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("Player is not available for this action"));
        }

        public static void ForceDisconnectCountermeasures()
        {
            Log.Message("[Openworld] Forcing counter measures on disconnect.");
            BooleanCache.isConnectedToServer = false;
            BooleanCache.isTryingToConnect = false;
            BooleanCache.hasLoadedCorrectly = false;
            BooleanCache.isGeneratingNewWorld = false;
            BooleanCache.worldGenerated = false;
            BooleanCache.worldSaved = false;
            BooleanCache.isGeneratingWorldFromPacket = false;
            DifficultyCache.usingCustomDifficulty = false;
            FactionCache.hasFaction = false;

            if (Current.ProgramState == ProgramState.Playing)
            {
                LongEventHandler.QueueLongEvent(delegate
                {
                    MemoryUtility.ClearAllMapsAndWorld();
                }, "Entry", "", doAsynchronously: false, null, showExtraUIInfo: false);
            }
        }
        public static void UnsuccessfulPassword() {
            Find.WindowStack.Add(new OW_ErrorDialog("The inputted server password was incorrect!"));
            FocusCache.waitWindowInstance.Close();

        }
        public static void BannedPlayerHandler()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("You are banned from this server"));
            FocusCache.waitWindowInstance.Close();
        }

        public static void WrongVersionHandle()
        {
            string[] errorChain = new string[]
            {
                "You have a different version of the mod from the server!",
                "Please update the mod/server to latest before joining..."
            };

            Find.WindowStack.Add(new OW_ChainErrorDialog(errorChain));
        }

        public static void MissingModsHandle(Packet receivedPacket)
        {
            Find.WindowStack.Add(new OW_ErrorListDialog("Mods are missing from the game", receivedPacket.contents));
        }

        public static void ExtraModsHandle(Packet receivedPacket)
        {
            Find.WindowStack.Add(new OW_ErrorListDialog("Mods are not permitted in the server", receivedPacket.contents));
        }

        public static void ForbiddenModsHandle(Packet receivedPacket)
        {
            Find.WindowStack.Add(new OW_ErrorListDialog("Mods are forbidden in the server", receivedPacket.contents));
        }

        public static void ServerFullHandle()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("The requested server is currently full"));
            FocusCache.waitWindowInstance.Close();
        }

        public static void NotWhitelistedHandle()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("You are not whitelisted in the server"));
            FocusCache.waitWindowInstance.Close();
        }
    }
}
