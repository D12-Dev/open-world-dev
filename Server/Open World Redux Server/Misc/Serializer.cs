using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenWorldReduxServer
{
    public static class Serializer
    {
        public static string Serialize(object serializable)
        {
            string toConvert = JsonConvert.SerializeObject(serializable);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(toConvert));
        }

        public static Packet Deserialize(string data)
        {
            try
            {
                string toConvert = Encoding.UTF8.GetString(Convert.FromBase64String(data));

                return JsonConvert.DeserializeObject<Packet>(toConvert);
            }

            catch { return null; }
        }

        public static string SoftSerialize(object toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }

        public static void SerializeToFile(object serializable, string path)
        {
            string toWrite = JsonConvert.SerializeObject(serializable, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(path, toWrite);
        }

        public static T SerializeToClass<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static T DeserializeFromFile<T>(string filePath) where T : class
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }
    }
}