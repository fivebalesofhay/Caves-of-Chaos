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
        public String name { get; set; } = "";
        public String symbol { get; set; } = "?";
        public String color { get; set; } = "white";
        public int minDepth { get; set; }
        public int maxDepth { get; set; }
        public double spawnRatio { get; set; }
        public int level { get; set; }
        public int health { get; set; }
        public int strength { get; set; }
        public int dexterity { get; set; }
        public int baseAttackRolls { get; set; } = 1;
        public int baseAttackDie { get; set; } = 4;
        public double movementSpeed { get; set; }
        public double actionSpeed { get; set; }
        public String[]? weapons { get; set; }
        public int[]? weaponRatios { get; set; }
        public String[]? armors { get; set; }
        public int[]? armorRatios { get; set; }
        public String[] tags { get; set; } = { };
        public String[]? resistances { get; set; }
        public int[]? resistanceStrengths { get; set; }
    }
}
