using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection;
using HarmonyLib;
using System.Reflection.Emit;

namespace Consciousness_Patch
{
    [HarmonyPatch(typeof(PawnCapacityWorker_Consciousness))]
    [HarmonyPatch(nameof(PawnCapacityWorker_Consciousness.CalculateCapacityLevel))]
    public static class TranspilerConsciousnessPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);
            List<int> startIndexes=new List<int>();
            List<int> endIndexes = new List<int>();

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4)
                {
                    if((float)codes[i].operand == 1 && codes[i + 1].opcode == OpCodes.Call){
                        startIndexes.Add(i);
                        endIndexes.Add(i+1);
                    }
                } 
            }
            for(var i = startIndexes.Count-1; i >= 0; i--)
            {
                codes[startIndexes[i]].opcode = OpCodes.Nop;
                codes.RemoveRange(startIndexes[i] + 1, endIndexes[i] - startIndexes[i]);
            }
            /*
                if (startIndexMath1 > -1 && endIndexMath1 > -1)
                {
                    codes[startIndexMath1].opcode = OpCodes.Nop;
                    codes.RemoveRange(startIndexMath1+1, endIndexMath1- startIndexMath1);
                }*/
            return codes.AsEnumerable();

        }
    }
}
