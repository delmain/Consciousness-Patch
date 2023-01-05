using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace Consciousness_Patch
{
    [StaticConstructorOnStartup]
    public class MyPatch
    {
        static MyPatch() {
            Log.Message("Custom Rimworld Mod Loaded");
            var harmony = new Harmony("com.consciousness.patch");
            harmony.PatchAll();
        }
    }
}
