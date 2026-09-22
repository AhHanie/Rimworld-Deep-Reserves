using HarmonyLib;
using RimWorld;
using Verse;

namespace Deep_Reserves.Patches
{
    [HarmonyPatch(typeof(DeepResourceGrid), "DeepResourcesOnGUI")]
    public static class DeepResourceGrid_DeepResourcesOnGUI_Patch
    {
        public static bool Prefix()
        {
            if (Find.DesignatorManager.SelectedDesignator is Designator_Place placing &&
                placing.PlacingDef is ThingDef thingDef &&
                thingDef.CompDefFor<CompDeepDrill>() != null)
            {
                return false;
            }

            return true;
        }
    }
}
