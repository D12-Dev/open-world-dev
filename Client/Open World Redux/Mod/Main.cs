using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public class Main
    {
        public static Injections _Injections = new Injections();
        public static ModConfigs _modConfigs = new ModConfigs();

        [StaticConstructorOnStartup]
        public static class OpenWorld
        {
            static OpenWorld()
            {
                SetupCulture();
                SetupParameterPaths();
                LoadAutosaveDetails();
                FactionsCache.FindFactionDefsInGame();
            }

            private static void SetupCulture()
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);
            }

            private static void SetupParameterPaths()
            {
                FocusCache.mainFolderPath = Application.persistentDataPath;
                FocusCache.saveFolderPath = FocusCache.mainFolderPath + Path.DirectorySeparatorChar + "Saves";
                FocusCache.ModFolderPath = FocusCache.mainFolderPath + Path.DirectorySeparatorChar + "Open World Redux";
                FocusCache.loginDataFilePath = FocusCache.ModFolderPath + Path.DirectorySeparatorChar + "LoginData.json";

                if (!Directory.Exists(FocusCache.saveFolderPath)) Directory.CreateDirectory(FocusCache.saveFolderPath);
                if (!Directory.Exists(FocusCache.ModFolderPath)) Directory.CreateDirectory(FocusCache.ModFolderPath);
            }

            private static void LoadAutosaveDetails()
            {
                LoginDataFile newLoginData;
                if (File.Exists(FocusCache.loginDataFilePath)) newLoginData = Serializer.DeserializeFromFile<LoginDataFile>(FocusCache.loginDataFilePath);
                else newLoginData = new LoginDataFile();

                FocusCache.autosaveDays = newLoginData.AutosaveDays;
                FocusCache.autosaveInternalTicks = Mathf.RoundToInt(FocusCache.autosaveDays * 60000f);
                Serializer.SerializeToFile(FocusCache.loginDataFilePath, newLoginData);
            }
        }
    }
}
