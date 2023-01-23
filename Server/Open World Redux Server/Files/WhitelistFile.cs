using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class WhitelistFile
    {
        public List<string> WhitelistedUsernames = new List<string>()
        {
            "Test 1",
            "Test 2",
            "Test 3"
        };
    }
}
