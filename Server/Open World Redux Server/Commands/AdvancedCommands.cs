using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OpenWorldReduxServer
{
    public static class AdvancedCommands
    {
        public static OpCommand opCommand = new OpCommand();
        public static DeopCommand deopCommand = new DeopCommand();
        public static KickCommand kickCommand = new KickCommand();
        public static BanCommand banCommand = new BanCommand();
        public static PardonCommand pardonCommand = new PardonCommand();
        public static InspectCommand inspectCommand = new InspectCommand();
        public static InvokeCommand invokeCommand = new InvokeCommand();

        public static Command[] commandArray = new Command[]
        {
            opCommand,
            deopCommand,
            kickCommand,
            banCommand,
            pardonCommand,
            inspectCommand,
            invokeCommand
        };

        public static void OpCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromConnected(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                toGet.IsAdmin = true;
                ClientHandler.SaveClient(toGet);
                ServerHandler.WriteToConsole($"Player [{toGet.Username}] has become OP", ServerHandler.LogMode.Warning);

                Packet OpCommandPacket = new Packet("OpCommandPacket");
                Network.SendData(toGet, OpCommandPacket);
            }
        }

        public static void DeopCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromConnected(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                toGet.IsAdmin = false;
                ClientHandler.SaveClient(toGet);
                ServerHandler.WriteToConsole($"Player [{toGet.Username}] has lost OP", ServerHandler.LogMode.Warning);

                Packet DeopCommandPacket = new Packet("DeopCommandPacket");
                Network.SendData(toGet, DeopCommandPacket);
            }
        }

        public static void InspectCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromConnected(username);
            
            if (toGet == null) toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                ServerHandler.WriteToConsole($"List of available details for [{toGet.Username}]", ServerHandler.LogMode.Title);
                ServerHandler.WriteToConsole($"Username: [{toGet.Username}]");
                ServerHandler.WriteToConsole($"Settlement: [{toGet.SettlementTile}]");
                ServerHandler.WriteToConsole($"Saved IP: [{toGet.SavedIP}]");
                ServerHandler.WriteToConsole($"Saved ID: [{toGet.SavedID}]");
                ServerHandler.WriteToConsole($"Faction: [{toGet.FactionName}]");
                ServerHandler.WriteToConsole($"Event Protected: [{toGet.isEventProtected}]");
                ServerHandler.WriteToConsole($"Admin: [{toGet.IsAdmin}]");
                ServerHandler.WriteToConsole($"Banned: [{toGet.IsBanned}]");
            }
        }

        public static void BanCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromConnected(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                toGet.IsBanned = true;
                toGet.disconnectFlag = true;
                ClientHandler.SaveClient(toGet);
                ServerHandler.WriteToConsole($"Player [{username}] has been banned", ServerHandler.LogMode.Warning);
            }
        }

        public static void PardonCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                toGet.IsBanned = false;
                ClientHandler.SaveClient(toGet);
                ServerHandler.WriteToConsole($"Player [{username}] has been pardoned", ServerHandler.LogMode.Warning);
            }
        }

        public static void KickCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromConnected(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else 
            {
                toGet.disconnectFlag = true;
                ServerHandler.WriteToConsole($"Player [{username}] has been kicked", ServerHandler.LogMode.Warning);
            }
        }

        public static void InvokeCommand()
        {
            string username = CommandHandler.parameterHolder[0];
            string eventID = CommandHandler.parameterHolder[1];

            ServerClient toGet = ClientHandler.GetClientFromConnected(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                string[] contents = new string[] { eventID };
                Packet SendBlackMarketEventPacket = new Packet("SendBlackMarketEventPacket", contents);
                Network.SendData(toGet, SendBlackMarketEventPacket);

                ServerHandler.WriteToConsole($"Sent event to player [{username}]", ServerHandler.LogMode.Warning);
            }
        }
    }
}
