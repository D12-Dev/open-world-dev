using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class DifficultyFile
    {
        public float ThreatScale;
        public bool AllowBigThreats;
        public bool AllowViolentQuests;
        public bool AllowIntroThreats;
        public bool PredatorsHuntHumanlikes;
        public bool AllowExtremeWeatherIncidents;

        public float CropYieldFactor;
        public float MineYieldFactor;
        public float ButcherYieldFactor;
        public float ResearchSpeedFactor;
        public float QuestRewardValueFactor;
        public float RaidLootPointsFactor;
        public float TradePriceFactorLoss;
        public float MaintenanceCostFactor;
        public float ScariaRotChance;
        public float EnemyDeathOnDownedChanceFactor;

        public float ColonistMoodOffset;
        public float FoodPoisonChanceFactor;
        public float ManhunterChanceOnDamageFactor;
        public float PlayerPawnInfectionChanceFactor;
        public float DiseaseIntervalFactor;
        public float DeepDrillInfestationChanceFactor;
        public float FriendlyFireChanceFactor;
        public float AllowInstantKillChance;

        public bool AllowTraps;
        public bool AllowTurrets;
        public bool AllowMortars;

        public float AdaptationEffectFactor;
        public float AdaptationGrowthRateFactorOverZero;
        public bool FixedWealthMode;
    }
}
