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
using  Caves_of_Chaos.ConditionScripts;
using Caves_of_Chaos.ItemScripts;
using System.Diagnostics;
using System.Net.Http.Headers;

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
        public int level;
        public int maxHealth;
        public int baseAttackRolls;
        public int baseAttackDie;
        public int strength;
        public int dexterity;
        public double movementSpeed;
        public double actionSpeed;
        public String[] tags;
        public Dictionary<String, int> resistances;

        public Grid grid;
        public int health;
        public double healingOffset = 0;
        public double actionPoints = 0;
        public Point? lastKnownTargetPosition = null;
        public List<Item> inventory = new List<Item>();
        public List<Condition> conditions = new List<Condition>();
        public Item? weapon = null;
        public Item? armor = null;

        public Creature(Point initialPosition, Grid initialGrid, CreatureTemplate template) 
        { 
            position = initialPosition;
            grid = initialGrid;
            glyph = new ColoredGlyph(Palette.colors[template.color], Palette.black, template.symbol.ToCharArray()[0]);

            name = template.name;
            level = template.level;
            maxHealth = template.health;
            strength = template.strength; 
            dexterity = template.dexterity;
            baseAttackRolls = template.baseAttackRolls;
            baseAttackDie = template.baseAttackDie;
            movementSpeed = template.movementSpeed; 
            actionSpeed = template.actionSpeed;
            tags = template.tags;
            resistances = new Dictionary<string, int>();
            
            if (template.resistances != null && template.resistanceStrengths != null)
            {
                for (int i = 0; i < template.resistances.Length; i++)
                {
                    resistances.Add(template.resistances[i], template.resistanceStrengths[i]);
                }
            }

            if (template.weapons != null && template.weaponRatios != null)
            {
                double totalSpawnRatio = 0.0;
                for (int i = 0; i < template.weaponRatios.Length; i++)
                {
                    totalSpawnRatio += template.weaponRatios[i];
                }

                double randomIndex = Program.random.NextDouble() * totalSpawnRatio;
                int chosenIndex = 0;
                for (int i = 0; i < template.weaponRatios.Length; i++)
                {
                    randomIndex -= template.weaponRatios[i];
                    if (randomIndex <= 0)
                    {
                        chosenIndex = i;
                        break;
                    }
                }
                if (template.weapons[chosenIndex] != "none")
                {
                    Item item = new Item(null, null, ItemManager.GetTemplate(template.weapons[chosenIndex]));
                    EquipItem(item);
                }
            }

            if (template.armors != null && template.armorRatios != null)
            {
                double totalSpawnRatio = 0.0;
                for (int i = 0; i < template.armorRatios.Length; i++)
                {
                    totalSpawnRatio += template.armorRatios[i];
                }

                double randomIndex = Program.random.NextDouble() * totalSpawnRatio;
                int chosenIndex = 0;
                for (int i = 0; i < template.armorRatios.Length; i++)
                {
                    randomIndex -= template.armorRatios[i];
                    if (randomIndex <= 0)
                    {
                        chosenIndex = i;
                        break;
                    }
                }
                if (template.armors[chosenIndex] != "none")
                {
                    Item item = new Item(null, null, ItemManager.GetTemplate(template.armors[chosenIndex]));
                    EquipItem(item);
                }
            }

            grid.tiles[position.X, position.Y].occupant = this;
            grid.creatures.Add(this);
            health = maxHealth;
        }

        public void Update()
        {
            UpdateConditions();
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

        public void UpdateConditions()
        {
            for (int i = conditions.Count-1; i >= 0; i--)
            {
                conditions[i].duration--;
                if (conditions[i].duration <= 0)
                {
                    conditions.RemoveAt(i);
                }
            }
        }

        // return action point cost of action
        public double Act()
        {
            // Simplest possible brain. Replace later with something better
            Point movement = new Point(0, 0);
            if (HasTag("AGRESSIVE"))
            {
                if (grid.GetTile(position).isSeen && grid == PlayerManager.player.grid)
                {
                    movement = MoveTowardsPoint(PlayerManager.player.GetPosition());
                    lastKnownTargetPosition = PlayerManager.player.GetPosition();
                } 
                else if (lastKnownTargetPosition != null)
                {
                    if (position == lastKnownTargetPosition)
                    {
                        StructureScripts.Structure? s = grid.GetTile(position).structure;
                        if (s != null)
                        {
                            if (s.HasTag("UP_STAIR") || s.HasTag("DOWN_STAIR"))
                            {
                                UseStair(s);
                                return GetMovementTime();
                            }
                        }
                        lastKnownTargetPosition = null;
                        movement = Utility.RandomDirection();
                    } 
                    else
                    {
                        movement = MoveTowardsPoint((Point)lastKnownTargetPosition);
                    }
                }
                else
                {
                    movement = Utility.RandomDirection();
                }
            } else
            {
                movement = Utility.RandomDirection();
            }

            // Don't attack anyone but the player
            if (grid.GetTile(position + movement).occupant != PlayerManager.player 
                    && grid.GetTile(position + movement).occupant != null)
            {
                return GetMovementTime();
            }

            return Move(movement);
        }

        public Point MoveTowardsPoint(Point p)
        {
            Point pointDiff = p - position;
            if (Math.Abs(pointDiff.X) == Math.Abs(pointDiff.Y)
                && !grid.tiles[position.X + Math.Sign(pointDiff.X), position.Y + Math.Sign(pointDiff.Y)].isWall)
            {
                return new Point(Math.Sign(pointDiff.X), Math.Sign(pointDiff.Y));
            }
            else if (Math.Abs(pointDiff.X) > Math.Abs(pointDiff.Y))
            {
                if (Program.random.NextDouble() < Math.Abs(pointDiff.X / Utility.Distance(new Point(0, 0), pointDiff))
                    && !grid.tiles[position.X + Math.Sign(pointDiff.X), position.Y + Math.Sign(pointDiff.Y)].isWall)
                {
                    return new Point(Math.Sign(pointDiff.X), Math.Sign(pointDiff.Y));
                }
                else
                {
                    return new Point(Math.Sign(pointDiff.X), 0);
                }
            }
            else
            {
                if (Program.random.NextDouble() < Math.Abs(pointDiff.Y / Utility.Distance(new Point(0, 0), pointDiff))
                    && !grid.tiles[position.X + Math.Sign(pointDiff.X), position.Y + Math.Sign(pointDiff.Y)].isWall)
                {
                    return new Point(Math.Sign(pointDiff.X), Math.Sign(pointDiff.Y));
                }
                else
                {
                    return new Point(0, Math.Sign(pointDiff.Y));
                }
            }
        }

        public double Move(Point direction)
        {
            if (direction == new Point(0, 0)) return 0;
            Point newPosition = position + direction;
            if (grid.GetTile(newPosition).isWall) { return 0; }
            if (grid.GetTile(newPosition).occupant != null)
            {
                Attack(grid.GetTile(newPosition).occupant);
                return GetAttackTime();
            }
            grid.GetTile(position).occupant = null;
            position = newPosition;
            grid.GetTile(position).occupant = this;
            return GetMovementTime();
        }
        
        public void MoveTo(Point location)
        {
            if (grid.GetTile(location).isWall) { return; }
            if (grid.GetTile(location).occupant != null) { return; }
            grid.GetTile(position).occupant = null;
            position = location;
            grid.GetTile(position).occupant = this;
        }

        public void UseStair(StructureScripts.Structure stair)
        {
            int change = 1;
            if (stair.HasTag("UP_STAIR"))
            {
                change = -1;
            }

            // Cancel if a monster tries to use the stair but is blocked by another
            if (stair.linkedStair != null)
            {
                Creature? c = grids[grid.depth + change].GetTile(stair.linkedStair.GetPosition()).occupant;
                if (c != null)
                {
                    if (!c.Push())
                    {
                        return;
                    }
                    if (c == PlayerManager.player)
                    {
                        LogConsole.UpdateLog("You are pushed aside.");
                    }
                }
            }

            grid.GetTile(position).occupant = null;
            grid.creatures.Remove(this);
            grid = grids[grid.depth + change];

            Point point;

            if (stair.linkedStair != null)
            {
                point = stair.linkedStair.GetPosition();
            }
            else
            {
                point = Utility.RandomPoint(grid);

                while (grid.GetTile(point).isWall == true
                    || grid.GetTile(point).occupant != null)
                {
                    point = Utility.RandomPoint(grid);
                }
            }

            MoveTo(point);
            grid.GetTile(position).occupant = this;
            grid.creatures.Add(this);
        }

        public bool Push()
        {
            Point[] directions = Utility.Shuffle<Point>(Utility.directions);
            for (int i = 0; i < directions.Length; i++)
            {
                Point newPosition = position + directions[i];
                if (grid.GetTile(newPosition).isWall || grid.GetTile(newPosition).occupant != null) 
                {
                    continue;
                }
                Move(directions[i]);
                return true;
            }
            return false;
        }

        public void Attack(Creature creature)
        {
            // Check to see if the attack misses or is deflected by armor (uses a sigmoid function)
            if (Program.random.NextDouble() < 1 / (1 + Math.Pow(1.5, GetAccuracy()-creature.GetDodgeValue())))
            {
                LogConsole.UpdateLog(name + " misses " + creature.name + "!");
                return;
            }
            if (Program.random.NextDouble() < 1 / (1 + Math.Pow(1.4, GetArmorPierce() - creature.GetArmorValue()))
                && creature.GetArmorValue() > 0)
            {
                LogConsole.UpdateLog(name + " attacks " + creature.name + ", but it is deflected by its armor!");
                return;
            }

            int damage = 0;
            if (weapon != null)
            {
                damage = Utility.randRoundInt(
                    Utility.Roll((int)weapon.damageRolls, (int)weapon.damageDie)
                    + weapon.enchantment);
            } else
            {
                damage = Utility.randRoundInt(Utility.Roll(baseAttackRolls, baseAttackDie));
            }
            damage = Utility.randRoundInt(damage * (1 + GetStrength() * GameSettings.STRENGTH_DAMAGE_MULTIPLIER));
            if (weapon != null)
            {
                damage = Utility.randRoundInt(damage * Math.Pow(2, -creature.GetResistance(weapon.damageType)));
            } else
            {
                damage = Utility.randRoundInt(damage * Math.Pow(2, -creature.GetResistance("bludgeoning")));
            }
            // Armor damage reduction:
            if (creature.GetArmorValue() > 0)
            {
                int reduction = Program.random.Next((int)creature.GetArmorValue() + 1);
                damage -= reduction;
                // Strength can regain some damage
                if (GetStrength() > 0)
                {
                    damage += Math.Min(Program.random.Next(GetStrength() + 1), reduction);
                }
            }
            damage = Math.Max(damage, 0);
            LogConsole.UpdateLog(name + " attacks " + creature.name + " for " + damage + " damage!");
            Boolean died = creature.Damage(damage);
            if (died && this == PlayerManager.player)
            {
                PlayerManager.GainExp((creature.level+1) * (creature.level+1));
            }
        }

        // Returns whether the creature died
        public Boolean Damage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                Die();
                return true;
            }
            return false;
        }

        public void Heal(int amount)
        {
            health += amount;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }

        public void Die()
        {
            grid.creatures.Remove(this);
            grid.GetTile(position).occupant = null;
            for (int i = inventory.Count-1; i >= 0; i--)
            {
                DropItem(inventory[i]);
            }

            if (this == PlayerManager.player)
            {
                ModeManager.lockedMessage = true;
                ModeManager.mode = ModeManager.modes.Message;
                MessageConsole.strings.Clear();
                MessageConsole.strings.Add("You died.");
                MessageConsole.Render();
            }
            Debug.WriteLine(name);
            if (name == "dragon")
            {
                ModeManager.lockedMessage = true;
                ModeManager.mode = ModeManager.modes.Message;
                MessageConsole.strings.Clear();
                MessageConsole.strings.Add("You have slain the dragon!");
                MessageConsole.Render();
            }
        }

        public void GetItem(Item item)
        {
            inventory.Add(item);
            item.owner = this;
            if (item.position == null) { return; }
            Point pos = (Point)item.position;
            grid.tiles[pos.X, pos.Y].items.Remove(item);
            item.position = null;
        }

        public void EquipItem(Item item)
        {
            if (!inventory.Contains(item))
            {
                GetItem(item);
            }
            if (item.HasTag("WEAPON"))
            {
                if (weapon != null)
                {
                    weapon.equipped = false;
                }
                weapon = item;
                weapon.equipped = true;
            } else if (item.HasTag("ARMOR"))
            {
                if (armor != null)
                {
                    armor.equipped = false;
                }
                armor = item;
                armor.equipped = true;
            }
        }

        public void QuaffPotion(Item p)
        {
            // This is like the laziest way to do this but whatever
            if (!p.HasTag("POTION")) { return; }

            if (p.HasTag("HEALING"))
            {
                Heal(Utility.randRoundInt(maxHealth * 0.5));
            } else if (p.HasTag("STRENGTH"))
            {
                conditions.Add(new Condition(Conditions.conditions.Strong, 200));
            }
            else if(p.HasTag("DEXTERITY"))
            {
                conditions.Add(new Condition(Conditions.conditions.Agile, 200));
            }
            ConsumeItem(p);
        }

        public void DropItem(Item item)
        {
            if (weapon == item)
            {
                weapon = null;
            }
            if (armor == item)
            {
                armor = null;
            }
            item.equipped = false;
            item.owner = null;
            inventory.Remove(item);
            item.position = position;
            grid.tiles[position.X,position.Y].items.Add(item);
        }

        public void ConsumeItem(Item item)
        {
            if (weapon == item)
            {
                weapon = null;
            }
            if (armor == item)
            {
                armor = null;
            }
            item.equipped = false;
            item.owner = null;
            inventory.Remove(item);
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

        public int GetStrength()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].condition == Conditions.conditions.Strong)
                {
                    return strength + 4;
                }
            }
            return strength;
        }

        public int GetDexterity()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].condition == Conditions.conditions.Agile)
                {
                    return dexterity + 4;
                }
            }
            return dexterity;
        }

        public double GetMovementTime()
        {
            return GameSettings.BASE_MOVEMENT_TIME / movementSpeed / actionSpeed;
        }

        public double GetActionTime()
        {
            return GameSettings.BASE_ACTION_TIME / actionSpeed;
        }

        public double GetAttackTime()
        {
            if (weapon == null)
            {
                return GameSettings.BASE_MOVEMENT_TIME / actionSpeed;
            }
            else
            {
                return (int)weapon.attackTime / actionSpeed;
            }
        }

        public double GetDodgeValue()
        {
            return GetDexterity();
        }
        public double GetArmorValue()
        {
            if (armor != null)
            {
                if (armor.armorValue != null)
                {
                    return (int)armor.armorValue + armor.enchantment;
                }
            }
            return 0;
        }

        public double GetAccuracy()
        {
            double n = 1 + GetDexterity();
            if (weapon != null)
            {
                n += 0.5 * weapon.enchantment;
            }
            return n;
        }

        public double GetArmorPierce()
        {
            double n = 3 + GetStrength() * 2;
            if (weapon != null)
            {
                n += weapon.enchantment;
            }
            return n;
        }

        public Boolean HasTag(String tag)
        {
            return tags.Contains(tag);
        }
    }
}
