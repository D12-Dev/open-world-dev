using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;


namespace OpenWorldReduxServer
{
    public static class WorldHandler
    {
        public static void HandleWorld(ServerClient client)
        {
            if (!CheckIfWorldExists())
            {
                Packet CreateNewWorldPacket = new Packet("CreateNewWorldPacket");
                Network.SendData(client, CreateNewWorldPacket);
            }

            else
            {
                string loadPath = Server.dataFolderPath + Path.DirectorySeparatorChar + "World.json";
                WorldFile toLoad = Serializer.DeserializeFromFile<WorldFile>(loadPath);

                string[] contents = new string[]
                {
                    toLoad.Seed,
                    toLoad.PlanetCoverage,
                    toLoad.OverallRainfall,
                    toLoad.OverallTemperature,
                    toLoad.OverallPopulation,
                    toLoad.Pollution
                };

                Packet CreateWorldFromPacketPacket = new Packet("CreateWorldFromPacketPacket", contents);
                Network.SendData(client, CreateWorldFromPacketPacket);
            }
        }

        public static void SaveNewServerData(ServerClient client, Packet receivedPacket)
        {
            WorldFile newWorldFile = new WorldFile();
            newWorldFile.Seed = receivedPacket.contents[0];
            newWorldFile.PlanetCoverage = receivedPacket.contents[1];
            newWorldFile.OverallRainfall = receivedPacket.contents[2];
            newWorldFile.OverallTemperature = receivedPacket.contents[3];
            newWorldFile.OverallPopulation = receivedPacket.contents[4];
            newWorldFile.Pollution = receivedPacket.contents[5];

            string savePath = Server.dataFolderPath + Path.DirectorySeparatorChar + "World.json";
            Serializer.SerializeToFile(newWorldFile, savePath);

            ServerHandler.WriteToConsole($"[New server data saved] > [{client.Username}] - [{client.SavedID}]");
        }

        private static bool CheckIfWorldExists()
        {
            if (File.Exists(Server.dataFolderPath + Path.DirectorySeparatorChar + "World.json")) return true;
            else return false;
        }
    }
}
