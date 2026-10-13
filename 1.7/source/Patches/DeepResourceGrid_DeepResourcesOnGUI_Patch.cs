using HarmonyLib;
using RimWorld;
using Verse;

namespace Deep_Reserves.Patches
{
    // Infix prefix on the RenderMouseAttachments call in vanilla's DeepResourcesOnGUI: while a deep drill is being
    // placed, the placement readout replaces the selected-thing readout so the two never draw together.
    [HarmonyPatch(typeof(DeepResourceGrid), nameof(DeepResourceGrid.DeepResourcesOnGUI))]
    public static class DeepResourceGrid_DeepResourcesOnGUI_Patch
    {
        [HarmonyInfix(typeof(DeepResourceGrid), "RenderMouseAttachments")]
        public static bool Prefix()
        {
            return !(Find.DesignatorManager.SelectedDesignator is Designator_Place placing &&
                placing.PlacingDef is ThingDef thingDef &&
                thingDef.CompDefFor<CompDeepDrill>() != null);
        }
    }
}
