using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldServerVerificator
{
    [Serializable]
    public class ConfigFile
    {
        public string LocalAddress { get; set; } = "0.0.0.0";

        public int ServerPort { get; set; } = 25555;
    }
}
