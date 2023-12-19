using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.GridScripts
{
    public class GridTemplate
    {
        public int depth { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public double creatureDensity { get; set; }
        public double structureDensity { get; set; }
        public double itemDensity { get; set; }
    }
}
