using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.ItemScripts
{
    public class ItemTemplate
    {
        public String name { get; set; } = "";
        public String? description { get; set; }
        public int minDepth { get; set; }
        public int maxDepth { get; set; }
        public int? damageDie { get; set; }
        public int? damageRolls { get; set; }
        public String? damageType { get; set; }
        public int? attackTime { get; set; }
        public int? armorValue { get; set; }
        public String[] tags { get; set; } = { };
    }
}
