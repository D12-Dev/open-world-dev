using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class ClientValuesFile
    {
        public bool isAdmin;

        public bool isCustomScenariosAllowed;

        public int selectedProductionItem;
    }
}
