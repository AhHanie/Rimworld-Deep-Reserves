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
            DeepDrillResources.CountAt(___map, center, resourceDefs, resourceTotals);

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
                Rect rect = new Rect(vector.x + offset, vector.y - UI.CurUICellSize() + offset - 29f * i, 27f, 27f);
                Widgets.ThingIcon(rect, resourceDef);
                Widgets.Label(new Rect(rect.xMax + 4f, rect.y, 999f, 29f), "DeepReserves.DeepDrillRangeRemaining".Translate(NamedArgumentUtility.Named(resourceDef, "RESOURCE"), resourceTotals[i].Named("COUNT")));
            }

            Text.Anchor = TextAnchor.UpperLeft;

            return false;
        }
    }
}
