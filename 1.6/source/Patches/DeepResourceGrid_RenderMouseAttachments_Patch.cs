using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Deep_Reserves.Patches
{
    [HarmonyPatch(typeof(DeepResourceGrid), "RenderMouseAttachments")]
    public static class DeepResourceGrid_RenderMouseAttachments_Patch
    {
        public static void Postfix(DeepResourceGrid __instance, Map ___map)
        {
            IntVec3 c = UI.MouseCell();
            if (!c.InBounds(___map))
            {
                return;
            }

            ThingDef thingDef = __instance.ThingDefAt(c);
            if (thingDef == null || __instance.CountAt(c) <= 0)
            {
                return;
            }

            long total = GetDepositTotal(__instance, ___map, c, thingDef);
            if (total <= 0)
            {
                return;
            }

            Vector2 vector = c.ToVector3().MapToUIPosition();
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            float offset = (UI.CurUICellSize() - 27f) / 2f;
            Rect rect = new Rect(vector.x + offset, vector.y - UI.CurUICellSize() + offset + 29f, 27f, 27f);
            Widgets.ThingIcon(rect, thingDef);
            Widgets.Label(new Rect(rect.xMax + 4f, rect.y, 999f, 29f), "DeepReserves.DeepDepositRemaining".Translate(NamedArgumentUtility.Named(thingDef, "RESOURCE"), total.Named("COUNT")));
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static long GetDepositTotal(DeepResourceGrid grid, Map map, IntVec3 start, ThingDef thingDef)
        {
            long total = 0;
            Queue<IntVec3> queue = new Queue<IntVec3>();
            HashSet<IntVec3> visited = new HashSet<IntVec3>();

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

            return total;
        }
    }
}
