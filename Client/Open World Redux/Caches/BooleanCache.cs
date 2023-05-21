using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldRedux
{
    public static class BooleanCache
    {
        public static bool isGeneratingNewWorld;

        public static bool isGeneratingWorldFromPacket;

        public static bool worldGenerated;
        public static bool BottomBarVis;
        public static bool worldSaved;

        public static bool newPlayer;

        public static bool isConnectedToServer;

        public static bool ishostingrtseserver = false;

        public static bool isTryingToConnect;

        public static bool hasLoadedCorrectly;

        public static bool secretFlag;

        public static bool hideProductionSiteMessages;

        public static bool isCustomScenariosAllowed;

        public static bool isAdmin;

        public static bool isSaving;
    }
}
