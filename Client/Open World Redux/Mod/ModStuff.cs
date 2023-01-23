using System.Net;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public class ModStuff : Mod
    {
        ModConfigs settings;

        public ModStuff(ModContentPack content) : base(content)
        {
            settings = GetSettings<ModConfigs>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("Multiplayer Parameters");
            listingStandard.CheckboxLabeled("Auto deny trades", ref settings.tradeBool, "Automatically denies trades");
            listingStandard.CheckboxLabeled("Hide production site messages", ref settings.hideProductionSite, "Hides production site messages");
            listingStandard.CheckboxLabeled("Very secret stuff", ref settings.secretBool, "Secret");
            if (listingStandard.ButtonTextLabeled("[When In Server] Delete current progress", "Delete"))
            {
                ResetServerProgress();
            }

            listingStandard.GapLine();
            listingStandard.Label("External Sources");
            if (listingStandard.ButtonTextLabeled("Check latest mod changelogs", "Open"))
            {
                try { System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/changelog/2768146099"); } catch { }
            }
            if (listingStandard.ButtonTextLabeled("Join Open World's discord community", "Open"))
            {
                try { System.Diagnostics.Process.Start("https://discord.gg/SNtVjR6bqU"); } catch { }
            }

            listingStandard.GapLine();
            listingStandard.Label("Mod Details");
            listingStandard.Label("Running version: " + FocusCache.versionCode);
            listingStandard.Label("Latest Version Available: 1.0.0");

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() { return "Open World"; }

        private void ResetServerProgress()
        {
            if (!BooleanCache.isConnectedToServer) return;
            else Find.WindowStack.Add(new OW_MPConfirmResetSave());
        }
    }
}
