using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenWorldServerVerificator
{
    public static class AdvancedCommands
    {
        public static KickCommand kickCommand = new KickCommand();
        public static BanCommand banCommand = new BanCommand();
        public static PardonCommand pardonCommand = new PardonCommand();

        public static Command[] commandArray = new Command[]
        {
            kickCommand,
            banCommand,
            pardonCommand,
        };

        public static void BanCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
        }

        public static void PardonCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
        }

        public static void KickCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
        }
    }
}
