using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class Packet
    {
        public string header = null;

        public string[] contents = null;

        public Packet(string header, string[] contents = null)
        {
            this.header = header;
            this.contents = contents;
        }
    }
}
