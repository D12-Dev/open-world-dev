using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWorldReduxServer
{
    [Serializable]
    public class WorldFile
    {
        public string Seed { get; set; }

        public string PlanetCoverage { get; set; }

        public string OverallRainfall { get; set; }

        public string OverallTemperature { get; set; }

        public string OverallPopulation { get; set; }

        public string Pollution { get; set; }
    }
}
