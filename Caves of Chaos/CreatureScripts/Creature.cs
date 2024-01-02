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
using Caves_of_Chaos.ItemScripts;
using System.Diagnostics;

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

        public int health;
        public double healingOffset = 0;
        public double actionPoints = 0;
        public List<Item> inventory = new List<Item>();
        public Item? weapon = null;
        public Item? armor = null;

        public Creature(Point initialPosition, Grid grid, CreatureTemplate template) 
        { 
            position = initialPosition;
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
            Point movement = new Point(0, 0);
            if (HasTag("AGRESSIVE"))
            {
                if (activeGrid.GetTile(position).isSeen)
                {
                    Point playerDiff = PlayerManager.player.GetPosition() - position;
                    if (Math.Abs(playerDiff.X) == Math.Abs(playerDiff.Y)
                        && !activeGrid.tiles[position.X + Math.Sign(playerDiff.X), position.Y + Math.Sign(playerDiff.Y)].isWall)
                    {
                        movement = new Point(Math.Sign(playerDiff.X), Math.Sign(playerDiff.Y));
                    }
                    else if (Math.Abs(playerDiff.X) > Math.Abs(playerDiff.Y))
                    {
                        if (Program.random.NextDouble() < Math.Abs(playerDiff.X / Utility.Distance(new Point(0, 0), playerDiff))
                            && !activeGrid.tiles[position.X + Math.Sign(playerDiff.X), position.Y + Math.Sign(playerDiff.Y)].isWall)
                        {
                            movement = new Point(Math.Sign(playerDiff.X), Math.Sign(playerDiff.Y));
                        } else
                        {
                            movement = new Point(Math.Sign(playerDiff.X), 0);
                        }
                    }
                    else
                    {
                        if (Program.random.NextDouble() < Math.Abs(playerDiff.Y / Utility.Distance(new Point(0, 0), playerDiff))
                            && !activeGrid.tiles[position.X + Math.Sign(playerDiff.X), position.Y + Math.Sign(playerDiff.Y)].isWall)
                        {
                            movement = new Point(Math.Sign(playerDiff.X), Math.Sign(playerDiff.Y));
                        }
                        else
                        {
                            movement = new Point(0, Math.Sign(playerDiff.Y));
                        }
                    }
                } else
                {
                    movement = Utility.RandomDirection();
                }
            } else
            {
                movement = Utility.RandomDirection();
            }

            // Don't attack anyone but the player
            if (activeGrid.GetTile(position + movement).occupant != PlayerManager.player 
                    && activeGrid.GetTile(position + movement).occupant != null)
            {
                return GetMovementTime();
            }

            return Move(movement);
        }

        public double Move(Point direction)
        {
            if (direction == new Point(0, 0)) return 0;
            Point newPosition = position + direction;
            if (activeGrid.GetTile(newPosition).isWall) { return 0; }
            if (activeGrid.GetTile(newPosition).occupant != null)
            {
                Attack(activeGrid.GetTile(newPosition).occupant);
                return GetAttackTime();
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
            damage = Utility.randRoundInt(damage * (1 + strength * GameSettings.STRENGTH_DAMAGE_MULTIPLIER));
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
                if (strength > 0)
                {
                    damage += Math.Min(Program.random.Next(strength + 1), reduction);
                }
            }
            damage = Math.Max(damage, 0);
            LogConsole.UpdateLog(name + " attacks " + creature.name + " for " + damage + " damage!");
            Boolean died = creature.Damage(damage);
            if (died && this == PlayerManager.player)
            {
                PlayerManager.GainExp(creature.level * creature.level);
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

        public void Die()
        {
            activeGrid.creatures.Remove(this);
            activeGrid.GetTile(position).occupant = null;
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
            if (name == "dragon")
            {
                ModeManager.lockedMessage = true;
                ModeManager.mode = ModeManager.modes.Message;
                MessageConsole.strings.Clear();
                MessageConsole.strings.Add("You won!!!!");
                MessageConsole.strings.Add("Yipeeeeeeeeeeeee :)");
                MessageConsole.Render();
            }
        }

        public void GetItem(Item item)
        {
            inventory.Add(item);
            item.owner = this;
            if (item.position == null) { return; }
            Point pos = (Point)item.position;
            activeGrid.tiles[pos.X, pos.Y].items.Remove(item);
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

        public void DropItem(Item item)
        {
            item.equipped = false;
            item.owner = null;
            inventory.Remove(item);
            item.position = position;
            activeGrid.tiles[position.X,position.Y].items.Add(item);
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
            return strength;
        }

        public int GetDexterity()
        {
            return dexterity;
        }

        public double GetMovementTime()
        {
            return GameSettings.BASE_MOVEMENT_TIME / movementSpeed / actionSpeed;
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
            return dexterity;
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
            double n = 1 + dexterity;
            if (weapon != null)
            {
                n += 0.5 * weapon.enchantment;
            }
            return n;
        }

        public double GetArmorPierce()
        {
            double n = 3 + strength * 2;
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
