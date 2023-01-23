using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OpenWorldRedux
{
    public static class SaveHandler
    {
        public static void SendSaveToServer(string fileName)
        {
            string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".rws";
            string newFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".zipx";

            File.WriteAllBytes(newFilePath, Zip(File.ReadAllBytes(filePath)));

            string[] contents = new string[] { Convert.ToBase64String(File.ReadAllBytes(newFilePath)) };
            Packet ClientSaveFilePacket = new Packet("ClientSaveFilePacket", contents);
            Network.SendData(ClientSaveFilePacket);
        }

        public static void LoadFromServerSave(Packet receivedPacket)
        {
            FocusCache.waitWindowInstance.Close();

            string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + "Open World Server Save.rws";

            File.WriteAllBytes(filePath, Unzip(Convert.FromBase64String(receivedPacket.contents[0])));
            GameDataSaveLoader.LoadGame("Open World Server Save");
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static byte[] Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                return mso.ToArray();
            }
        }
    }
}
