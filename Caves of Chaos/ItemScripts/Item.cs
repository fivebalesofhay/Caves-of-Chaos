using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caves_of_Chaos.CreatureScripts;
using Caves_of_Chaos.GridScripts;

namespace Caves_of_Chaos.ItemScripts
{
    public class Item
    {
        public Creature? owner = null;
        public String name;
        public String[] tags;
        // Weapon variables:
        public int? damageDie;
        public int? damageRolls;
        public String? damageType;
        public int? attackTime;
        // Armor variables: 
        public int? armorValue;

        public Point? position;
        public Boolean equipped = false;

        public Item(Point? initialPosition, Grid? grid, ItemTemplate template) { 
            name = template.name; 
            tags = template.tags;
            damageDie = template.damageDie;
            damageRolls = template.damageRolls;
            damageType = template.damageType;
            attackTime = template.attackTime;
            armorValue = template.armorValue;

            position = initialPosition;
            if (position != null && grid != null) {
                Point p = (Point)position;
                grid.tiles[p.X, p.Y].items.Add(this);
            }
        }

        public Boolean HasTag(String s)
        {
            return tags.Contains(s);
        }
    }
}
