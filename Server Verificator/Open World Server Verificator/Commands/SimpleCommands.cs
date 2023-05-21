using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OpenWorldServerVerificator
{
    public static class SimpleCommands
    {
        public static HelpCommand helpCommand = new HelpCommand();
        public static ReloadCommand reloadCommand = new ReloadCommand();
        public static ListCommand listCommand = new ListCommand();
        public static StatusCommand statusCommand = new StatusCommand();
        public static ExitCommand exitCommand = new ExitCommand();

        public static Command[] commandArray = new Command[]
        {
            helpCommand,
            reloadCommand,
            listCommand,
            statusCommand,
            exitCommand
        };

        public static void HelpCommandHandle()
        {
            ServerHandler.WriteToConsole("List of available commands", ServerHandler.LogMode.Title);

            foreach(Command cmd in commandArray)
            {
                ServerHandler.WriteToConsole($"- {cmd.prefix}", ServerHandler.LogMode.Warning);
                ServerHandler.WriteToConsole($"+ {cmd.prefixHelp}");
            }

            foreach (Command cmd in AdvancedCommands.commandArray)
            {
                ServerHandler.WriteToConsole($"- {cmd.prefix}", ServerHandler.LogMode.Warning);
                ServerHandler.WriteToConsole($"+ {cmd.prefixHelp}");
            }
        }

        public static void ReloadCommandHandle()
        {
            ServerHandler.WriteToConsole("Configurations have been reloaded", ServerHandler.LogMode.Title);
        }

        public static void StatusCommand()
        {
            ServerHandler.WriteToConsole("Server status", ServerHandler.LogMode.Title);
            ServerHandler.WriteToConsole($"IP: {Network.localAddress}");
            ServerHandler.WriteToConsole($"Port: {Network.serverPort}");
            ServerHandler.WriteToConsole($"Connected Clients: {Network.connectedClients.Count}");
        }

        public static void ListCommand()
        {
            string[] clientFiles = Directory.GetFiles(Server.usersFolderPath);
            ServerHandler.WriteToConsole($"List of registered clients [{clientFiles.Count()}]", ServerHandler.LogMode.Title);
            foreach(string file in clientFiles)
            {
                ServerClient toGet = Serializer.DeserializeFromFile<ServerClient>(file);
                ServerHandler.WriteToConsole(toGet.Username);
            }

            ServerHandler.WriteToConsole($"List of connected clients [{Network.connectedClients.Count}]", ServerHandler.LogMode.Title);
            ServerClient[] connectedClients = Network.connectedClients.ToArray();
            foreach(ServerClient sc in connectedClients) ServerHandler.WriteToConsole(sc.Username);
        }

        public static void ExitCommand()
        {
            Server.isActive = false;
        }
    }
}
