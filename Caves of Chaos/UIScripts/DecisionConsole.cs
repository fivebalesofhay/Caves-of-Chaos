using Caves_of_Chaos.CreatureScripts;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.Program;
using static GameSettings;

namespace Caves_of_Chaos.UIScripts
{
    // Remember to set list and onSelection when you do this.
    public static class DecisionConsole
    {
        public static int selection = 0;
        public static List<String> list = new List<String>();
        public static Action<int> onSelection = i => { };

        public static void Render()
        {
            container.smallScreenConsole.IsVisible = true;
            container.smallScreenConsole.Clear();

            // Borders:
            for (int i = 0; i < SMALL_SCREEN_WIDTH; i++)
            {
                for (int j = 0; j < SMALL_SCREEN_HEIGHT; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        container.smallScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 201));
                    }
                    else if (i == 0 && j == SMALL_SCREEN_HEIGHT - 1)
                    {
                        container.smallScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 200));
                    }
                    else if (i == SMALL_SCREEN_WIDTH - 1 && j == 0)
                    {
                        container.smallScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 187));
                    }
                    else if (i == SMALL_SCREEN_WIDTH - 1 && j == SMALL_SCREEN_HEIGHT - 1)
                    {
                        container.smallScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 188));
                    }
                    else if (i == 0 || i == SMALL_SCREEN_WIDTH - 1)
                    {
                        container.smallScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 186));
                    }
                    else if (j == 0 || j == SMALL_SCREEN_HEIGHT - 1)
                    {
                        container.smallScreenConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, 205));
                    }
                }
            }

            // Items:
            for (int i = 0; i < list.Count; i++)
            {
                ColoredString s = new ColoredString(list[i]);
                if (i == selection)
                {
                    s = new ColoredString(list[i], Palette.black, Palette.white);
                }
                container.smallScreenConsole.Print(1, i + 1, s);
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
            }
            if (selection < 0)
            {
                selection = 0;
            }
            if (selection >= list.Count)
            {
                selection = list.Count - 1;
            }
        }
    }
}
