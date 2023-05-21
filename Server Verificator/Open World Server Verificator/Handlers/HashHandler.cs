using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldServerVerificator
{
    public static class HashHandler
    {
        public static string GetTokenForNewClient()
        {
            string dateTimeNow = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            byte[] byteArray = Encoding.ASCII.GetBytes(dateTimeNow);

            using (SHA256 shaAlgorythm = SHA256.Create())
            {
                byte[] code = shaAlgorythm.ComputeHash(byteArray);
                return BitConverter.ToString(code).Replace("-", "");
            }
        }
    }
}
