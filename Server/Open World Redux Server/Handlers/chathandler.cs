using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class ServerChatHandler
    {
        public static List<string> ChatCache = new List<string>() {
            "<color=yellow>Welcome to the chat!</color>",
            "<color=yellow>Type /help to see a list of all commands.</color>",
            "<color=yellow>Please report any bugs or ask for help on the Open World Discord!</color>"
        };



        public static void SendClientMessageCache(ServerClient client)
        {

            string[] contents = ChatCache.ToArray();
            Packet ServerSaveFilePacket = new Packet("SendClientMsgCache", contents);
            Network.SendData(client, ServerSaveFilePacket);
        }
        public static void SendAllClientsNewMsg(string Msg)
        {


            string[] contents = new string[] { Msg };
            Packet NewMsgPacket = new Packet("SendClientNewMsg", contents);
            Network.SendDataToAllConnectedClients(NewMsgPacket);
        }
        public static void SendClientNewMsg(ServerClient client, string Msg)
        {
            try
            {
                string raw = Msg.Split(' ')[0];
                string playerName = Msg.Split(' ')[1];
                string msgBuffer = Msg.Remove(1, (raw.Length + playerName.Length + 1));

                ServerClient SentUser = ClientHandler.GetClientFromConnected(playerName);

                string sentColor = "<color=#708090>";
                if (client.IsAdmin) { sentColor = "<color=#FFD700>"; }


                string receiveColor = "<color=#708090>";
                if (SentUser.IsAdmin) { receiveColor = "<color=#FFD700>"; }

                string finishedMessage = "<b>" + DateTime.Now.ToString("h:mm tt") + " | [" + sentColor + client.Username + "</color> --> " + receiveColor + SentUser.Username + "</color>]: </b><color=#99ccff>" + msgBuffer.Remove(0, 1) + "</color>";


                string[] contents = new string[] { finishedMessage };
                Packet NewMsgPacket = new Packet("SendClientNewMsg", contents);
                Network.SendData(SentUser, NewMsgPacket);
                Network.SendData(client, NewMsgPacket);
            }
            catch (Exception ex)
            {
                ServerHandler.WriteToConsole(ex.ToString());
            }
        }
        public static void AddMsgToChatCache(ServerClient client, string Msg)
        {
            try
            {
                if (ChatCache.Count >= 100)
                {
                    ChatCache.RemoveAt(0);
                }
                string userColor = "<color=#708090>";
                if (client.IsAdmin) { userColor = "<color=#FFD700>"; }

                string finishedMessage = "<b>" + DateTime.Now.ToString("h:mm tt") + " | [" + userColor + client.Username + "</color>]: </b>" + Msg;
                ServerHandler.WriteToConsole("[CHAT] " + DateTime.Now.ToString("h:mm tt") + " | [" + client.Username + "]: " + Msg, ServerHandler.LogMode.Normal);

                ChatCache.Add(finishedMessage);

                SendAllClientsNewMsg(finishedMessage);
            }
            catch (Exception ex)
            {
                ServerHandler.WriteToConsole($"[CHAT] > Tried to add msg to cache failed --> {ex}", ServerHandler.LogMode.Warning);
            }

        }
        public static void SendRawMessage(string Msg)
        {
            try
            {
                if (ChatCache.Count >= 100)
                {
                    ChatCache.RemoveAt(0);
                }

                string finishedMessage = "<b>" + DateTime.Now.ToString("h:mm tt") + " | [<color=#FF5900>SERVER</color>]: </b>" + Msg;
                ServerHandler.WriteToConsole("[CHAT] " + DateTime.Now.ToString("h:mm tt") + " | [SERVER]: " + Msg, ServerHandler.LogMode.Normal);

                ChatCache.Add(finishedMessage);

                SendAllClientsNewMsg(finishedMessage);
            }
            catch (Exception ex)
            {
                ServerHandler.WriteToConsole($"[CHAT] > Tried to add msg to cache failed --> {ex}", ServerHandler.LogMode.Warning);
            }
        }


        public static void HandleVisitRequest(ServerClient client, string clienttosend, string caravanitems)
        {
            //client is original sender of request. Msg is person who needs to be sent to.
            try
            {
                ServerHandler.WriteToConsole("Started visit request");
                ServerHandler.WriteToConsole("From:" + client);
                ServerHandler.WriteToConsole("sent user: " + caravanitems);
                ServerHandler.WriteToConsole("sent user: " + clienttosend);
                ServerClient SentUser = ClientHandler.GetClientFromConnected(clienttosend);
                ServerHandler.WriteToConsole("sent user: " + ClientHandler.GetClientFromConnected(clienttosend));
                string finishedMessage = DateTime.Now.ToString("h:mm tt") + " | [" + client.Username + "]:" + "Visit Request:" + caravanitems;
                string[] contents = new string[] { finishedMessage };
                Packet NewMsgPacket = new Packet("VisitRequest", contents);
                Network.SendData(SentUser, NewMsgPacket);

            }
            catch (Exception ex)
            {
                ServerHandler.WriteToConsole("Ex within try for handle: " + ex.ToString());
            }
        }

        public static void HandleVisitAccept(ServerClient client, string Msg, string steamid)
        {
            //client is original sender of request. Msg is person who needs to be sent to.
            try
            {
                ServerHandler.WriteToConsole("Started visit accept");
                ServerHandler.WriteToConsole("sent user: " + Msg);
                ServerClient SentUser = ClientHandler.GetClientFromConnected(Msg);
                ServerHandler.WriteToConsole("sent user: " + ClientHandler.GetClientFromConnected(Msg));
                string finishedMessage = DateTime.Now.ToString("h:mm tt") + " | [" + client.Username + "]:" + steamid;
                ServerHandler.WriteToConsole("finished message: " + finishedMessage);
                string[] contents = new string[] { finishedMessage };
                Packet NewMsgPacket = new Packet("VisitAccept", contents);
                Network.SendData(SentUser, NewMsgPacket);
                ServerHandler.WriteToConsole("Sent visit accept to: " + Msg);
            }
            catch (Exception ex)
            {
                ServerHandler.WriteToConsole(ex.ToString());
            }
        }


        public static void HandleLeaveNotification(ServerClient client, string clienttosend, string caravanitems)
        {
            //client is original sender of request. Msg is person who needs to be sent to.
            try
            {
                ServerClient SentUser = ClientHandler.GetClientFromConnected(clienttosend);
                string finishedMessage = DateTime.Now.ToString("h:mm tt") + " | [" + client.Username + "]:" + "Leave Notification:" + caravanitems;
                string[] contents = new string[] { finishedMessage };
                Packet NewMsgPacket = new Packet("LeaveNotification", contents);
                Network.SendData(SentUser, NewMsgPacket);

            }
            catch (Exception ex)
            {
                ServerHandler.WriteToConsole("Ex within try for handle: " + ex.ToString());
            }
        }




        public static void SendAdminHelp(ServerClient client)
        {
            if (client.IsAdmin)
            {
                List<string> adminCommands = new List<string>();

                foreach (Command cmd in SimpleCommands.commandArray)
                {
                    adminCommands.Add(cmd.prefix + ": " + cmd.prefixHelp);
                }

                foreach (Command cmd in AdvancedCommands.commandArray)
                {
                    adminCommands.Add(cmd.prefix + ": " + cmd.prefixHelp);
                }

                Packet NewMsgPacket = new Packet("SendAdminHelp", adminCommands.ToArray());
                Network.SendDataToAllConnectedClients(NewMsgPacket);
            }
        }

        public static void HandleCommandMsg(ServerClient client, string commandmsg)
        {
            if (client.IsAdmin)
            {
                string CommandRes = Server.CmdPostHandler(commandmsg.Remove(0, 1)); // Do something with CommandRes 

                string finishedMessage = "<color=#FF5900><b>[SERVER]: </b>" + CommandRes + "</color>";


                string[] contents = new string[] { finishedMessage };

                Packet NewMsgPacket = new Packet("SendClientNewMsg", contents);
                Network.SendData(client, NewMsgPacket);

                ServerHandler.WriteToConsole(client.Username + " just ran " + commandmsg);
            }
        }
    }
}