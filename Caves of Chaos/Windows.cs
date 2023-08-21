using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameSettings;

namespace Caves_of_Chaos
{
    internal class Windows
    {
        public static void Init()
        {
            // Container:
            ScreenObject container = new ScreenObject();
            Game.Instance.Screen = container;
            Game.Instance.DestroyDefaultStartingConsole();

            // Game world console:
            SadConsole.Console gridConsole = new SadConsole.Console(GRID_WIDTH, GRID_HEIGHT);
            gridConsole.Position = new Point(0, 0);
            gridConsole.DefaultBackground = Palette.Black;

            // Right side information console:
            SadConsole.Console infoConsole = new SadConsole.Console(GAME_WIDTH - GRID_WIDTH, GRID_HEIGHT);
            infoConsole.Position = new Point(GRID_WIDTH, 0);
            infoConsole.DefaultBackground = Palette.Black;

            // Bottom log console:
            SadConsole.Console logConsole = new SadConsole.Console(GAME_WIDTH, GAME_HEIGHT - GRID_HEIGHT);
            logConsole.Position = new Point(0, GRID_HEIGHT);
            logConsole.DefaultBackground = Palette.Black;

            // Generate borders:
            for (int i = 0; i < GRID_HEIGHT; i++)
            {
                infoConsole.SetCellAppearance(0, i, new ColoredGlyph(Palette.White, Palette.Black, 186));
            }
            for (int i = 0; i < GAME_WIDTH; i++)
            {
                if (i == GRID_WIDTH)
                {
                    // infoConsole/logConsole intersection
                    logConsole.SetCellAppearance(i, 0, new ColoredGlyph(Palette.White, Palette.Black, 202));
                }
                else 
                {
                    logConsole.SetCellAppearance(i, 0, new ColoredGlyph(Palette.White, Palette.Black, 205));
                }
            }

            // Temp:
            infoConsole.Print(1, 0, "Stats:");
            logConsole.Print(0, 1, "Event Log:");

            container.Children.Add(gridConsole);
            container.Children.Add(infoConsole);
            container.Children.Add(logConsole);
        }
    }
}
