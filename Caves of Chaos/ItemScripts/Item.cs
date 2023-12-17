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
        public int? damageDie;
        public int? damageRolls;
        public String? damageType;
        public int? attackTime;

        public Point? position;

        public Item(Point? initialPosition, Grid? grid, ItemTemplate template) { 
            name = template.name; 
            tags = template.tags;
            damageDie = template.damageDie;
            damageRolls = template.damageRolls;
            damageType = template.damageType;
            attackTime = template.attackTime;

            position = initialPosition;
            if (position != null && grid != null) {
                Point p = (Point)position;
                grid.tiles[p.X, p.Y].item = this;
            }
        }

        public Boolean hasTag(String s)
        {
            return tags.Contains(s);
        }
    }
}
