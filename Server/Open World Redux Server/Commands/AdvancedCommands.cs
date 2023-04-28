using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.Serialization;

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
        public static WipeCommand wipeCommand = new WipeCommand(); 
        public static BackupCommand Backupcommand = new BackupCommand();
        public static ChangePasswordCommand ResetPasswordcommand = new ChangePasswordCommand();
        public static bool WipeConfirm; 
        public static Command[] commandArray = new Command[]
        {
            opCommand,
            deopCommand,
            kickCommand,
            banCommand,
            pardonCommand,
            inspectCommand,
            invokeCommand,
            transferCommand,
            wipeCommand,
            Backupcommand,
            ResetPasswordcommand
        };


        public static string WipeCommandHandle()
        {
            if (WipeConfirm == true)
            {
                File.Delete(@Server.dataFolderPath + Path.DirectorySeparatorChar + "World.json"); // Delete world.json
                ServerHandler.WriteToConsole($"Wiped {Server.dataFolderPath + Path.DirectorySeparatorChar + "World.json"}.", ServerHandler.LogMode.Title);
                List<string> FoldersToDelete = new List<string>() { // A list of the folder paths to delete
                    Server.playersFolderPath,
                    Server.savesFolderPath,
                    Server.settlementsFolderPath,
                    Server.factionsFolderPath,
                    Server.WorldGenDataPath
                };


                foreach (string X in FoldersToDelete) { // Loop through the list and delete folders
                    Directory.Delete(X, true);
                    ServerHandler.WriteToConsole($"Wiped {X}.", ServerHandler.LogMode.Title);
                }
                ServerHandler.WriteToConsole(@"Wipe completed, Server will now shutdown.", ServerHandler.LogMode.Title);
                Thread.Sleep(5000); // Wait and let the console be read.



                Server.isActive = false; // Turn off server
            }
            else
            {
                ServerHandler.WriteToConsole(@"[WARNING] Using the wipe command will remove all player saves, accounts, factions,settlements and world data. To continue type ""wipe"" again.", ServerHandler.LogMode.Warning);
                WipeConfirm = true; // Allow wiping.

            }
            return "hi";
        }

        public static string ChangePasswordCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            string password = CommandHandler.parameterHolder[1]; 
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{username}] was not found";
            }

            else
            {
                string passwordtoset;
                passwordtoset = Hash.GetHashCode(password);
                //passwordtoset = Serializer.Serialize(passwordtoset);

                toGet.Password = passwordtoset;

                ClientHandler.SaveClient(toGet);
                ServerHandler.WriteToConsole($"Player [{toGet.Username}] has had their password changed to {password}", ServerHandler.LogMode.Warning);
                return $"Player [{toGet.Username}] has had their password changed to {password}";
            }
        }
        
        public static string OpCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{username}] was not found";
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
                return $"Player [{toGet.Username}] has become OP";
            }
        }
        public static string TransferCommandHandle() {
            string ply1username = CommandHandler.parameterHolder[0];
            string ply2username = CommandHandler.parameterHolder[1];
            ServerClient ply1 = ClientHandler.GetClientFromSave(ply1username);
            ServerClient ply2 = ClientHandler.GetClientFromSave(ply2username);
            ServerClient Connectedply1 = ClientHandler.GetClientFromConnected(ply1username);
            ServerClient Connectedply2 = ClientHandler.GetClientFromConnected (ply2username);
            if (ply1 == null) {
                ServerHandler.WriteToConsole($"Player [{ply1username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{ply1username}] was not found";
            }
            if (ply2 == null)
            {
                ServerHandler.WriteToConsole($"Player [{ply2username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{ply2username}] was not found";
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
            return $"Succesffully transfered save from {ply1username} to {ply2username}";
        }

        public static string DeopCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{username}] was not found";
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
                return $"Player [{toGet.Username}] has lost OP";
            }
        }

        public static string InspectCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromConnected(username);
            
            if (toGet == null) toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{username}] was not found";
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
                return "Placeholder command response for inspect";
            }
        }

        public static string BanCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{username}] was not found";
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
                return $"Player [{username}] has been banned";
            }
        }

        public static string PardonCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromSave(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{username}] was not found";
            }

            else
            {
                toGet.IsBanned = false;
                ClientHandler.SaveClient(toGet);
                ServerHandler.WriteToConsole($"Player [{username}] has been pardoned", ServerHandler.LogMode.Warning);
                return $"Player [{username}] has been pardoned";
            }
        }

        public static string KickCommandHandle()
        {
            string username = CommandHandler.parameterHolder[0];
            ServerClient toGet = ClientHandler.GetClientFromConnected(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{username}] was not found";
            }

            else 
            {
                toGet.disconnectsaveFlag = true;
                ServerHandler.WriteToConsole($"Player [{username}] has been kicked", ServerHandler.LogMode.Warning);
                return $"Player [{username}] has been kicked";
            }
        }

        public static string InvokeCommand()
        {
            string username = CommandHandler.parameterHolder[0];
            string eventID = CommandHandler.parameterHolder[1];

            ServerClient toGet = ClientHandler.GetClientFromConnected(username);

            if (toGet == null)
            {
                ServerHandler.WriteToConsole($"Player [{username}] was not found", ServerHandler.LogMode.Warning);
                return $"Player [{username}] was not found";
            }

            else
            {
                string[] contents = new string[] { eventID };
                Packet SendBlackMarketEventPacket = new Packet("SendBlackMarketEventPacket", contents);
                Network.SendData(toGet, SendBlackMarketEventPacket);

                ServerHandler.WriteToConsole($"Sent event to player [{username}]", ServerHandler.LogMode.Warning);
                return $"Sent event to player [{username}]";
            }
        }
    }
}
