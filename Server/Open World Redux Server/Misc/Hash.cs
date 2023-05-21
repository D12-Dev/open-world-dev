using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OpenWorldReduxServer
{
    public class Hash
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