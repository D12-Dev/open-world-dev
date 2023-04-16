using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    public static class ServerHandler
    {
        public enum LogMode { Normal, Warning, Error, Title }

        public static Dictionary<LogMode, ConsoleColor> colorDictionary = new Dictionary<LogMode, ConsoleColor>
        {
            { LogMode.Normal, ConsoleColor.White },
            { LogMode.Warning, ConsoleColor.Yellow },
            { LogMode.Error, ConsoleColor.Red },
            { LogMode.Title, ConsoleColor.Green }
        };

        public static void UpdateTitle()
        {
            Console.Title = $"Open World Server v{Server.serverVersion} [{Network.connectedClients.Count}/{Network.maxPlayers}]";
        }

        public static void SetCulture()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);

            WriteToConsole($"Using culture [{CultureInfo.CurrentCulture}]");
        }

        public static void SetPaths()
        {
            Server.mainFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            Server.playersFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Players";
            Server.BackupFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Backups";
            Server.savesFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Saves";
            Server.settlementsFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Settlements";
            Server.factionsFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Factions";
            Server.dataFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Data";
            Server.WorldGenDataPath = Server.dataFolderPath + Path.DirectorySeparatorChar + "WorldGenData";
            Server.logsFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Logs";
            Server.enforcedModsFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Mods enforced";
            Server.whitelistedModsFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Mods whitelisted";
            Server.blacklistedModsFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Mods blacklisted";

            Server.authFilePath = Server.dataFolderPath + Path.DirectorySeparatorChar + "AuthFile.json";
            Server.configFilePath = Server.dataFolderPath + Path.DirectorySeparatorChar + "Config.json";
            Server.deepConfigsFilePath = Server.dataFolderPath + Path.DirectorySeparatorChar + "Deep Config.json";
            Server.valuesFilePath = Server.dataFolderPath + Path.DirectorySeparatorChar + "Values.json";
            Server.difficultyFilePath = Server.dataFolderPath + Path.DirectorySeparatorChar + "Difficulty.json";
            Server.whitelistFilePath = Server.dataFolderPath + Path.DirectorySeparatorChar + "Whitelist.json";
            Server.CachedVarsPath = Server.dataFolderPath + Path.DirectorySeparatorChar + "CachedValues.json";
            if (!Directory.Exists(Server.enforcedModsFolderPath)) Directory.CreateDirectory(Server.enforcedModsFolderPath);
            if (!Directory.Exists(Server.whitelistedModsFolderPath)) Directory.CreateDirectory(Server.whitelistedModsFolderPath);
            if (!Directory.Exists(Server.blacklistedModsFolderPath)) Directory.CreateDirectory(Server.blacklistedModsFolderPath);
            if (!Directory.Exists(Server.playersFolderPath)) Directory.CreateDirectory(Server.playersFolderPath);
            if (!Directory.Exists(Server.savesFolderPath)) Directory.CreateDirectory(Server.savesFolderPath);
            if (!Directory.Exists(Server.settlementsFolderPath)) Directory.CreateDirectory(Server.settlementsFolderPath);
            if (!Directory.Exists(Server.factionsFolderPath)) Directory.CreateDirectory(Server.factionsFolderPath);
            if (!Directory.Exists(Server.dataFolderPath)) Directory.CreateDirectory(Server.dataFolderPath);
            if (!Directory.Exists(Server.WorldGenDataPath)) Directory.CreateDirectory(Server.WorldGenDataPath);
            if (!Directory.Exists(Server.logsFolderPath)) Directory.CreateDirectory(Server.logsFolderPath);
            if (!Directory.Exists(Server.BackupFolderPath)) Directory.CreateDirectory(Server.BackupFolderPath);


            WriteToConsole($"Main folder path [{Server.mainFolderPath}]");
            WriteToConsole($"Enforced mods folder path [{Server.enforcedModsFolderPath}]");
            WriteToConsole($"Whitelisted mods folder path [{Server.whitelistedModsFolderPath}]");
            WriteToConsole($"Blacklisted Mods folder path [{Server.blacklistedModsFolderPath}]");
            WriteToConsole($"Players folder path [{Server.playersFolderPath}]");
            WriteToConsole($"Backup folder path [{Server.BackupFolderPath}]");
            WriteToConsole($"Saves folder path [{Server.savesFolderPath}]");
            WriteToConsole($"Settlements folder path [{Server.settlementsFolderPath}]");
            WriteToConsole($"Factions folder path [{Server.settlementsFolderPath}]");
            WriteToConsole($"Data folder path [{Server.dataFolderPath}]");
            WriteToConsole($"World Gen Data folder path [{Server.dataFolderPath}]");
            WriteToConsole($"Logs folder path [{Server.WorldGenDataPath}]");
        }

        public static void CheckAuthFile()
        {
            AuthFile authFile;

            if (File.Exists(Server.authFilePath))
            {
                authFile = Serializer.DeserializeFromFile<AuthFile>(Server.authFilePath);
            }

            else
            {
                authFile = new AuthFile();

                Serializer.SerializeToFile(authFile, Server.authFilePath);
            }

            Server.serverAuth = authFile;
        }

        public static void CheckConfigFile(bool firstTime = false)
        {
            ConfigFile configFile;

            if (File.Exists(Server.configFilePath))
            {
                configFile = Serializer.DeserializeFromFile<ConfigFile>(Server.configFilePath);
            }

            else
            {
                configFile = new ConfigFile();

                Serializer.SerializeToFile(configFile, Server.configFilePath);
            }

            Server.serverConfig = configFile;

            if (firstTime)
            {
                Network.localAddress = IPAddress.Parse(configFile.LocalAddress);
                Network.serverPort = configFile.ServerPort;

                WriteToConsole($"Local address [{Network.localAddress}]");
                WriteToConsole($"Server port [{Network.serverPort}]");
            }

            Network.maxPlayers = configFile.MaxPlayers;

            PopulateConfigFile();
        }

        private static void PopulateConfigFile()
        {
            Server.serverConfig.enforcedMods.Clear();
            Server.serverConfig.whitelistedMods.Clear();
            Server.serverConfig.blacklistedMods.Clear();

            string[] enforcedModsDeflate = Directory.GetFiles(Server.enforcedModsFolderPath);
            foreach (string mod in enforcedModsDeflate) Server.serverConfig.enforcedMods.Add(Path.GetFileNameWithoutExtension(mod));

            string[] whitelistedModsDeflate = Directory.GetFiles(Server.whitelistedModsFolderPath);
            foreach (string mod in whitelistedModsDeflate) Server.serverConfig.whitelistedMods.Add(Path.GetFileNameWithoutExtension(mod));

            string[] blacklistedModsDeflate = Directory.GetFiles(Server.blacklistedModsFolderPath);
            foreach (string mod in blacklistedModsDeflate) Server.serverConfig.blacklistedMods.Add(Path.GetFileNameWithoutExtension(mod));
        }

        public static void CheckDeepSettingsFile()
        {
            DeepConfigFile deepSettingsFile = null;

            if (File.Exists(Server.deepConfigsFilePath))
            {
                deepSettingsFile = Serializer.DeserializeFromFile<DeepConfigFile>(Server.deepConfigsFilePath);
            }

            else
            {
                deepSettingsFile = new DeepConfigFile();

                Serializer.SerializeToFile(deepSettingsFile, Server.deepConfigsFilePath);
            }

            Server.serverDeepConfigs = deepSettingsFile;
        }

        public static void CheckValuesFile()
        {
            ServerValuesFile valuesFile = null;

            if (File.Exists(Server.valuesFilePath))
            {
                valuesFile = Serializer.DeserializeFromFile<ServerValuesFile>(Server.valuesFilePath);
            }

            else
            {
                valuesFile = new ServerValuesFile();

                Serializer.SerializeToFile(valuesFile, Server.valuesFilePath);
            }

            Server.serverValues = valuesFile;
        }
        public static void CheckCachedVarsFile()
        {
            CachedVariableFile CachedVars = null;
            if (File.Exists(Server.CachedVarsPath))
            {
                CachedVars = Serializer.DeserializeFromFile<CachedVariableFile>(Server.CachedVarsPath);
            }

            else
            {
                CachedVars = new CachedVariableFile();
                Serializer.SerializeToFile(CachedVars, Server.CachedVarsPath);
            }

            Server.cachedVariables = CachedVars;
        }

        public static void CheckDifficultyFile()
        {
            DifficultyFile difficultyFile = null;

            if (File.Exists(Server.difficultyFilePath))
            {
                difficultyFile = Serializer.DeserializeFromFile<DifficultyFile>(Server.difficultyFilePath);
            }

            else
            {
                difficultyFile = new DifficultyFile();

                Serializer.SerializeToFile(difficultyFile, Server.difficultyFilePath);
            }

            Server.serverDifficulty = difficultyFile;
        }

        public static void CheckWhitelistFile()
        {
            WhitelistFile whitelistFile = null;

            if (File.Exists(Server.whitelistFilePath))
            {
                whitelistFile = Serializer.DeserializeFromFile<WhitelistFile>(Server.whitelistFilePath);
            }

            else
            {
                whitelistFile = new WhitelistFile();

                Serializer.SerializeToFile(whitelistFile, Server.whitelistFilePath);
            }

            Server.whitelist = whitelistFile;
        }

        public static void WriteToConsole(string text, LogMode mode = LogMode.Normal)
        {
            WriteToLogs(text);

            Console.ForegroundColor = colorDictionary[mode];
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] | " + text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteToLogs(string toLog)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(toLog);
            stringBuilder.Append(Environment.NewLine);

            DateTime dateTime = DateTime.Now.Date;
            string nowFileName = (dateTime.Month + "-" + dateTime.Day + "-" + dateTime.Year).ToString();
            string nowFullPath = Server.logsFolderPath + Path.DirectorySeparatorChar + nowFileName + ".txt";

            File.AppendAllText(nowFullPath, stringBuilder.ToString());
            stringBuilder.Clear();
        }
    }
}
