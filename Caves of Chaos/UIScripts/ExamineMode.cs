using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caves_of_Chaos.GridScripts;
using Caves_of_Chaos.ItemScripts;
using SadConsole.Input;

namespace Caves_of_Chaos.UIScripts
{
    public static class ExamineMode
    {
        public static Point pos = new Point(0,0);
        public static void HandleInput(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8))
            {
                pos += new Point(0, -1);
            }
            else if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2))
            {
                pos += new Point(0, 1);
            }
            else if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.NumPad4))
            {
                pos += new Point(-1, 0);
            }
            else if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.NumPad6))
            {
                pos += new Point(1, 0);
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad7))
            {
                pos += new Point(-1, -1);
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad9))
            {
                pos += new Point(1, -1);
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad3))
            {
                pos += new Point(1, 1);
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad1))
            {
                pos += new Point(-1, 1);
            } else if (keyboard.IsKeyPressed(Keys.Enter))
            {
                if (GridManager.activeGrid.tiles[pos.X,pos.Y].occupant != null)
                {
                    CreatureScripts.Creature c = GridManager.activeGrid.tiles[pos.X, pos.Y].occupant;
                    MessageConsole.strings.Clear();
                    MessageConsole.strings.Add(Utility.Capitalize(c.name));
                    MessageConsole.strings.Add("");
                    MessageConsole.strings.Add("Health: " + c.health + "/" + c.maxHealth);
                    MessageConsole.strings.Add("");
                    MessageConsole.strings.Add("Strength: " + c.GetStrength());
                    MessageConsole.strings.Add("Dexterity: " + c.GetDexterity());
                    MessageConsole.strings.Add("");
                    MessageConsole.strings.Add("Movement Speed: " + c.movementSpeed);
                    MessageConsole.strings.Add("Action Speed: " + c.actionSpeed);
                    MessageConsole.strings.Add("");
                    if (c.weapon != null)
                    {
                        MessageConsole.strings.Add("Wielding: " + c.weapon.name);
                    }
                    if (c.armor != null)
                    {
                        MessageConsole.strings.Add("Wearing: " + c.armor.name);
                    }
                    if (c.weapon != null || c.armor != null)
                    {
                        MessageConsole.strings.Add("");
                    }
                    foreach (KeyValuePair<string, int> entry in c.resistances)
                    {
                        if (entry.Value > 0)
                        {
                            MessageConsole.strings.Add("Resistant to " + entry.Key + " damage");
                        } else if (entry.Value < 0)
                        {
                            MessageConsole.strings.Add("Vulnerable to " + entry.Key + " damage");
                        }
                    }
                    ModeManager.mode = ModeManager.modes.Message;
                    MessageConsole.Render();
                }
            }
        }
    }
}
