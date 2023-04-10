using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class ServerChatHandler {
        public static List<string> ChatCache = new List<string>();



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
            string[] contents = new string[] { Msg };
            Packet NewMsgPacket = new Packet("SendClientNewMsg", contents);
            Network.SendData(client, NewMsgPacket);

        }
        public static void AddMsgToChatCache(string Msg)
        {
            try
            {
                if (ChatCache.Count >= 1000) {
                    ChatCache.RemoveAt(0);
                }
                ChatCache.Add(Msg);
            }
            catch (Exception ex) {
                ServerHandler.WriteToConsole($"[CHAT] > Tried to add msg to cache failed --> {ex}", ServerHandler.LogMode.Warning);
            }

        }
        public static void HandleCommandMsg(string commandmsg)
        {



            Server.CmdPostHandler(commandmsg);


        }

    }

}
