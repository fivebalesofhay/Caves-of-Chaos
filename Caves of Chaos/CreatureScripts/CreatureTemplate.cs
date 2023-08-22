using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.CreatureScripts
{
    public class CreatureTemplate
    {
        public String name { get; set; }
        public String symbol { get; set; }
        public String color { get; set; }
        public int health { get; set; }
        public int strength { get; set; }
        public String[] tags { get; set; }
    }
}
