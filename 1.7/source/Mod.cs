using HarmonyLib;
using Verse;

namespace Deep_Reserves
{
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            new Harmony("sk.deepreserves").PatchAll();
        }
    }
}
