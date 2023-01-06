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

            Dictionary<String,Dictionary<String,int>> mainMap = new Dictionary<String,Dictionary<String, int>>();
            AddMaptoMain(ref mainMap,"BloodPumping");
            AddMaptoMain(ref mainMap, "Breathing");
            AddMaptoMain(ref mainMap, "BloodFiltration");

            mainMap["BloodPumping"]=getMapIndexes(mainMap["BloodPumping"], codes);
            mainMap["Breathing"] = getMapIndexes(mainMap["Breathing"], codes,mainMap["BloodPumping"]["Percentage"]);
            mainMap["BloodFiltration"] = getMapIndexes(mainMap["BloodFiltration"], codes, mainMap["Breathing"]["Percentage"]);

            /*
            Log.Message("BloodPumping: "+" Percentage: "+ mainMap["BloodPumping"]["Percentage"]+
                " Cap: "+ mainMap["BloodPumping"]["Cap"]+" MathfMin: "+ mainMap["BloodPumping"]["MathfMin"]);
            Log.Message("Breathing: " + " Percentage: " + mainMap["Breathing"]["Percentage"] +
                " Cap: " + mainMap["Breathing"]["Cap"] + " MathfMin: " + mainMap["Breathing"]["MathfMin"]);
            Log.Message("BloodFiltration: " + " Percentage: " + mainMap["BloodFiltration"]["Percentage"] +
                " Cap: " + mainMap["BloodFiltration"]["Cap"] + " MathfMin: " + mainMap["BloodFiltration"]["MathfMin"]);
            */

            //changePercentages(ref codes, mainMap["BloodFiltration"]["Percentage"], Patches.ConsciousnessSettings.instance.percentageModifierBF);
            //changePercentages(ref codes, mainMap["Breathing"]["Percentage"], Patches.ConsciousnessSettings.instance.percentageModifierBR);
            //changePercentages(ref codes, mainMap["BloodPumping"]["Percentage"], Patches.ConsciousnessSettings.instance.percentageModifierBP);

            if (Patches.ConsciousnessSettings.instance.setCap)
            {
                float maxCap = Patches.ConsciousnessSettings.instance.maxCap * Patches.ConsciousnessSettings.instance.mCMultiplier / 100;
                changeCaps(ref codes, mainMap["BloodFiltration"]["Cap"], maxCap);
                changeCaps(ref codes, mainMap["Breathing"]["Cap"], maxCap);
                changeCaps(ref codes, mainMap["BloodPumping"]["Cap"], maxCap);
            }
            else
            {

                removeLimits(ref codes, mainMap["BloodFiltration"]["Cap"], mainMap["BloodFiltration"]["MathfMin"]);
                removeLimits(ref codes, mainMap["Breathing"]["Cap"], mainMap["Breathing"]["MathfMin"]);
                removeLimits(ref codes, mainMap["BloodPumping"]["Cap"], mainMap["BloodPumping"]["MathfMin"]);


            }

            return codes.AsEnumerable();

        }

        public static void AddMaptoMain(ref Dictionary<String, Dictionary<String, int>> __mainMap, String name)
        {
            Dictionary<String, int> subMap = new Dictionary<String, int>();
            subMap["Cap"] = 0;
            subMap["MathfMin"] = 0;
            subMap["Percentage"] = 0;
            __mainMap[name] = subMap;
        }


        public static Dictionary<String, int> getMapIndexes(Dictionary<String, int> subMap, List<CodeInstruction> codes,int start=0)
        {
            for (var i = start; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4)
                {
                    if ((float)codes[i].operand == 1 && codes[i + 1].opcode == OpCodes.Call)
                    {
                        subMap["Cap"] = i;
                        subMap["MathfMin"] = i+1;
                            subMap["Percentage"] = i + 3;
                        break;
                    }

                }
            }
            return subMap;
        }

        public static void removeLimits(ref List<CodeInstruction> __codes,int cap,int mathfMin)
        {
            __codes[cap].opcode = OpCodes.Nop;
            __codes.RemoveRange(cap+1, 1);
        }

        public static void changePercentages(ref List<CodeInstruction> __codes,int percentageIndex,float percentage)
        {
            __codes[percentageIndex].opcode = OpCodes.Nop;
            __codes.RemoveAt(percentageIndex+1);
            __codes.Insert(percentageIndex+1, new CodeInstruction(OpCodes.Ldc_R4, percentage));
        }

        public static void changeCaps(ref List<CodeInstruction> __codes,int capIndex, float maxCap)
        {
            __codes[capIndex].opcode = OpCodes.Nop;
            __codes.RemoveAt(capIndex+1);
            __codes.Insert(capIndex+1, new CodeInstruction(OpCodes.Ldc_R4, maxCap));
        }
    }
}
