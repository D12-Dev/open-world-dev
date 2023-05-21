using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldRedux
{
    public static class DifficultyCache
    {
        public static bool usingCustomDifficulty;

        public static float threatScale;
        public static bool allowBigThreats;
        public static bool allowViolentQuests;
        public static bool allowIntroThreats;
        public static bool predatorsHuntHumanlikes;
        public static bool allowExtremeWeatherIncidents;

        public static float cropYieldFactor;
        public static float mineYieldFactor;
        public static float butcherYieldFactor;
        public static float researchSpeedFactor;
        public static float questRewardValueFactor;
        public static float raidLootPointsFactor;
        public static float tradePriceFactorLoss;
        public static float maintenanceCostFactor;
        public static float scariaRotChance;
        public static float enemyDeathOnDownedChanceFactor;

        public static float colonistMoodOffset;
        public static float foodPoisonChanceFactor;
        public static float manhunterChanceOnDamageFactor;
        public static float playerPawnInfectionChanceFactor;
        public static float diseaseIntervalFactor;
        public static float deepDrillInfestationChanceFactor;
        public static float friendlyFireChanceFactor;
        public static float allowInstantKillChance;

        public static bool allowTraps;
        public static bool allowTurrets;
        public static bool allowMortars;

        public static float adaptationEffectFactor;
        public static float adaptationGrowthRateFactorOverZero;
        public static bool fixedWealthMode;
    }
}
