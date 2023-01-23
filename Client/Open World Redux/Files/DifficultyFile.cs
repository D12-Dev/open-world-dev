using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldRedux
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

        public void ApplyValues()
        {
            DifficultyCache.usingCustomDifficulty = true;

            DifficultyCache.threatScale = ThreatScale;
            DifficultyCache.allowBigThreats = AllowBigThreats;
            DifficultyCache.allowViolentQuests = AllowViolentQuests;
            DifficultyCache.allowIntroThreats = AllowIntroThreats;
            DifficultyCache.predatorsHuntHumanlikes = PredatorsHuntHumanlikes;
            DifficultyCache.allowExtremeWeatherIncidents = AllowExtremeWeatherIncidents;

            DifficultyCache.cropYieldFactor = CropYieldFactor;
            DifficultyCache.mineYieldFactor = MineYieldFactor;
            DifficultyCache.butcherYieldFactor = ButcherYieldFactor;
            DifficultyCache.researchSpeedFactor = ResearchSpeedFactor;
            DifficultyCache.questRewardValueFactor = QuestRewardValueFactor;
            DifficultyCache.raidLootPointsFactor = RaidLootPointsFactor;
            DifficultyCache.tradePriceFactorLoss = TradePriceFactorLoss;
            DifficultyCache.maintenanceCostFactor = MaintenanceCostFactor;
            DifficultyCache.scariaRotChance = ScariaRotChance;
            DifficultyCache.enemyDeathOnDownedChanceFactor = EnemyDeathOnDownedChanceFactor;

            DifficultyCache.colonistMoodOffset = ColonistMoodOffset;
            DifficultyCache.foodPoisonChanceFactor = FoodPoisonChanceFactor;
            DifficultyCache.manhunterChanceOnDamageFactor = ManhunterChanceOnDamageFactor;
            DifficultyCache.playerPawnInfectionChanceFactor = PlayerPawnInfectionChanceFactor;
            DifficultyCache.diseaseIntervalFactor = DiseaseIntervalFactor;
            DifficultyCache.deepDrillInfestationChanceFactor = DeepDrillInfestationChanceFactor;
            DifficultyCache.friendlyFireChanceFactor = FriendlyFireChanceFactor;
            DifficultyCache.allowInstantKillChance = AllowInstantKillChance;

            DifficultyCache.allowTraps = AllowTraps;
            DifficultyCache.allowTurrets = AllowTurrets;
            DifficultyCache.allowMortars = AllowMortars;

            DifficultyCache.adaptationEffectFactor = AdaptationEffectFactor;
            DifficultyCache.adaptationGrowthRateFactorOverZero = AdaptationGrowthRateFactorOverZero;
            DifficultyCache.fixedWealthMode = FixedWealthMode;
        }
    }
}
