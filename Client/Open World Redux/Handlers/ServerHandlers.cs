using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorldRedux.Handlers
{
    public static class ServerHandlers
    {
        public static void SaveAndSendToSever()
        {
            Log.Message("Got Save request");
            //string filename = Current.Game.Info.permadeathModeUniqueName;
            MPGame.ForceSave();
            Packet ClientSaveFilePacket = new Packet("ForceClientSyncPacketReturn");
            Network.SendData(ClientSaveFilePacket);
            Log.Message("Returned save request");


        }

    }
}
