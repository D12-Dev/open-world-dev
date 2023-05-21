using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldRedux
{
    [Serializable]
    public class ClientValuesFile
    {
        public bool isAdmin;

        public bool isCustomScenariosAllowed;

        public int selectedProductionItem;

        public void ApplyValues()
        {
            BooleanCache.isAdmin = isAdmin;
            BooleanCache.isCustomScenariosAllowed = isCustomScenariosAllowed;
            FactionCache.selectedProduct = (FactionCache.ProductionSiteProduct)selectedProductionItem;
        }
    }
}
