using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameSettings;
using static Caves_of_Chaos.GridScripts.GridManager;
using System.ComponentModel;
using Caves_of_Chaos.CreatureScripts;
using SadConsole.Input;

namespace Caves_of_Chaos
{
    // The container for the game. Contains the main loop and handles input and rendering.
    public class GameContainer : ScreenObject
    {
        // Game world console:
        public SadConsole.Console gridConsole = new SadConsole.Console(GRID_CONSOLE_WIDTH, GRID_CONSOLE_HEIGHT);
        // Right side information console:
        public SadConsole.Console infoConsole = new SadConsole.Console(GAME_WIDTH - GRID_CONSOLE_WIDTH, GRID_CONSOLE_HEIGHT);
        // Bottom log console:
        public SadConsole.Console logConsole = new SadConsole.Console(GAME_WIDTH, GAME_HEIGHT - GRID_CONSOLE_HEIGHT);

        public GameContainer()
        {
            IsFocused = true;
            
            // Initialize consoles:
            gridConsole.Position = new Point(0, 0);
            gridConsole.DefaultBackground = Palette.black;

            infoConsole.Position = new Point(GRID_CONSOLE_WIDTH, 0);
            infoConsole.DefaultBackground = Palette.black;

            logConsole.Position = new Point(0, GRID_CONSOLE_HEIGHT);
            logConsole.DefaultBackground = Palette.black;

            // Generate borders:
            for (int i = 0; i < GRID_CONSOLE_HEIGHT; i++)
            {
                infoConsole.SetCellAppearance(0, i, new ColoredGlyph(Palette.white, Palette.black, 186));
            }
            for (int i = 0; i < GAME_WIDTH; i++)
            {
                if (i == GRID_CONSOLE_WIDTH)
                {
                    // infoConsole/logConsole intersection:
                    logConsole.SetCellAppearance(i, 0, new ColoredGlyph(Palette.white, Palette.black, 202));
                }
                else
                {
                    logConsole.SetCellAppearance(i, 0, new ColoredGlyph(Palette.white, Palette.black, 205));
                }
            }

            // Temp:
            infoConsole.Print(1, 0, "Stats:");
            logConsole.Print(0, 1, "Event Log:");

            Children.Add(gridConsole);
            Children.Add(infoConsole);
            Children.Add(logConsole);
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            // Main game loop:

            DrawGrid();
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            PlayerManager.HandleInput(keyboard);

            return true;
        }

        public void DrawGrid()
        {
            for (int i = 0; i < activeGrid.tiles.GetLength(0); i++)
            {
                for (int j = 0; j < activeGrid.tiles.GetLength(1); j++)
                {
                    if (activeGrid.tiles[i, j].occupant != null)
                    {
                        gridConsole.SetCellAppearance(i, j, activeGrid.tiles[i, j].occupant.glyph);
                    }
                    else if (activeGrid.tiles[i, j].isWall)
                    {
                        gridConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, '#'));
                    }
                    else
                    {
                        gridConsole.SetCellAppearance(i, j, new ColoredGlyph(Palette.white, Palette.black, '.'));
                    }
                }
            }
        }
    }
}
