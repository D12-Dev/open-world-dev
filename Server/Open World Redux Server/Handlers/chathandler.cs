using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class ServerChatHandler {
        public static List<string> ChatCache = new List<string>() { 
            "<color=yellow>Welcome to the chat!</color>", 
            "<color=yellow>Type /help to see a list of all commands.</color>", 
            "<color=yellow>Please report any bugs or ask for help on the Open World Discord!</color>" 
        };



        public static void SendClientMessageCache(ServerClient client) {

            string[] contents = ChatCache.ToArray();
            Packet ServerSaveFilePacket = new Packet("SendClientMsgCache", contents);
            Network.SendData(client, ServerSaveFilePacket);
        }
        public static void SendAllClientsNewMsg(string Msg) {


            string[] contents = new string[] { Msg };
            Packet NewMsgPacket = new Packet("SendClientNewMsg", contents);
            Network.SendDataToAllConnectedClients(NewMsgPacket);
        }
        public static void SendClientNewMsg(ServerClient client, string Msg)
        {
            try
            {
                // Add a try/catch so that it doesnt crash your fucking game

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
                if (ChatCache.Count >= 100) {
                    ChatCache.RemoveAt(0);
                }
                string userColor = "<color=#708090>";
                if (client.IsAdmin) { userColor = "<color=#FFD700>"; }

                string finishedMessage = "<b>" + DateTime.Now.ToString("h:mm tt") + " | [" + userColor + client.Username + "</color>]: </b>" + Msg;
                ServerHandler.WriteToConsole("[CHAT] " + DateTime.Now.ToString("h:mm tt") + " | [" + client.Username + "]: " + Msg, ServerHandler.LogMode.Normal);

                ChatCache.Add(finishedMessage);

                SendAllClientsNewMsg(finishedMessage);
            }
            catch (Exception ex) {
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
        public static void HandleCommandMsg(ServerClient client, string commandmsg)
        {
            if (client.IsAdmin)
            {
                Server.CmdPostHandler(commandmsg.Remove(0, 1));
                ServerHandler.WriteToConsole(client.Username + " just ran " + commandmsg);
            }
        }
    }
}
