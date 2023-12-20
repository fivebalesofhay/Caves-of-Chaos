using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.Program;
using static GameSettings;

namespace Caves_of_Chaos.UIScripts
{
    public static class MessageConsole
    {
        public static List<String> strings = new List<String>();
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

            for (int i = 0; i < strings.Count; i++)
            {
                container.smallScreenConsole.Print(1, i + 1, strings[i]);
            }
        }
    }
}
