using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldRedux
{
    [Serializable]
    public class TradeItem
    {
        public string defName;

        public string madeOfDef;

        public int stackCount;

        public int itemQuality;

        public bool isMinified;

        public bool isAnimal;
    }
}
