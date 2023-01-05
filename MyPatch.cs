using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace Custom_Rimworld_Mod
{
    public class MyPatch
    {
        public static void DoPatching()
        {
            var harmony = new Harmony("com.consciousness.patch");
            
            
            harmony.PatchAll();
        }

    }
}
