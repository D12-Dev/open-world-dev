using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenWorldReduxServer.Handlers
{
    public static class SaveBaseGameHandler
    {

        public static void SaveBaseGame(ServerClient client, Packet receivedPacket) {
            string savePath = Server.WorldGenDataPath + Path.DirectorySeparatorChar + "BaseGame.zipx";
            File.WriteAllBytes(savePath, Convert.FromBase64String(receivedPacket.contents[0]));

            ServerHandler.WriteToConsole($"Saved Base Generated World File!");


        }
    }
}
