using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Deep_Reserves
{
    public static class DeepDrillResources
    {
        public static void CountAt(Map map, IntVec3 center, List<ThingDef> resourceDefs, List<long> resourceTotals)
        {
            resourceDefs.Clear();
            resourceTotals.Clear();

            if (map == null || !center.InBounds(map))
            {
                return;
            }

            DeepResourceGrid grid = map.deepResourceGrid;
            for (int i = 0; i < DeepDrillUtility.NumCellsToScan; i++)
            {
                IntVec3 cell = center + GenRadial.RadialPattern[i];
                if (!cell.InBounds(map))
                {
                    continue;
                }

                ThingDef resourceDef = grid.ThingDefAt(cell);
                if (resourceDef == null)
                {
                    continue;
                }

                int count = grid.CountAt(cell);
                if (count <= 0)
                {
                    continue;
                }

                int index = resourceDefs.IndexOf(resourceDef);
                if (index < 0)
                {
                    resourceDefs.Add(resourceDef);
                    resourceTotals.Add(count);
                }
                else
                {
                    resourceTotals[index] += count;
                }
            }
        }
    }
}
