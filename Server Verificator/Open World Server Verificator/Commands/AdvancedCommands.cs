using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenWorldServerVerificator
{
    public static class AdvancedCommands
    {
        public static RegisterCommand registerCommand = new RegisterCommand();
        public static RenewCommand renewCommand = new RenewCommand();
        public static UnrenewCommand unrenewCommand = new UnrenewCommand();

        public static Command[] commandArray = new Command[]
        {
            registerCommand,
            renewCommand,
            unrenewCommand
        };

        public static void RegisterClientCommand()
        {
            string username = CommandHandler.parameterHolder[0];

            ClientHandler.RegisterClient(username);
        }

        public static void RenewClientCommand()
        {
            string username = CommandHandler.parameterHolder[0];

            ClientHandler.RenewClient(username);
        }

        public static void UnrenewClientCommand()
        {
            string username = CommandHandler.parameterHolder[0];

            ClientHandler.UnrenewClient(username);
        }
    }
}
