using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Deep_Reserves.Patches
{
    // Infix prefix on the RenderMouseAttachments call in vanilla's DrawPlacingMouseAttachments. Vanilla has already
    // checked that a deep drill is being placed and that a deep scanner is active, so this only replaces the
    // single-cell readout with the per-resource totals for the cells the drill would scan.
    [HarmonyPatch(typeof(DeepResourceGrid), nameof(DeepResourceGrid.DrawPlacingMouseAttachments))]
    public static class DeepResourceGrid_DrawPlacingMouseAttachments_Patch
    {
        private const float IconSize = 27f;
        private const float IconPaddingRight = 4f;
        private const float LineSpacing = 29f;

        private static readonly List<ThingDef> resourceDefs = new List<ThingDef>();
        private static readonly List<long> resourceTotals = new List<long>();

        [HarmonyInfix(typeof(DeepResourceGrid), "RenderMouseAttachments")]
        public static bool Prefix(Map ___map)
        {
            IntVec3 center = UI.MouseCell();
            DeepDrillResources.CountAt(___map, center, resourceDefs, resourceTotals);
            if (resourceDefs.Count == 0)
            {
                return false;
            }

            Color previousColor = GUI.color;
            GameFont previousFont = Text.Font;
            TextAnchor previousAnchor = Text.Anchor;
            try
            {
                Vector2 vector = center.ToVector3().MapToUIPosition();
                GUI.color = Color.white;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                float offset = (UI.CurUICellSize() - IconSize) / 2f;

                for (int i = 0; i < resourceDefs.Count; i++)
                {
                    ThingDef resourceDef = resourceDefs[i];
                    Rect rect = new Rect(vector.x + offset, vector.y - UI.CurUICellSize() + offset - LineSpacing * i, IconSize, IconSize);
                    Widgets.ThingIcon(rect, resourceDef);
                    Widgets.Label(new Rect(rect.xMax + IconPaddingRight, rect.y, 999f, LineSpacing), "DeepReserves.DeepDrillRangeRemaining".Translate(NamedArgumentUtility.Named(resourceDef, "RESOURCE"), resourceTotals[i].Named("COUNT")));
                }
            }
            finally
            {
                GUI.color = previousColor;
                Text.Font = previousFont;
                Text.Anchor = previousAnchor;
            }

            return false;
        }
    }
}
