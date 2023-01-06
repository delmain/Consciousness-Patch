using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;
using UnityEngine;

namespace Consciousness_Patch.Patches
{
    public class ConsciousnessSettings : ModSettings
    {
        public ConsciousnessSettings()
        {
            ConsciousnessSettings.instance = this;
        }
        public static ConsciousnessSettings instance;

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.setCap, "setCap");
            Scribe_Values.Look<float>(ref this.maxCap, "maxCap",0.3f);
            Scribe_Values.Look<float>(ref this.percentageModifierBP, "percentageModifierBP", 0.2f);
            Scribe_Values.Look<float>(ref this.percentageModifierBR, "percentageModifierBR", 0.2f);
            Scribe_Values.Look<float>(ref this.percentageModifierBF, "percentageModifierBF", 0.1f);
            base.ExposeData();
        }
        public bool setCap;
        public float mCMultiplier=1000f;
        public float maxCap = 0.3f;
        public float pMMultiplier = 100f;
        public float percentageModifierBP = 0.2f;
        public float percentageModifierBR = 0.2f;
        public float percentageModifierBF = 0.1f;


    }

    public class ConsciousnessMod : Mod
    {
        public static ConsciousnessSettings settings;

        public ConsciousnessMod(ModContentPack content) : base(content)
        {
            ConsciousnessMod.settings = this.GetSettings<ConsciousnessSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("-----Placeholder Settings-----");
            listingStandard.CheckboxLabeled("Has Cap", ref settings.setCap, "Sets if there is a maximum cap for Blood Filtration, Blood Pumping and Breathing scaling.");
            listingStandard.Label("Maximum Cap: " +Math.Round(settings.maxCap*settings.mCMultiplier)+"%",-1, "The maximum cap until the stats no longer give any increases to Consciousness");
            settings.maxCap = listingStandard.Slider(settings.maxCap, 0.1f, 1f);
            listingStandard.Label("Blood Pumping Percentage Modifier: " + Math.Round(settings.percentageModifierBP*settings.pMMultiplier)+"%", -1, "How much each stat increases Consciousness." +
                "\n20% means that 150% Blood Pumping increases Consciousness by 10% (20% default)");
            settings.percentageModifierBP = listingStandard.Slider(settings.percentageModifierBP, 0.1f, 1f);
            listingStandard.Label("Blood Filtration Percentage Modifier: " + Math.Round(settings.percentageModifierBF * settings.pMMultiplier) + "%", -1, "How much each stat increases Consciousness." +
                "\n20% means that 150% Blood Pumping increases Consciousness by 10%. (10% default)");
            settings.percentageModifierBF = listingStandard.Slider(settings.percentageModifierBF, 0.1f, 1f);
            listingStandard.Label("Breathing Percentage Modifier: " + Math.Round(settings.percentageModifierBR * settings.pMMultiplier) + "%", -1, "How much each stat increases Consciousness." +
                "\n20% means that 150% Blood Pumping increases Consciousness by 10% (20% default)");
            settings.percentageModifierBR = listingStandard.Slider(settings.percentageModifierBR, 0.1f, 1f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
            ConsciousnessMod.settings.Write();
        }

        public override string SettingsCategory()
        {
            return "Consciousness Patch";
        }
    }






}
