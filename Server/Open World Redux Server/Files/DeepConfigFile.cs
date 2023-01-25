using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class DeepConfigFile
    {
        public int BankCashPerTick = 50;
        public int MaximumCashInBank = 10000;
    }
}
