using System.Collections.Generic;
using System;
using System.Net;
using UnityEngine;
using Verse;
using System.IO;

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
            if (listingStandard.ButtonTextLabeled("[When In Server] Server sync interval", $"[{FocusCache.autosaveDays}] Day/s"))
            {
                ShowFloatMenu();
            }
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

        private void ShowFloatMenu()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            List<Tuple<string, int>> savedServers = new List<Tuple<string, int>>()
            {
                Tuple.Create("1 Day", 1),
                Tuple.Create("2 Days", 2),
                Tuple.Create("3 Days", 3),
                Tuple.Create("5 Days", 5),
                Tuple.Create("7 Days", 7),
            };

            foreach (Tuple<string, int> tuple in savedServers)
            {
                FloatMenuOption item = new FloatMenuOption(tuple.Item1, delegate
                {
                    FocusCache.autosaveDays = tuple.Item2;
                    FocusCache.autosaveInternalTicks = Mathf.RoundToInt(tuple.Item2 * 60000f);

                    LoginDataFile newLoginData;
                    if (File.Exists(FocusCache.loginDataFilePath)) newLoginData = Serializer.DeserializeFromFile<LoginDataFile>(FocusCache.loginDataFilePath);
                    else newLoginData = new LoginDataFile();

                    newLoginData.AutosaveDays = FocusCache.autosaveDays;
                    Serializer.SerializeToFile(FocusCache.loginDataFilePath, newLoginData);
                });

                list.Add(item);
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}
