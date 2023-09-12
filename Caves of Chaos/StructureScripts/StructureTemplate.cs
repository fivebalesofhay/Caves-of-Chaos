using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.StructureScripts
{
    public class StructureTemplate
    {
        public String name {  get; set; }
        public String symbol { get; set; }
        public String color { get; set; }
        public int minDepth { get; set; }
        public int maxDepth { get; set; }
    }
}
