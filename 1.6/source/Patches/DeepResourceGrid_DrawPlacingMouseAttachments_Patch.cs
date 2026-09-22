using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Deep_Reserves.Patches
{
    [HarmonyPatch(typeof(DeepResourceGrid), "DrawPlacingMouseAttachments")]
    public static class DeepResourceGrid_DrawPlacingMouseAttachments_Patch
    {
        public static bool Prefix(DeepResourceGrid __instance, BuildableDef placingDef, Map ___map)
        {
            if (!(placingDef is ThingDef thingDef) || thingDef.CompDefFor<CompDeepDrill>() == null)
            {
                return true;
            }

            if (!__instance.AnyActiveDeepScannersOnMap())
            {
                return false;
            }

            IntVec3 center = UI.MouseCell();
            if (!center.InBounds(___map))
            {
                return false;
            }

            List<ThingDef> resourceDefs = new List<ThingDef>();
            List<long> resourceTotals = new List<long>();

            for (int i = 0; i < DeepDrillUtility.NumCellsToScan; i++)
            {
                IntVec3 cell = center + GenRadial.RadialPattern[i];
                if (!cell.InBounds(___map))
                {
                    continue;
                }

                ThingDef cellDef = __instance.ThingDefAt(cell);
                if (cellDef == null)
                {
                    continue;
                }

                int count = __instance.CountAt(cell);
                if (count <= 0)
                {
                    continue;
                }

                int index = resourceDefs.IndexOf(cellDef);
                if (index < 0)
                {
                    resourceDefs.Add(cellDef);
                    resourceTotals.Add(count);
                }
                else
                {
                    resourceTotals[index] += count;
                }
            }

            if (resourceDefs.Count == 0)
            {
                return false;
            }

            Vector2 vector = center.ToVector3().MapToUIPosition();
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            float offset = (UI.CurUICellSize() - 27f) / 2f;

            for (int i = 0; i < resourceDefs.Count; i++)
            {
                ThingDef resourceDef = resourceDefs[i];
                Rect rect = new Rect(vector.x + offset, vector.y - UI.CurUICellSize() + offset + 29f * i, 27f, 27f);
                Widgets.ThingIcon(rect, resourceDef);
                Widgets.Label(new Rect(rect.xMax + 4f, rect.y, 999f, 29f), "DeepReserves.DeepDrillRangeRemaining".Translate(NamedArgumentUtility.Named(resourceDef, "RESOURCE"), resourceTotals[i].Named("COUNT")));
            }

            Text.Anchor = TextAnchor.UpperLeft;

            return false;
        }
    }
}
