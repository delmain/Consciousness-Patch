using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection;

namespace Custom_Rimworld_Mod
{
	
	public class ConsciousnessPatches
	{
		[HarmonyReversePatch]
		[HarmonyPatch(typeof(PawnCapacityWorker_Consciousness), nameof(PawnCapacityWorker_Consciousness.CalculateCapacityLevel))]
		public static float Postfix(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors, ref float __result) { 
			Verse.PawnCapacityWorker newbase = new PawnCapacityWorker();

			float pump=(float)typeof(PawnCapacityWorker).GetMethod("CalculateCapacityAndRecord", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(newbase, new object[] { diffSet, PawnCapacityDefOf.BloodPumping, impactors });
			float breath = (float)typeof(PawnCapacityWorker).GetMethod("CalculateCapacityAndRecord", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(newbase, new object[] { diffSet, PawnCapacityDefOf.Breathing, impactors });
			float filtration = (float)typeof(PawnCapacityWorker).GetMethod("CalculateCapacityAndRecord", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(newbase, new object[] { diffSet, PawnCapacityDefOf.BloodFiltration, impactors });


			float num = PawnCapacityUtility.CalculateTagEfficiency(diffSet, BodyPartTagDefOf.ConsciousnessSource, float.MaxValue, default(FloatRange), impactors, -1f);
			float num2 = Mathf.Clamp(GenMath.LerpDouble(0.1f, 1f, 0f, 0.4f, diffSet.PainTotal), 0f, 0.4f);
			if ((double)num2 >= 0.01)
			{
				num -= num2;
				if (impactors != null)
				{
					impactors.Add(new PawnCapacityUtility.CapacityImpactorPain());
				}
			}
			num = Mathf.Lerp(num, num * pump, 0.2f);
			num = Mathf.Lerp(num, num * breath, 0.2f);
			return Mathf.Lerp(num, num * filtration, 0.1f);
		} }
	}

