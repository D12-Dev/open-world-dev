using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class AuthFile
    {
        public string Username { get; set; } = "";

        public string Token { get; set; } = "";
    }
}
