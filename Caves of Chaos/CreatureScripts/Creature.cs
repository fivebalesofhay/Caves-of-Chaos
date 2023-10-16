using Caves_of_Chaos.GridScripts;
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
        public double movementSpeed;
        public double actionSpeed;
        public String[] tags;
        public Dictionary<String, int> resistances;

        public int health;
        public double healingOffset = 0;
        public double actionPoints = 0;

        public Creature(Point initialPosition, Grid grid, CreatureTemplate template) 
        { 
            position = initialPosition;
            glyph = new ColoredGlyph(Palette.colors[template.color], Palette.black, template.symbol.ToCharArray()[0]);

            name = template.name;
            maxHealth = template.health;
            baseAttackStrength = template.strength;
            movementSpeed = template.movementSpeed; 
            actionSpeed = template.actionSpeed;
            tags = template.tags;
            resistances = new Dictionary<string, int>();
            
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

        public void Update()
        {
            if (health < maxHealth)
            {
                healingOffset += maxHealth * GameSettings.BASE_HEALING_RATE;
            }
            while (healingOffset >= 1)
            {
                health++;
                healingOffset--;
            }
        }

        // return action point cost of action
        public double Act()
        {
            // Simplest possible brain. Replace later with something better
            if (HasTag("AGRESSIVE"))
            {
                if (activeGrid.GetTile(position).isSeen)
                {
                    Point playerDiff = PlayerManager.player.GetPosition() - position;
                    if (Math.Abs(playerDiff.X) == Math.Abs(playerDiff.Y))
                    {
                        System.Diagnostics.Debug.WriteLine("Diagonal");
                        return Move(new Point(Math.Sign(playerDiff.X), Math.Sign(playerDiff.Y)));
                    }
                    else if (Program.random.NextDouble() < Math.Abs(playerDiff.X/Utility.Distance(new Point(0,0), playerDiff)))
                    {
                        System.Diagnostics.Debug.WriteLine("X");
                        return Move(new Point(Math.Sign(playerDiff.X), 0));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Y");
                        return Move(new Point(0, Math.Sign(playerDiff.Y)));
                    }
                } else
                {
                    return Move(Utility.RandomDirection());
                }
            } else
            {
                return Move(Utility.RandomDirection());
            }
        }

        public double Move(Point direction)
        {
            if (direction == new Point(0, 0)) return 0;
            Point newPosition = position + direction;
            if (activeGrid.GetTile(newPosition).isWall) { return 0; }
            if (activeGrid.GetTile(newPosition).occupant != null)
            {
                Attack(activeGrid.GetTile(newPosition).occupant);
                return GetMovementTime();
            }
            activeGrid.GetTile(position).occupant = null;
            position = newPosition;
            activeGrid.GetTile(position).occupant = this;
            return GetMovementTime();
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

        public double GetMovementTime()
        {
            return GameSettings.BASE_MOVEMENT_TIME / movementSpeed / actionSpeed;
        }

        public Boolean HasTag(String tag)
        {
            return tags.Contains(tag);
        }
    }
}
