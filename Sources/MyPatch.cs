using Verse;
using HarmonyLib;

namespace Consciousness_Patch
{
    [StaticConstructorOnStartup]
    public class MyPatch
    {
        static MyPatch() {
            var harmony = new Harmony("com.consciousness.patch");
            harmony.PatchAll();
        }
    }
}
