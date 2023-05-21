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

    public class RegisterCommand : Command
    {
        public override string prefix => "register";

        public override string prefixHelp => "Register new client [1].";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.RegisterClientCommand;
    }

    public class RenewCommand : Command
    {
        public override string prefix => "renew";

        public override string prefixHelp => "Renews client [1].";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.RenewClientCommand;
    }

    public class UnrenewCommand : Command
    {
        public override string prefix => "unrenew";

        public override string prefixHelp => "Cancels client [1] from renewal service.";

        public override int parameterCount => 1;

        public override Action actionToDo => AdvancedCommands.UnrenewClientCommand;
    }

    public class ReloadCommand : Command
    {
        public override string prefix => "reload";

        public override string prefixHelp => "Reloads server configurations.";

        public override int parameterCount => 0;

        public override Action actionToDo => SimpleCommands.ReloadCommandHandle;
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

        public override string prefixHelp => "Shows a list of the connected and registered clients.";

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
