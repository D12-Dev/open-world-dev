﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class SimpleCommands
    {
        public static HelpCommand helpCommand = new HelpCommand();
        public static ReloadCommand reloadCommand = new ReloadCommand();
        public static ReconnectCommand reconnectCommand = new ReconnectCommand();
        public static AnnounceCommand announceCommand = new AnnounceCommand();
        public static EventsCommand eventsCommand = new EventsCommand();
        public static ListCommand listCommand = new ListCommand();
        public static StatusCommand statusCommand = new StatusCommand();
        public static CleanupCommand cleanupCommand = new CleanupCommand();
        public static ExitCommand exitCommand = new ExitCommand();

        public static Command[] commandArray = new Command[]
        {
            helpCommand,
            reloadCommand,
            reconnectCommand,
            announceCommand,
            eventsCommand,
            listCommand,
            statusCommand,
            cleanupCommand,
            exitCommand
        };

        public static void HelpCommandHandle()
        {
            ServerHandler.WriteToConsole($"List of available commands [{commandArray.Count() + AdvancedCommands.commandArray.Count()}]", ServerHandler.LogMode.Title);

            ServerHandler.WriteToConsole($"Simple Commands [{commandArray.Count()}]", ServerHandler.LogMode.Title);
            foreach (Command cmd in commandArray)
            {
                ServerHandler.WriteToConsole($"- {cmd.prefix}", ServerHandler.LogMode.Warning);
                ServerHandler.WriteToConsole($"+ {cmd.prefixHelp}");
            }

            ServerHandler.WriteToConsole($"Advanced Commands [{AdvancedCommands.commandArray.Count()}]", ServerHandler.LogMode.Title);
            foreach (Command cmd in AdvancedCommands.commandArray)
            {
                ServerHandler.WriteToConsole($"- {cmd.prefix}", ServerHandler.LogMode.Warning);
                ServerHandler.WriteToConsole($"+ {cmd.prefixHelp}");
            }
        }

        public static void ReloadCommandHandle()
        {
            ServerHandler.CheckConfigFile();
            ServerHandler.CheckAuthFile();
            ServerHandler.CheckDeepSettingsFile();
            ServerHandler.CheckValuesFile();
            ServerHandler.CheckDifficultyFile();
            ServerHandler.CheckWhitelistFile();

            ServerHandler.WriteToConsole("Configurations have been reloaded", ServerHandler.LogMode.Title);
        }

        public static void AnnounceCommand()
        {
            ServerHandler.WriteToConsole("Type the message to send:", ServerHandler.LogMode.Title);
            string toSend = Console.ReadLine();

            string[] contents = new string[] { toSend };
            Packet AnnouncementMessagePacket = new Packet("AnnouncementMessagePacket", contents);

            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach(ServerClient client in connectedClients)
            {
                Network.SendData(client, AnnouncementMessagePacket);
            }

            ServerHandler.WriteToConsole($"Sent announcement: [{toSend}]", ServerHandler.LogMode.Title);
        }

        public static void StatusCommand()
        {
            ServerHandler.WriteToConsole("Server status", ServerHandler.LogMode.Title);
            ServerHandler.WriteToConsole($"IP: {Network.localAddress}");
            ServerHandler.WriteToConsole($"Port: {Network.serverPort}");
            ServerHandler.WriteToConsole($"Max Players: {Network.maxPlayers}");
            ServerHandler.WriteToConsole($"Connected Players: {Network.connectedClients.Count}");
        }

        public static void ListCommand()
        {
            ServerHandler.WriteToConsole($"List of connected players [{Network.connectedClients.Count}]", ServerHandler.LogMode.Title);

            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach(ServerClient sc in connectedClients) ServerHandler.WriteToConsole(sc.Username);
        }

        public static void CleanupCommand()
        {
            string[] playerFiles = Directory.GetFiles(Server.playersFolderPath);
            foreach (string file in playerFiles)
            {
                FileInfo fi = new FileInfo(file);
                Debug.WriteLine(Path.GetFileNameWithoutExtension(fi.Name));
                if (fi.LastAccessTime < DateTime.Now.AddDays(-14))
                {
                    string userIDReference = Path.GetFileNameWithoutExtension(fi.Name);
                    DeleteSaveData(userIDReference);
                    DeleteSettlementData(userIDReference);
                    fi.Delete();
                }
            }

            string[] factionFiles = Directory.GetFiles(Server.factionsFolderPath);
            foreach (string file in factionFiles)
            {
                FileInfo fi = new FileInfo(file);
                Debug.WriteLine(Path.GetFileNameWithoutExtension(fi.Name));
                if (fi.LastAccessTime < DateTime.Now.AddDays(-14)) fi.Delete();
            }

            ServerHandler.WriteToConsole("Cleanup has been executed", ServerHandler.LogMode.Title);

            void DeleteSettlementData(string userReference)
            {
                string settlementFilePath = Server.settlementsFolderPath + Path.DirectorySeparatorChar + userReference + ".json";
                if (File.Exists(settlementFilePath)) File.Delete(settlementFilePath);
            }

            void DeleteSaveData(string userReference)
            {
                string saveFilePath = Server.savesFolderPath + Path.DirectorySeparatorChar + userReference + ".zipx";
                if (File.Exists(saveFilePath)) File.Delete(saveFilePath);
            }
        }

        public static void EventsCommand()
        {
            ServerHandler.WriteToConsole($"List of available events [{10}]", ServerHandler.LogMode.Title);
            ServerHandler.WriteToConsole($"To use, reference the event using it's ID number.", ServerHandler.LogMode.Warning);
            ServerHandler.WriteToConsole($"{0} - Raid");
            ServerHandler.WriteToConsole($"{1} - Infestation");
            ServerHandler.WriteToConsole($"{2} - Mechanoid Cluster");
            ServerHandler.WriteToConsole($"{3} - Toxic Fallout");
            ServerHandler.WriteToConsole($"{4} - Manhunter Pack");
            ServerHandler.WriteToConsole($"{5} - Wanderer Pawn");
            ServerHandler.WriteToConsole($"{6} - Animal Pack");
            ServerHandler.WriteToConsole($"{7} - Space Chunks");
            ServerHandler.WriteToConsole($"{8} - Generate Quest");
            ServerHandler.WriteToConsole($"{9} - Trader Caravan");
        }

        public static void ReconnectCommandHandle()
        {
            ThreadHandler.GenerateServerThread(3);
        }

        public static void ExitCommand()
        {
            Server.isActive = false;
        }
    }
}
