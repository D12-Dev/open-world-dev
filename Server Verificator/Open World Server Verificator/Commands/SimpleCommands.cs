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
        public static AnnounceCommand announceCommand = new AnnounceCommand();
        public static ListCommand listCommand = new ListCommand();
        public static StatusCommand statusCommand = new StatusCommand();
        public static ExitCommand exitCommand = new ExitCommand();

        public static Command[] commandArray = new Command[]
        {
            helpCommand,
            reloadCommand,
            announceCommand,
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

        public static void ExitCommand()
        {
            Server.isActive = false;
        }
    }
}
