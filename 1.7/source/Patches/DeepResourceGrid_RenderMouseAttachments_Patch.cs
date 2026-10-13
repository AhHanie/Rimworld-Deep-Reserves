using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Deep_Reserves.Patches
{
    // Infix on the single Widgets.Label call in vanilla's RenderMouseAttachments. Vanilla only reaches it
    // for a revealed cell with resources, and has already set GUI.color, Text.Font and Text.Anchor for its own
    // line, so this only has to draw the deposit total on the line below the vanilla label.
    [HarmonyPatch(typeof(DeepResourceGrid), "RenderMouseAttachments")]
    public static class DeepResourceGrid_RenderMouseAttachments_Patch
    {
        private const float IconSize = 27f;
        private const float IconPaddingRight = 4f;
        private const float LineSpacing = 29f;

        private static readonly Queue<IntVec3> queue = new Queue<IntVec3>();
        private static readonly HashSet<IntVec3> visited = new HashSet<IntVec3>();

        [HarmonyInfix(typeof(Widgets), nameof(Widgets.Label), typeof(Rect), typeof(TaggedString))]
        public static void Postfix(Rect rect, [HarmonyOuter] DeepResourceGrid __instance, [HarmonyOuter] Map ___map)
        {
            IntVec3 c = UI.MouseCell();
            ThingDef thingDef = __instance.ThingDefAt(c);
            long total = GetDepositTotal(__instance, ___map, c, thingDef);

            Rect iconRect = new Rect(rect.x - IconPaddingRight - IconSize, rect.y + LineSpacing, IconSize, IconSize);
            Widgets.ThingIcon(iconRect, thingDef);
            Widgets.Label(new Rect(rect.x, iconRect.y, rect.width, LineSpacing), "DeepReserves.DeepDepositRemaining".Translate(NamedArgumentUtility.Named(thingDef, "RESOURCE"), total.Named("COUNT")));
        }

        private static long GetDepositTotal(DeepResourceGrid grid, Map map, IntVec3 start, ThingDef thingDef)
        {
            long total = 0;
            queue.Clear();
            visited.Clear();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                IntVec3 cell = queue.Dequeue();
                total += grid.CountAt(cell);

                for (int i = 0; i < GenAdj.CardinalDirections.Length; i++)
                {
                    IntVec3 next = cell + GenAdj.CardinalDirections[i];
                    if (!next.InBounds(map) || visited.Contains(next))
                    {
                        continue;
                    }

                    if (grid.ThingDefAt(next) != thingDef || grid.CountAt(next) <= 0)
                    {
                        continue;
                    }

                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }

            queue.Clear();
            visited.Clear();
            return total;
        }
    }
}
