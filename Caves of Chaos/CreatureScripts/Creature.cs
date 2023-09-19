﻿using Caves_of_Chaos.GridScripts;
using Caves_of_Chaos.UIScripts;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static Caves_of_Chaos.GridScripts.GridManager;

namespace Caves_of_Chaos.CreatureScripts
{
    public class Creature
    {
        private Point position = new Point();

        public ColoredGlyph glyph = new ColoredGlyph();
        public String name;
        public int minDepth;
        public int maxDepth;
        public double spawnRatio;
        public int maxHealth;
        public double baseAttackStrength;
        public String[] tags;
        public Dictionary<String, int> resistances;

        public int health;
        public double actionPoints = 0;

        public Creature(Point initialPosition, Grid grid, CreatureTemplate template) 
        { 
            position = initialPosition;
            this.glyph = new ColoredGlyph(Palette.colors[template.color], Palette.black, template.symbol.ToCharArray()[0]);

            this.name = template.name;
            this.maxHealth = template.health;
            this.baseAttackStrength = template.strength;
            this.tags = template.tags;
            this.resistances = new Dictionary<string, int>();
            
            if (template.resistances != null)
            {
                for (int i = 0; i < template.resistances.Length; i++)
                {
                    resistances.Add(template.resistances[i], template.resistanceStrengths[i]);
                    
                }
            }

            grid.tiles[position.X, position.Y].occupant = this;
            grid.creatures.Add(this);
            health = maxHealth;
        }

        // return action point cost of action
        public double Act()
        {
            // Simplest possible brain. Replace later with something better
            if (hasTag("AGRESSIVE"))
            {
                if (activeGrid.GetTile(position).isSeen)
                {
                    Point playerDiff = PlayerManager.player.GetPosition() - position;
                    if (Program.random.NextDouble() < Math.Abs(playerDiff.X/Utility.Distance(new Point(0,0), playerDiff)))
                    {
                        Move(new Point(Math.Sign(playerDiff.X), 0));
                        return 10; // Change this
                    } else
                    {
                        Move(new Point(0, Math.Sign(playerDiff.Y)));
                        return 10; // Change this
                    }
                } else
                {
                    Move(Utility.RandomDirection());
                    return 10; // Change this
                }
            } else
            {
                Move(Utility.RandomDirection());
                return 10; // Change this
            }
        }

        public void Move(Point direction)
        {
            Point newPosition = position + direction;
            if (activeGrid.GetTile(newPosition).isWall) { return; }
            if (activeGrid.GetTile(newPosition).occupant != null)
            {
                Attack(activeGrid.GetTile(newPosition).occupant);
                return;
            }
            activeGrid.GetTile(position).occupant = null;
            position = newPosition;
            activeGrid.GetTile(position).occupant = this;
        }
        
        public void MoveTo(Point location)
        {
            if (activeGrid.GetTile(location).isWall) { return; }
            if (activeGrid.GetTile(location).occupant != null) { return; }
            activeGrid.GetTile(position).occupant = null;
            position = location;
            activeGrid.GetTile(position).occupant = this;
        }

        public void Attack(Creature creature)
        {
            int damage = Utility.randRoundInt(baseAttackStrength * Math.Pow(2, -creature.GetResistance("bludgeoning")));
            LogConsole.UpdateLog(name + " attacks " + creature.name + " for " + damage + " damage!");
            creature.Damage(damage);
        }

        public void Damage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            activeGrid.creatures.Remove(this);
            activeGrid.GetTile(position).occupant = null;
        }

        public int GetResistance(String type)
        {
            if (resistances.ContainsKey(type))
            {
                return resistances[type];
            }
            return 0;
        }

        public Point GetPosition()
        {
            return position;
        }

        public Boolean hasTag(String tag)
        {
            return tags.Contains(tag);
        }
    }
}
