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
            string filename = Current.Game.Info.permadeathModeUniqueName;
            BooleanCache.isSaving = true;
            GameDataSaveLoader.SaveGame(filename);
            Log.Message(filename);
            BooleanCache.isSaving = false;
            string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + filename + ".rws";
            string newFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + filename + ".zipx";

            File.WriteAllBytes(newFilePath, SaveHandler.Zip(File.ReadAllBytes(filePath)));

            string[] contents = new string[] { Convert.ToBase64String(File.ReadAllBytes(newFilePath)) };
            Packet ClientSaveFilePacket = new Packet("ForceClientSyncPacketReturn", contents);
            Network.SendData(ClientSaveFilePacket);



        }

    }
}
