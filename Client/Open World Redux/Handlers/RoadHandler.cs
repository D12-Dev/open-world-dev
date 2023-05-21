using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using System.Reflection;
using Verse.AI;

namespace OpenWorldRedux
{
    public static class RoadHandler
    {
        public static void CreateRoad(int startingTile, int endingTile, RoadDef road)
        {
            WorldGrid worldGrid = Find.WorldGrid;

            worldGrid.OverlayRoad(startingTile, endingTile, road);

            Find.World.renderer.SetAllLayersDirty();
        }

        public static void RemoveRoad(int tile)
        {
            WorldGrid worldGrid = Find.WorldGrid;

            worldGrid[tile].potentialRoads = null;

            Find.World.renderer.SetAllLayersDirty();
        }
    }
}
