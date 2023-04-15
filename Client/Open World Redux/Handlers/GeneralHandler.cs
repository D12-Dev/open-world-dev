using HugsLib.Settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OpenWorldRedux
{
    public static class GeneralHandler
    {
        public static void AcceptedConnectionHandle()
        {
            Find.WindowStack.Add(new OW_MPLoginType());
        }

        public static void RegisteredClientHandle()
        {
            string[] chainInfo = new string[]
            {
                "You have successfully registered into the server",
                "Please login using your new account"
            };

            Find.WindowStack.Add(new OW_ChainInfoDialog(chainInfo));
        }

        public static void ServerValuesHandle(Packet receivedPacket)
        {
            ServerValuesFile newServerValues = Serializer.SerializeToClass<ServerValuesFile>(receivedPacket.contents[0]);
            newServerValues.ApplyValues();
        }

        public static void ServerDifficultyHandle(Packet receivedPacket)
        {
            DifficultyFile newDifficultyFile = Serializer.SerializeToClass<DifficultyFile>(receivedPacket.contents[0]);
            newDifficultyFile.ApplyValues();
        }

        public static void ClientValuesHandle(Packet receivedPacket)
        {
            ClientValuesFile newClientValues = Serializer.SerializeToClass<ClientValuesFile>(receivedPacket.contents[0]);
            newClientValues.ApplyValues();
        }

        public static void ServerPlayersHandle(Packet receivedPacket)
        {
            List<string> tempContents = receivedPacket.contents.ToList();
            FocusCache.playerCount = int.Parse(tempContents[0]);
            tempContents.RemoveAt(0);

            FocusCache.playerList = tempContents.ToList();
        }

        public static void ToggleOp(bool mode)
        {
            BooleanCache.isAdmin = mode;

            RimworldHandler.ToggleDevOptions();

            if (mode) Find.WindowStack.Add(new OW_InfoDialog("You have been granted admin privileges"));
            else Find.WindowStack.Add(new OW_InfoDialog("You have lost admin privileges"));
        }

        public static string[] GetCompactedModList()
        {
            List<string> compactedMods = new List<string>();

            ModContentPack[] runningMods = LoadedModManager.RunningMods.ToArray();
            foreach (ModContentPack mod in runningMods)
            {
                foreach (FileInfo item in from f in ModContentPack.GetAllFilesForModPreserveOrder(mod, "Assemblies/", (string e) => e.ToLower() == ".dll") select f.Item2)
                {
                    string modAssemblyName = Path.GetFileNameWithoutExtension(item.Name);
                    if (!compactedMods.Contains(modAssemblyName)) compactedMods.Add(modAssemblyName);
                }
            }

            return compactedMods.ToArray();
        }

        public static void GetAnnouncement(Packet receivedPacket)
        {
            if (!BooleanCache.hasLoadedCorrectly) return;
            else
            {
                LetterCache.GetLetterDetails("Server announcement",
                    receivedPacket.contents[0], LetterDefOf.PositiveEvent);

                Injections.thingsToDoInUpdate.Add(LetterCache.GenerateLetter);
            }
        }

        public static void SendAuthFile()
        {
            ClientAuthFile authFile = new ClientAuthFile();
            authFile.clientVersion = FocusCache.versionCode;
            authFile.clientMods = GetCompactedModList();

            string[] contents = new string[] { Serializer.SoftSerialize(authFile) };
            Packet ClientAuthInfoPacket = new Packet("ClientAuthPacket", contents);
            Network.SendData(ClientAuthInfoPacket);
        }
    }
}
