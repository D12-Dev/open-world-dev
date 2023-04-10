using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OpenWorldReduxServer
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
    public class ShutDownCommand : Command
    {
        public override string prefix => "shutdown";

        public override string prefixHelp => "Safely shutdown the server.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.ShutdownCommandHandle;
    }


    public class OpCommand : Command
    {
        public override string prefix => "op";

        public override string prefixHelp => "Grants admin privileges to player [1].";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.OpCommandHandle;
    }

    public class DeopCommand : Command
    {
        public override string prefix => "deop";

        public override string prefixHelp => "Takes away admin privileges from player [1].";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.DeopCommandHandle;
    }

    public class InspectCommand : Command
    {
        public override string prefix => "inspect";

        public override string prefixHelp => "Gets all the details of player [1].";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.InspectCommandHandle;
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

    public class CleanupCommand : Command
    {
        public override string prefix => "cleanup";

        public override string prefixHelp => "Deletes all data of players that have been afk for more than 7d.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.CleanupCommand;
    }

    public class InvokeCommand : Command
    {
        public override string prefix => "invoke";

        public override string prefixHelp => "Sends a black market event [2] to player [1].";

        public override int parameterCount => 2;

        public override Action actionToDo => AdvancedCommands.InvokeCommand;
    }

    public class EventsCommand : Command
    {
        public override string prefix => "events";

        public override string prefixHelp => "Shows a list of current usable black market events.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.EventsCommand;
    }

    public class ExitCommand : Command
    {
        public override string prefix => "exit";

        public override string prefixHelp => "Exits the server application.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.ExitCommand;
    }
}
