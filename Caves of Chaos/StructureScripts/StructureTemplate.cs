using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.StructureScripts
{
    public class StructureTemplate
    {
        public String name { get; set; } = "";
        public String symbol { get; set; } = "?";
        public int? symbolIndex { get; set; }

        public String color { get; set; } = "white";
        public String? backgroundColor { get; set; }
        public int minDepth { get; set; }
        public int maxDepth { get; set; }
        public double spawnRatio { get; set; }
        public String[] tags { get; set; } = { };

        public int? growth { get; set; }
    }
}
