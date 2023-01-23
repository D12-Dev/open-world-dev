using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace OpenWorldRedux
{
    public static class Serializer
    {
        public static string Serialize(object toSerialize)
        {
            string toConvert = JsonUtility.ToJson(toSerialize);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(toConvert));
        }

        public static Packet Deserialize(string toDeserialize)
        {
            string toConvert = Encoding.UTF8.GetString(Convert.FromBase64String(toDeserialize));

            return JsonUtility.FromJson<Packet>(toConvert);
        }

        public static string SoftSerialize(object toSerialize)
        {
            return JsonUtility.ToJson(toSerialize);
        }

        public static void SerializeToFile(string path, object toSerialize)
        {
            string toWrite = JsonUtility.ToJson(toSerialize, true);
            File.WriteAllText(path, toWrite);
        }

        public static T DeserializeFromFile<T>(string path)
        {
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        public static T SerializeToClass<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }
    }
}
