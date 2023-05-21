using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldRedux
{
    public static class Hash
    {
        public static string GetHashCode(string input)
        {
            using (SHA256 shaAlgorythm = SHA256.Create())
            {
                byte[] code = shaAlgorythm.ComputeHash(Encoding.ASCII.GetBytes(input));
                return BitConverter.ToString(code).Replace("-", "");
            }
        }
    }
}
