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
            int[] BloodPumping = getIndexes(codes);
            int[] Breathing = getIndexes(codes, BloodPumping[0]);
            int[] BloodFiltration = getIndexes(codes, Breathing[0]);
            List<int[]> allVariables = new List<int[]>();
            allVariables.Add(BloodFiltration);
            allVariables.Add(BloodPumping);
            allVariables.Add(Breathing);
            if (Patches.ConsciousnessSettings.instance.setCap)
            {
                allVariables.ForEach(x => changeCap(ref codes, x));
            }
            else
            {
                removeLimit(ref codes, BloodFiltration);
                getAllIndexes(ref codes, ref BloodPumping, ref Breathing, ref BloodFiltration);
                removeLimit(ref codes, Breathing);
                getAllIndexes(ref codes, ref BloodPumping, ref Breathing, ref BloodFiltration);
                removeLimit(ref codes, BloodPumping);
                getAllIndexes(ref codes, ref BloodPumping, ref Breathing, ref BloodFiltration);
            }

            changePercentage(ref codes, BloodPumping, Patches.ConsciousnessSettings.instance.percentageModifierBP);
            changePercentage(ref codes, Breathing, Patches.ConsciousnessSettings.instance.percentageModifierBR);
            changePercentage(ref codes, BloodFiltration, Patches.ConsciousnessSettings.instance.percentageModifierBF);

            getAllIndexes(ref codes,ref BloodPumping,ref Breathing,ref BloodFiltration);


            return codes.AsEnumerable();

        }

        public static void changePercentage(ref List<CodeInstruction> __codes, int[] ItemRange,float percentage)
        {
            for (var i = ItemRange[0]; i < ItemRange[1]; i++)
            {
                if (__codes[i].opcode == OpCodes.Ldc_R4 && (float)__codes[i].operand != 1)
                {
                    __codes[i].opcode = OpCodes.Nop;
                    __codes.RemoveAt(i);
                    __codes.Insert(i, new CodeInstruction(OpCodes.Ldc_R4, percentage));
                }
            }
        }

        public static void changeCap(ref List<CodeInstruction> __codes, int[] ItemRange)
        {
            float maxCap = Patches.ConsciousnessSettings.instance.maxCap * Patches.ConsciousnessSettings.instance.mCMultiplier / 100;
            for (var i = ItemRange[0]; i < ItemRange[1]; i++)
            {
                if (__codes[i].opcode == OpCodes.Ldc_R4 && (float)__codes[i].operand==1)
                {
                    __codes[i].opcode = OpCodes.Nop;
                    __codes.RemoveAt(i);
                    __codes.Insert(i, new CodeInstruction(OpCodes.Ldc_R4, maxCap));
                }
            }
        }

        public static void getAllIndexes(ref List<CodeInstruction> __codes, ref int[] __BloodPumping, ref int[] __Breathing, ref int[] __BloodFiltration)
        {
            __BloodPumping = getIndexes(__codes);
            __Breathing = getIndexes(__codes, __BloodPumping[0]);
            __BloodFiltration = getIndexes(__codes, __Breathing[0]);
        }


        public static void removeLimit(ref List<CodeInstruction> __codes,int[] ItemRange)
        {
            for (var i = ItemRange[0]; i < ItemRange[1]; i++)
            {
                if (__codes[i].opcode == OpCodes.Ldc_R4)
                {
                    if ((float)__codes[i].operand == 1 && __codes[i + 1].opcode == OpCodes.Call)
                    {
                        __codes[i].opcode = OpCodes.Nop;
                        __codes.RemoveRange(i + 1, 1);
                    }
                }
            }
            

        }

        public static int[] getIndexes(List<CodeInstruction> codes,int startIndex=-1)
        {
            int endIndex = -1;

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && startIndex == -1)
                {
                    startIndex = i;   
                }
                else if(codes[i].opcode== OpCodes.Ldsfld)
                {
                    endIndex = i;
                }
            }

            return new int[] { startIndex, endIndex };
        }
    }
}
