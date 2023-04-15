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
        public static TransferCommand transferCommand = new TransferCommand();

        public static Command[] commandArray = new Command[]
        {
            opCommand,
            deopCommand,
            kickCommand,
            banCommand,
            pardonCommand,
            inspectCommand,
            invokeCommand,
            transferCommand
        };

        public static void OpCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                toGet.IsAdmin = true;
                ClientHandler.SaveClient(toGet);
                ServerHandler.WriteToConsole($"Player [{toGet.Username}] has become OP", ServerHandler.LogMode.Warning);
                ServerClient ConnnectedClient = ClientHandler.GetClientFromConnected(username);
                if (ConnnectedClient != null)
                {
                    Packet OpCommandPacket = new Packet("OpCommandPacket");
                    Network.SendData(ConnnectedClient, OpCommandPacket);
                }
            }
        }
        public static void TransferCommandHandle() {
            string ply1username = CommandHandler.parameterHolder[0];
            string ply2username = CommandHandler.parameterHolder[1];
            ServerClient ply1 = ClientHandler.GetClientFromSave(ply1username);
            ServerClient ply2 = ClientHandler.GetClientFromSave(ply2username);
            ServerClient Connectedply1 = ClientHandler.GetClientFromConnected(ply1username);
            ServerClient Connectedply2 = ClientHandler.GetClientFromConnected (ply2username);
            if (ply1 == null) {
                ServerHandler.WriteToConsole($"Player [{ply1username}] was not found", ServerHandler.LogMode.Warning);
                return;
            }
            if (ply2 == null)
            {
                ServerHandler.WriteToConsole($"Player [{ply2username}] was not found", ServerHandler.LogMode.Warning);
                return;
            }
            if (Connectedply1 != null)
            {
                Connectedply1.disconnectsaveFlag = true;
                ServerHandler.WriteToConsole($"Player [{ply1username}] is being kicked", ServerHandler.LogMode.Warning);
            }
            if (Connectedply2 != null)
            {
                Connectedply2.disconnectsaveFlag = true;
                ServerHandler.WriteToConsole($"Player [{ply2username}] is being kicked", ServerHandler.LogMode.Warning);
            }
           
            while (true)
            {
                if (Connectedply1 != null && Connectedply2 != null)
                {
                    if (Connectedply1.disconnectsaveFlag == false && Connectedply2.disconnectsaveFlag == false) ////// Wait for both players to be disconnected.
                    {
                        break;
                    }

                }
                else if (Connectedply1 != null && Connectedply2 == null)
                {
                    if (Connectedply1.disconnectsaveFlag == false) ////// Wait for both players to be disconnected.
                    {
                        break;
                    }
                }
                else if (Connectedply2 != null && Connectedply1 == null)
                {
                    if (Connectedply2.disconnectsaveFlag == false) ////// Wait for both players to be disconnected.
                    {
                        break;
                    }

                }
                else {
                    break; // Neither player is connected
                }

            }


            SettlementFile SettlementFile1 = ClientHandler.GetClientSettlmentFileFromName(ply1username);
            SettlementFile SettlementFile2 = ClientHandler.GetClientSettlmentFileFromName(ply2username);
            SettlementFile1.settlementUsername = ply2username; // Set the settlement username to other persons username
            SettlementFile2.settlementUsername = ply1username; // Set the settlement username to other persons username


            SettlementHandler.SaveSettlementFile(SettlementFile1, ply1.SavedID);
            SettlementHandler.SaveSettlementFile(SettlementFile2, ply2.SavedID);
            ClientHandler.SwapNamesOfClientSettlementFile(ply1.SavedID, ply2.SavedID);
          


            string TempSaveid = ply1.SavedID;
            ply1.SavedID = ply2.SavedID;
            ply2.SavedID = TempSaveid;

            ClientHandler.SaveClient(ply1);
            ClientHandler.SaveClient(ply2);

            ServerHandler.WriteToConsole($"Succesffully transfered save from {ply1username} to {ply2username}", ServerHandler.LogMode.Warning);
        }
        
        public static void DeopCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                toGet.IsAdmin = false;
                ClientHandler.SaveClient(toGet);
                ServerHandler.WriteToConsole($"Player [{toGet.Username}] has lost OP", ServerHandler.LogMode.Warning);
                ServerClient ConnnectedClient = ClientHandler.GetClientFromConnected(username);
                if (ConnnectedClient != null)
                {
                    Packet DeopCommandPacket = new Packet("DeopCommandPacket");
                    Network.SendData(ConnnectedClient, DeopCommandPacket);
                }
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
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
            }

            else
            {
                toGet.IsBanned = true;
                ClientHandler.SaveClient(toGet);
                if (ClientHandler.GetClientFromConnected(username) != null)
                {
                    toGet.disconnectFlag = true;
                }
                //ClientHandler.SaveClient(toGet);
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
                toGet.disconnectsaveFlag = true;
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
