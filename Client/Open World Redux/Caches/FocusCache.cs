using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public static class FocusCache
    {
        public static Settlement focusedSettlement;
        public static Caravan focusedCaravan;
        public static OW_MPTradeMenu tradeMenuInstance;
        public static OW_MPGiftMenu giftMenuInstance;
        public static OW_MPBarterMenu barterMenuInstance;
        public static OW_MPBarterRequest barterRequestInstance;
        public static OW_MPFactionMenu factionMenuInstance;
        public static OW_MPFactionOnPlayer factionOnPlayerInstance;
        public static OW_MPFactionSiloDeposit siloDepositMenuInstance;
        public static OW_MPFactionBankDeposit bankDepositMenuInstance;
        public static OW_MPAidMenu aidMenuInstance;
        public static OW_WaitingDialog waitWindowInstance;
        public static OW_SavingDialog saveWindowInstance;

        public static int quantityChosenOnDialog;

        public static int actualSaveTicks;
        public static int autosaveDays;
        public static int autosaveInternalTicks;

        public static int playerCount;

        public static List<string> playerList;

        public static string mainFolderPath;
        public static string saveFolderPath;
        public static string loginDataFilePath;
        public static string ModFolderPath;
        public static string versionCode = "1.13";
    }
}
