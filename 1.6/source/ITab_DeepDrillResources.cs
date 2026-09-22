using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Deep_Reserves
{
    public class ITab_DeepDrillResources : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(280f, 260f);
        private readonly List<ThingDef> resourceDefs = new List<ThingDef>();
        private readonly List<long> resourceTotals = new List<long>();
        private Vector2 scrollPosition;

        public override bool IsVisible => SelThing != null && SelThing.Spawned && SelThing.TryGetComp<CompDeepDrill>() != null;

        public ITab_DeepDrillResources()
        {
            size = WinSize;
            labelKey = "DeepReserves.DrillResourcesTab";
        }

        public override void OnOpen()
        {
            base.OnOpen();
            scrollPosition = Vector2.zero;
        }

        protected override void FillTab()
        {
            Thing drill = SelThing;
            if (drill == null || !drill.Spawned)
            {
                return;
            }

            DeepDrillResources.CountAt(drill.Map, drill.Position, resourceDefs, resourceTotals);
            Rect contentRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(12f);
            if (resourceDefs.Count == 0)
            {
                Widgets.Label(contentRect, "DeepReserves.NoDrillResources".Translate());
                return;
            }

            const float rowHeight = 32f;
            Rect viewRect = new Rect(0f, 0f, contentRect.width - 16f, resourceDefs.Count * rowHeight);
            Widgets.BeginScrollView(contentRect, ref scrollPosition, viewRect);
            TextAnchor previousAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            for (int i = 0; i < resourceDefs.Count; i++)
            {
                Rect iconRect = new Rect(0f, i * rowHeight + 2f, 27f, 27f);
                Widgets.ThingIcon(iconRect, resourceDefs[i]);
                Widgets.Label(new Rect(iconRect.xMax + 8f, i * rowHeight, viewRect.width - iconRect.xMax - 8f, rowHeight), resourceTotals[i].ToString());
                TooltipHandler.TipRegion(new Rect(0f, i * rowHeight, viewRect.width, rowHeight), resourceDefs[i].LabelCap);
            }
            Text.Anchor = previousAnchor;
            Widgets.EndScrollView();
        }
    }
}
