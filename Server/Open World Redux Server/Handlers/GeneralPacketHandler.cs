using Open_World_Redux;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    public static class GeneralPacketHandler
    {
        public static void ClientAuthHandle(ServerClient client, Packet receivedPacket)
        {
            ClientAuthFile authFile = Serializer.SerializeToClass<ClientAuthFile>(receivedPacket.contents[0]);

            ClientLoginHandler.CheckClientAuthFile(client, authFile);
        }

        public static void DeleteAllPlayerData(ServerClient client)
        {
            client.disconnectFlag = true;

            string settlementFilePath = Server.settlementsFolderPath + Path.DirectorySeparatorChar + client.SavedID + ".json";
            if (File.Exists(settlementFilePath)) File.Delete(settlementFilePath);

            string saveFilePath = Server.savesFolderPath + Path.DirectorySeparatorChar + client.SavedID + ".zipx";
            if (File.Exists(saveFilePath)) File.Delete(saveFilePath);

            ServerHandler.WriteToConsole($"[Deleted save data] > [{client.Username}] - [{client.SavedID}]");
        }
    }
}
