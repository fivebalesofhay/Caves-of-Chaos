using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameSettings;
using static Caves_of_Chaos.GridScripts.GridManager;
using static Caves_of_Chaos.CreatureScripts.PlayerManager;
using System.ComponentModel;
using Caves_of_Chaos.CreatureScripts;
using SadConsole.Input;
using System.Diagnostics;
using Caves_of_Chaos.UIScripts;

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
        // Inventory Screen:
        public SadConsole.Console largeScreenConsole = new SadConsole.Console(LARGE_SCREEN_WIDTH, LARGE_SCREEN_HEIGHT);
        // Decisions and messages screen:
        public SadConsole.Console smallScreenConsole = new SadConsole.Console(SMALL_SCREEN_WIDTH, SMALL_SCREEN_HEIGHT);

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

            largeScreenConsole.Position = new Point((GAME_WIDTH - LARGE_SCREEN_WIDTH) / 2, (GAME_HEIGHT - LARGE_SCREEN_HEIGHT) / 2);
            largeScreenConsole.DefaultBackground = Palette.black;

            smallScreenConsole.Position = new Point((GAME_WIDTH - SMALL_SCREEN_WIDTH) / 2, (GAME_HEIGHT - SMALL_SCREEN_HEIGHT) / 2);
            smallScreenConsole.DefaultBackground = Palette.black;

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

            infoConsole.Print(1, 0, "Stats:");
            logConsole.Print(0, 1, "Event Log:");

            Children.Add(gridConsole);
            Children.Add(infoConsole);
            Children.Add(logConsole);
            Children.Add(largeScreenConsole);
            Children.Add(smallScreenConsole);
            largeScreenConsole.IsVisible = false;
            smallScreenConsole.IsVisible = false;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            // Main game loop:
            while (player.actionPoints < 0 && player.health > 0)
            {
                CreatureManager.UpdateCreatures();
            }
            DrawGrid();
            InfoConsole.UpdateStats();
            ModeManager.Draw();
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            ModeManager.HandleInput(keyboard);
            
            return true;
        }

        public void DrawGrid()
        {
            int leftMargin = player.GetPosition().X - GRID_CONSOLE_WIDTH / 2;
            int topMargin = player.GetPosition().Y - GRID_CONSOLE_HEIGHT / 2;

            if (leftMargin < 0)
            {
                leftMargin = 0;
            }
            if (leftMargin + GRID_CONSOLE_WIDTH > player.grid.width)
            {
                leftMargin = player.grid.width - GRID_CONSOLE_WIDTH;
            }
            if (topMargin < 0)
            {
                topMargin = 0;
            }
            if (topMargin + GRID_CONSOLE_HEIGHT > player.grid.height)
            {
                topMargin = player.grid.height - GRID_CONSOLE_HEIGHT;
            }

            for (int i = leftMargin; i < leftMargin + GRID_CONSOLE_WIDTH; i++)
            {
                for (int j = topMargin; j < topMargin + GRID_CONSOLE_HEIGHT; j++)
                {
                    DrawTile(i, j, leftMargin, topMargin);
                }
            }
        }

        public void DrawTile(int i, int j, int leftMargin, int topMargin)
        {
            if (ModeManager.mode == ModeManager.modes.Examine && ExamineMode.pos == new Point(i, j))
            {
                gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, 'X'));
            }
            else if (player.grid.GetTile(new Point(i, j)).isSeen)
            {
                if (player.grid.tiles[i, j].occupant != null)
                {
                    gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, player.grid.tiles[i, j].occupant.glyph);
                }
                else if (player.grid.tiles[i, j].structure != null)
                {
                    if (player.grid.tiles[i, j].items.Count > 0 && !player.grid.tiles[i, j].structure.HasTag("PRIORITY"))
                    {
                        if (player.grid.tiles[i, j].items[0].HasTag("WEAPON"))
                        {
                            gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, '/'));
                        }
                        else if (player.grid.tiles[i, j].items[0].HasTag("ARMOR"))
                        {
                            gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, ']'));
                        }
                        else if (player.grid.tiles[i, j].items[0].HasTag("POTION"))
                        {
                            gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, '!'));
                        }
                        else
                        {
                            gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, '%'));
                        }
                    }
                    else
                    {
                        gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, player.grid.tiles[i, j].structure.glyph);
                    }
                }
                else if (player.grid.tiles[i, j].items.Count > 0)
                {
                    if (player.grid.tiles[i, j].items[0].HasTag("WEAPON")) {
                        gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, '/'));
                    } 
                    else if (player.grid.tiles[i, j].items[0].HasTag("ARMOR"))
                    {
                        gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, ']'));
                    }
                    else if(player.grid.tiles[i, j].items[0].HasTag("POTION")) 
                    {
                        gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, '!'));
                    } 
                    else
                    {
                        gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, '%'));
                    }
                }
                else if (player.grid.tiles[i, j].isWall)
                {
                    gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, '#'));
                }
                else
                {
                    gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.white, Palette.black, '.'));
                }
            }
            else if (player.grid.GetTile(new Point(i, j)).wasSeen)
            {
                if (player.grid.tiles[i, j].structure != null)
                {
                    gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.lightGray, Palette.black, 
                        player.grid.tiles[i, j].structure.glyph.Glyph));
                }
                else if (player.grid.tiles[i, j].isWall)
                {
                    gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.lightGray, Palette.black, '#'));
                }
                else
                {
                    gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.lightGray, Palette.black, '.'));
                }
            }
            else
            {
                gridConsole.SetCellAppearance(i - leftMargin, j - topMargin, new ColoredGlyph(Palette.black));
            }
        }
    }
}