using System.IO;
using Verse;

namespace OpenWorldRedux
{
    public class ModConfigs : ModSettings
    {
        public bool tradeBool;
        public bool hideProductionSite;
        public bool secretBool;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref tradeBool, "tradeBool");
            Scribe_Values.Look(ref hideProductionSite, "hideProductionSite");
            Scribe_Values.Look(ref secretBool, "secretBool");
            base.ExposeData();

            TradeCache.inTrade = tradeBool;
            BooleanCache.secretFlag = secretBool;
            BooleanCache.hideProductionSiteMessages = hideProductionSite;
        }
    }
}
