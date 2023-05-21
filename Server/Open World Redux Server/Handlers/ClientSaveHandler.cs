using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;


namespace OpenWorldReduxServer
{
    public static class ClientSaveHandler
    {
        public static void SaveClientSave(ServerClient client, Packet receivedPacket)
        {
            string savePath = Server.savesFolderPath + Path.DirectorySeparatorChar + client.SavedID + ".zipx";
            File.WriteAllBytes(savePath, Convert.FromBase64String(receivedPacket.contents[0]));

            ServerHandler.WriteToConsole($"[Saved File] > [{client.Username}] - [{client.SavedID}]");
        }

        public static void SendSaveFile(ServerClient client)
        {
            string filePath = Server.savesFolderPath + Path.DirectorySeparatorChar + client.SavedID + ".zipx";

            string[] contents = new string[] { Convert.ToBase64String(File.ReadAllBytes(filePath)) };
            Packet ServerSaveFilePacket = new Packet("ServerSaveFilePacket", contents);
            Network.SendData(client, ServerSaveFilePacket);
        }

        public static void SendWorldGenSave(ServerClient client)
        {
            string filePath = Server.WorldGenDataPath + Path.DirectorySeparatorChar + "BaseGame.zipx";
            string[] contents = new string[] { Convert.ToBase64String(File.ReadAllBytes(filePath)) };
            Packet SendBaseSave = new Packet("ServerWorldGenPacket", contents);
            Network.SendData(client, SendBaseSave);
        }
    }
}