
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OpenWorldRedux
{
    public class OW_MPFactionSiteConfirmBuild : Window
    {
        public override Vector2 InitialSize => new Vector2(350f, 150f);

        private string windowTitle = "Warning!";
        private string windowDescription = "Are you sure you want to build this site?";

        private float buttonX = 137f;
        private float buttonY = 38f;

        private FactionCache.StructureTypes buildingType;
        private int silverCost;

        public OW_MPFactionSiteConfirmBuild(FactionCache.StructureTypes buildingType)
        {
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = true;

            this.buildingType = buildingType;
        }

        public override void DoWindowContents(Rect rect)
        {
            float centeredX = rect.width / 2;

            float windowDescriptionDif = Text.CalcSize(windowDescription).y + StandardMargin;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowTitle).x / 2, rect.y, Text.CalcSize(windowTitle).x, Text.CalcSize(windowTitle).y), windowTitle);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredX - Text.CalcSize(windowDescription).x / 2, windowDescriptionDif, Text.CalcSize(windowDescription).x, Text.CalcSize(windowDescription).y), windowDescription);

            if (Widgets.ButtonText(new Rect(new Vector2(rect.x, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "Yes"))
            {
                OnAccept();
            }

            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.yMax - buttonY), new Vector2(buttonX, buttonY)), "No"))
            {
                OnCancel();
            }
        }

        private void OnAccept()
        {
            silverCost = FactionCache.GetStructureCost((int)buildingType);

            if (buildingType == FactionCache.StructureTypes.Silo) Build();

            if (buildingType == FactionCache.StructureTypes.Marketplace) Build();

            if (buildingType == FactionCache.StructureTypes.ProductionSite)
            {
                foreach (StructureFile structure in WorldCache.onlineStructuresDeflate)
                {
                    if (structure.structureType == (int)FactionCache.StructureTypes.ProductionSite && 
                        structure.structureFaction == 1)
                    {
                        Find.WindowStack.Add(new OW_ErrorDialog("Max structure limit has been reached"));
                        Close();
                        return;
                    }
                }

                Build();
            }

            if (buildingType == FactionCache.StructureTypes.Wonder)
            {
                foreach (StructureFile structure in WorldCache.onlineStructuresDeflate)
                {
                    if (structure.structureType == (int)FactionCache.StructureTypes.Wonder)
                    {
                        Find.WindowStack.Add(new OW_ErrorDialog("Max structure limit has been reached"));
                        Close();
                        return;
                    }
                }

                Build();
            }

            if (buildingType == FactionCache.StructureTypes.Bank) Build();

            if (buildingType == FactionCache.StructureTypes.Aeroport) Build();

            if (buildingType == FactionCache.StructureTypes.Courier) Build();

            Close();
        }

        private void OnCancel()
        {
            Close();
        }

        private void Build()
        {
            if (!CaravanInventoryUtility.HasThings(FocusCache.focusedCaravan, ThingDefOf.Silver, silverCost))
            {
                Find.WindowStack.Add(new OW_ErrorDialog("You don't have enough funds for this action"));
            }

            else
            {
                RimworldHandler.TakeSilverFromCaravan(silverCost);

                StructureFile newStructure = new StructureFile();
                newStructure.structureTile = FocusCache.focusedCaravan.Tile;
                newStructure.structureType = (int)buildingType;

                string[] contents = new string[] { Serializer.SoftSerialize(newStructure) };
                Packet BuildFactionStructure = new Packet("BuildFactionStructure", contents);
                Network.SendData(BuildFactionStructure);
            }
        }
    }
}
