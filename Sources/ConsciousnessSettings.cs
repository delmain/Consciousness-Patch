using System;
using Verse;
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
        //Multiplier to display the stats properly to the user
        public float mCMultiplier=1000f;
        public float maxCap = 0.3f;
        //Multiplier to display the stats properly to the user
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

        //Creates all the settings for the mod. 
        public override void DoSettingsWindowContents(Rect inRect)
        {

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("ConPat_Settings_Header".Translate());
            listingStandard.CheckboxLabeled("ConPat_HasCap_Label".Translate(), ref settings.setCap, "ConPat_HasCap_Tooltip".Translate());
            listingStandard.Label("ConPat_MaxCap_Label".Translate(Math.Round(settings.maxCap*settings.mCMultiplier)), -1f, "ConPat_MaxCap_Tooltip".Translate());
            settings.maxCap = listingStandard.Slider(settings.maxCap, 0.1f, 1f);
            listingStandard.Label("ConPat_BloodPumping_Label".Translate(Math.Round(settings.percentageModifierBP*settings.pMMultiplier)), -1f, "ConPat_BloodPumping_Tooltip".Translate());
            settings.percentageModifierBP = listingStandard.Slider(settings.percentageModifierBP, 0.1f, 1f);
            listingStandard.Label("ConPat_BloodFiltration_Label".Translate(Math.Round(settings.percentageModifierBF * settings.pMMultiplier)), -1f, "ConPat_BloodFiltration_Tooltip".Translate());
            settings.percentageModifierBF = listingStandard.Slider(settings.percentageModifierBF, 0.1f, 1f);
            listingStandard.Label("ConPat_Breathing_Label".Translate(Math.Round(settings.percentageModifierBR * settings.pMMultiplier)), -1f, "ConPat_Breathing_Tooltip".Translate());
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
