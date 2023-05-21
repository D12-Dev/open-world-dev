using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldServerVerificator
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
            Console.Title = $"Server Verification Tool [{Network.connectedClients.Count}]";
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
            Server.logsFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Logs";
            Server.usersFolderPath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Users";

            Server.configFilePath = Server.mainFolderPath + Path.DirectorySeparatorChar + "Config.json";

            if (!Directory.Exists(Server.mainFolderPath)) Directory.CreateDirectory(Server.mainFolderPath);
            if (!Directory.Exists(Server.logsFolderPath)) Directory.CreateDirectory(Server.logsFolderPath);
            if (!Directory.Exists(Server.usersFolderPath)) Directory.CreateDirectory(Server.usersFolderPath);

            WriteToConsole($"Main folder path [{Server.mainFolderPath}]");
            WriteToConsole($"Logs folder path [{Server.logsFolderPath}]");
            WriteToConsole($"Users folder path [{Server.usersFolderPath}]");
        }

        public static void CheckConfigFile()
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

            Network.localAddress = IPAddress.Parse(configFile.LocalAddress);
            Network.serverPort = configFile.ServerPort;

            WriteToConsole($"Local Address [{Network.localAddress}]");
            WriteToConsole($"Server Port [{Network.serverPort}]");
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
