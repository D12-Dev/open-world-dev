using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class AidHandler
    {
        private static IncidentDef incidentDef;
        private static IncidentParms parms;

        public static int aidUsageCost;

        public static void SendAidHandle()
        {
            RimworldHandler.TakeSilverFromCaravan(aidUsageCost);

            LetterCache.GetLetterDetails("Aid sent",
                "You have successfully sent aid!", LetterDefOf.PositiveEvent);

            Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);

            FocusCache.waitWindowInstance.Close();
        }

        public static void AidNotAvailableHandle()
        {
            Find.WindowStack.Add(new OW_ErrorDialog("Player is not available for this action"));
            FocusCache.waitWindowInstance.Close();
        }

        public static void ReceiveAidHandle()
        {
            incidentDef = incidentDef = IncidentDefOf.WandererJoin;

            IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Find.AnyPlayerHomeMap);

            parms = new IncidentParms
            {
                customLetterLabel = "Received Aid - Wanderer",
                target = Find.AnyPlayerHomeMap,
                points = defaultParms.points
            };

            Injections.thingsToDoInUpdate.Add(ExecuteAid);
        }

        private static void ExecuteAid()
        {
            incidentDef.Worker.TryExecute(parms);
        }
    }
}
