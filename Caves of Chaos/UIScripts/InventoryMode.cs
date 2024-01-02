using Caves_of_Chaos.CreatureScripts;
using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.Program;
using static GameSettings;

namespace Caves_of_Chaos.UIScripts
{
    public static class InventoryMode
    {
        public static int selection = 0;

        public static void Render()
        {
            container.largeScreenConsole.IsVisible = true;
            container.largeScreenConsole.Clear();

            // Borders:
            for (int i = 0; i < LARGE_SCREEN_WIDTH; i++)
            {
                for (int j = 0; j < LARGE_SCREEN_HEIGHT; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        container.largeScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 201));
                    } else if (i == 0 && j == LARGE_SCREEN_HEIGHT - 1)
                    {
                        container.largeScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 200));
                    }
                    else if (i == LARGE_SCREEN_WIDTH-1 && j == 0)
                    {
                        container.largeScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 187));
                    }
                    else if (i == LARGE_SCREEN_WIDTH-1 && j == LARGE_SCREEN_HEIGHT-1)
                    {
                        container.largeScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 188));
                    }
                    else if (i == 0 || i == LARGE_SCREEN_WIDTH-1)
                    {
                        container.largeScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 186));
                    } else if (j == 0 || j == LARGE_SCREEN_HEIGHT - 1)
                    {
                        container.largeScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 205));
                    }
                }
            }

            // Items:
            for (int i = 0; i < PlayerManager.player.inventory.Count; i++)
            {
                ColoredString s = new ColoredString(PlayerManager.player.inventory[i].DisplayName());
                if (i == selection)
                {
                    s = new ColoredString(PlayerManager.player.inventory[i].DisplayName(), Palette.black, Palette.white);
                }
                if (PlayerManager.player.inventory[i].equipped)
                {
                    s += " (e)";
                }
                container.largeScreenConsole.Print(1, i+1, s);
            }
        }

        public static void HandleInput(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8))
            {
                selection--;
            }
            else if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2))
            {
                selection++;
            } else if (keyboard.IsKeyPressed(Keys.Enter))
            {
                ItemScripts.Item item = PlayerManager.player.inventory[selection];
                MessageConsole.strings.Clear();
                MessageConsole.strings.Add(Utility.Capitalize(item.DisplayName()));
                MessageConsole.strings.Add("");
                if (item.HasTag("WEAPON"))
                {
                    MessageConsole.strings.Add("Damage: " + item.damageRolls + "d" + item.damageDie);
                    MessageConsole.strings.Add("Attack Time: " + item.attackTime);
                } else if (item.HasTag("ARMOR"))
                {
                    MessageConsole.strings.Add("Armor Value: " + item.armorValue);
                }
                ModeManager.mode = ModeManager.modes.Message;
                MessageConsole.Render();
            }
            if (selection < 0)
            {
                selection = 0;
            }
            if (selection >= PlayerManager.player.inventory.Count)
            {
                selection = PlayerManager.player.inventory.Count - 1;
            }
        }
    }
}
