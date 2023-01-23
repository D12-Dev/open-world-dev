using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldServerVerificator
{
    public static class Server
    {
        public static string mainFolderPath;
        public static string logsFolderPath;
        public static string usersFolderPath;

        public static string configFilePath;

        public static ConfigFile serverConfig;

        public static bool isActive = true;

        private static void Main()
        {
            StartProgram();

            while (isActive) { ListenForCommands(); }
        }

        private static void StartProgram()
        {
            ServerHandler.SetPaths();
            ServerHandler.SetCulture();
            ServerHandler.CheckConfigFile();

            ThreadHandler.GenerateServerThread(0);
            ThreadHandler.GenerateServerThread(1);
            ThreadHandler.GenerateServerThread(2);

            ServerHandler.UpdateTitle();
        }

        private static void ListenForCommands()
        {
            string command = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(command)) return;

            try
            {
                string commandBase = command.Split(' ')[0];
                string commandArgumentString = command.Remove(0, commandBase.Length);
                if (commandArgumentString.StartsWith(' ')) commandArgumentString = commandArgumentString.Remove(0, 1);

                string[] commandArgumentsArray;
                if (string.IsNullOrWhiteSpace(commandArgumentString)) commandArgumentsArray = new string[0];
                else commandArgumentsArray = commandArgumentString.Split(' ');

                Command toInvoke = null;
                foreach (Command cmd in SimpleCommands.commandArray) if (cmd.prefix == commandBase) toInvoke = cmd;
                foreach (Command cmd in AdvancedCommands.commandArray) if (cmd.prefix == commandBase) toInvoke = cmd;

                if (toInvoke == null) throw new Exception();
                else
                {
                    CommandHandler.parameterHolder = commandArgumentsArray;

                    if (!CommandHandler.CheckForRequirements(toInvoke))
                    {
                        ServerHandler.WriteToConsole($"Missing requirements for [{toInvoke.prefix}] command", ServerHandler.LogMode.Error);
                        return;
                    }

                    if (CommandHandler.CheckParameterCount(toInvoke))
                    {
                        try { toInvoke.actionToDo.Invoke(); }
                        catch { ServerHandler.WriteToConsole($"Unexpected error at [{toInvoke.prefix}] command", ServerHandler.LogMode.Error); }
                    }
                }
            }

            catch
            {
                ServerHandler.WriteToConsole($"Command [{command}] is not recognized by the program. " +
                $"Please try again", ServerHandler.LogMode.Error);
            }
        }
    }
}
