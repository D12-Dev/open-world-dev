using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OpenWorldServerVerificator
{
    public static class CommandHandler
    {
        public static string[] parameterHolder;

        public static bool CheckParameterCount(Command commandToCheck)
        {
            if (commandToCheck.parameterCount == parameterHolder.Length) return true;
            else
            {
                ServerHandler.WriteToConsole($"Command [{commandToCheck.prefix}] had incorrect parameter count. " +
                    $"Asked for [{commandToCheck.parameterCount}] and got [{parameterHolder.Length}]", ServerHandler.LogMode.Error);

                return false;
            }
        }

        public static bool CheckForRequirements(Command commandToCheck)
        {
            return true;
        }
    }

    public abstract class Command
    {
        public virtual string prefix { get; set; }

        public virtual string prefixHelp { get; set; }

        public virtual int parameterCount { get; set; }

        public virtual Action actionToDo { get; set; }
    }

    public class HelpCommand : Command
    {
        public override string prefix => "help";

        public override string prefixHelp => "Shows a list of available commands.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.HelpCommandHandle;
    }

    public class ReloadCommand : Command
    {
        public override string prefix => "reload";

        public override string prefixHelp => "Reloads server configurations.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.ReloadCommandHandle;
    }

    public class BanCommand : Command
    {
        public override string prefix => "ban";

        public override string prefixHelp => "Bans player [1].";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.BanCommandHandle;
    }

    public class PardonCommand : Command
    {
        public override string prefix => "pardon";

        public override string prefixHelp => "Pardons player [1].";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.PardonCommandHandle;
    }

    public class KickCommand : Command
    {
        public override string prefix => "kick";

        public override string prefixHelp => "Kicks player [1].";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.KickCommandHandle;
    }

    public class AnnounceCommand : Command
    {
        public override string prefix => "announce";

        public override string prefixHelp => "Sends a message to every connected player.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.AnnounceCommand;
    }

    public class StatusCommand : Command
    {
        public override string prefix => "status";

        public override string prefixHelp => "Shows information about the server.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.StatusCommand;
    }

    public class ListCommand : Command
    {
        public override string prefix => "list";

        public override string prefixHelp => "Shows a list of the connected players.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.ListCommand;
    }

    public class ExitCommand : Command
    {
        public override string prefix => "exit";

        public override string prefixHelp => "Exits the server application.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.ExitCommand;
    }
}
