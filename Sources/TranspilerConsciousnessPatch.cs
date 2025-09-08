using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
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

            //Creates a Dictionary to easier change each setting.
            Dictionary<String,Dictionary<String,int>> mainMap = new Dictionary<String,Dictionary<String, int>>();
            AddMaptoMain(ref mainMap,"BloodPumping");
            AddMaptoMain(ref mainMap, "Breathing");
            AddMaptoMain(ref mainMap, "BloodFiltration");

            //The map indexes for all three maps are gathered and stored
            mainMap["BloodPumping"] = getMapIndexes(mainMap["BloodPumping"], codes);
            mainMap["Breathing"] = getMapIndexes(mainMap["Breathing"], codes,mainMap["BloodPumping"]["Percentage"]);
            mainMap["BloodFiltration"] = getMapIndexes(mainMap["BloodFiltration"], codes, mainMap["Breathing"]["Percentage"]);

            //OpCodes.Nop are set for each location.
            codes[mainMap["BloodFiltration"]["Cap"]].opcode = OpCodes.Nop;
            codes[mainMap["Breathing"]["Cap"]].opcode = OpCodes.Nop;
            codes[mainMap["BloodPumping"]["Cap"]].opcode = OpCodes.Nop;

            //Percentages are changed for each stat.
            ChangePercentage(ref codes, mainMap["BloodFiltration"]["Percentage"], Patches.ConsciousnessSettings.instance.percentageModifierBF);
            ChangePercentage(ref codes, mainMap["Breathing"]["Percentage"], Patches.ConsciousnessSettings.instance.percentageModifierBR);
            ChangePercentage(ref codes, mainMap["BloodPumping"]["Percentage"], Patches.ConsciousnessSettings.instance.percentageModifierBP);
            
            //If the user enables a cap, the cap will be changed to what the user set it to.
            //Otherwise the cap will be removed.
            if (Patches.ConsciousnessSettings.instance.setCap)
            {
                float maxCap = Patches.ConsciousnessSettings.instance.maxCap * Patches.ConsciousnessSettings.instance.mCMultiplier / 100;
                ChangeCap(ref codes, mainMap["BloodFiltration"]["Cap"], maxCap);
                ChangeCap(ref codes, mainMap["Breathing"]["Cap"], maxCap);
                ChangeCap(ref codes, mainMap["BloodPumping"]["Cap"], maxCap);
            }
            else
            {
                RemoveLimit(ref codes, mainMap["BloodFiltration"]["Cap"], mainMap["BloodFiltration"]["MathfMin"]);
                RemoveLimit(ref codes, mainMap["Breathing"]["Cap"], mainMap["Breathing"]["MathfMin"]);
                RemoveLimit(ref codes, mainMap["BloodPumping"]["Cap"], mainMap["BloodPumping"]["MathfMin"]);
            }
            
            Log.Message("Consciousness has been Patched");
            return codes.AsEnumerable();

        }

        //Creates a basic map with 0 indexes to manage it afterwards.
        public static void AddMaptoMain(ref Dictionary<String, Dictionary<String, int>> __mainMap, String name)
        {
            Dictionary<String, int> subMap = new Dictionary<String, int>();
            subMap["Cap"] = 0;
            subMap["MathfMin"] = 0;
            subMap["Percentage"] = 0;
            __mainMap[name] = subMap;
        }


        //Gets the indexes for each of the required things to change.
        //Cap is the location that determines how high each stat can go, before it gives no benefit anymore.
        //Mathfmin is the location of the Mathf.Min function, which sets the limit for each stat.
        //Percentage is the location of how much each stat scales.
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

        //Removes the limit by deleting the location of the Cap and MathfMin 
        public static void RemoveLimit(ref List<CodeInstruction> __codes,int cap,int mathfMin)
        {
            __codes.RemoveRange(cap+1, 1);
        }

        //Changes the percentage to a new value. 1f means every point over 100% increases consciousness by 1.
        public static void ChangePercentage(ref List<CodeInstruction> __codes,int percentageIndex,float percentage)
        {
            __codes.RemoveAt(percentageIndex);
            __codes.Insert(percentageIndex, new CodeInstruction(OpCodes.Ldc_R4, percentage));
        }

        //Sets a new cap. This cap applies to each stat it is set for. 1f would mean that it behaves like vanilla.
        //2f means it scales up to 200%, before cutting of.
        public static void ChangeCap(ref List<CodeInstruction> __codes,int capIndex, float maxCap)
        {
            __codes.RemoveAt(capIndex+1);
            __codes.Insert(capIndex+1, new CodeInstruction(OpCodes.Ldc_R4, maxCap));
        }
    }
}
