using Open_World_Redux;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenWorldReduxServer
{
    public static class WorldGenDataHandler
    {
        public static void CheckForExistingWorldGen(ServerClient client)
        {
            string[] worldGenFile = Directory.GetFiles(Server.WorldGenDataPath);

            if (worldGenFile.Length == 1)
            {
                string[] contents = new string[] { "true" };
                Packet ServerSaveFilePacket = new Packet("WorldGenExistsReturn", contents);
                Network.SendData(client, ServerSaveFilePacket);
            }
            else
            {
                string[] contents = new string[] { "false" };
                Packet ServerSaveFilePacket = new Packet("WorldGenExistsReturn", contents);
                Network.SendData(client, ServerSaveFilePacket);
            }
        }
    }


}