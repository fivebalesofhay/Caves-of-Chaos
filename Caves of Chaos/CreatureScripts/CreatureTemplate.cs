using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Caves_of_Chaos.CreatureScripts
{
    public class CreatureTemplate
    {
        public String name { get; set; }
        public String symbol { get; set; }
        public String color { get; set; }
        public int minDepth { get; set; }
        public int maxDepth { get; set; }
        public double spawnRatio { get; set; }
        public int level { get; set; }
        public int health { get; set; }
        public int strength { get; set; }
        public int dexterity { get; set; }
        public double movementSpeed { get; set; }
        public double actionSpeed { get; set; }
        public String[]? weapons { get; set; }
        public int[]? weaponRatios { get; set; }
        public String[] tags { get; set; }
        public String[]? resistances { get; set; }
        public int[]? resistanceStrengths { get; set; }
    }
}
