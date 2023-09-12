using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caves_of_Chaos.GridScripts;
using SadConsole.Input;
using static Caves_of_Chaos.GridScripts.GridManager;
using static GameSettings;

namespace Caves_of_Chaos.CreatureScripts
{
    public static class PlayerManager
    {
        public static Creature player;

        public static void Init()
        {
            // Initalize the player (note: find a better way to do this)
            CreatureTemplate playerTemplate = new CreatureTemplate();
            playerTemplate.name = "Player";
            playerTemplate.symbol = "@";
            playerTemplate.color = "white";
            playerTemplate.health = 100;
            playerTemplate.strength = 5;
            playerTemplate.tags = new String[0];

            Point point = Utility.RandomPoint();

            while (activeGrid.GetTile(point).isWall == true
                || activeGrid.GetTile(point).occupant != null)
            {
                point = Utility.RandomPoint();
            }

            player = new Creature(point, activeGrid, playerTemplate);
        }

        public static void HandleInput(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8))
            {
                player.Move(new Point(0, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2))
            {
                player.Move(new Point(0, 1));
            }
            else if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.NumPad4))
            {
                player.Move(new Point(-1, 0));
            }
            else if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.NumPad6))
            {
                player.Move(new Point(1, 0));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad7))
            {
                player.Move(new Point(-1, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad9))
            {
                player.Move(new Point(1, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad3))
            {
                player.Move(new Point(1, 1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad1))
            {
                player.Move(new Point(-1, 1));
            }
            else if (keyboard.IsKeyPressed(Keys.OemPeriod) && activeGrid.GetTile(player.GetPosition()).structure != null)
            {
                if (activeGrid.GetTile(player.GetPosition()).structure.name == "Down Stair")
                {
                    activeGrid = grids[activeGrid.depth + 1];

                    Point point = Utility.RandomPoint();

                    while (activeGrid.GetTile(point).isWall == true
                        || activeGrid.GetTile(point).occupant != null)
                    {
                        point = Utility.RandomPoint();
                    }

                    player.MoveTo(point);
                }
            }
            else if (keyboard.IsKeyPressed(Keys.OemComma) && activeGrid.GetTile(player.GetPosition()).structure != null)
            {
                if (activeGrid.GetTile(player.GetPosition()).structure.name == "Up Stair")
                {
                    activeGrid = grids[activeGrid.depth - 1];

                    Point point = Utility.RandomPoint();

                    while (activeGrid.GetTile(point).isWall == true
                        || activeGrid.GetTile(point).occupant != null)
                    {
                        point = Utility.RandomPoint();
                    }

                    player.MoveTo(point);
                }
            }

            // Field of View:
            for (int i = 0; i < activeGrid.tiles.GetLength(0); i++)
            {
                for (int j = 0; j < activeGrid.tiles.GetLength(1); j++)
                {
                    if (activeGrid.GetTile(new Point(i, j)).isSeen) {
                        activeGrid.GetTile(new Point(i, j)).isSeen = false;
                        activeGrid.GetTile(new Point(i, j)).wasSeen = true;
                    }
                }
            }

            for (int i = player.GetPosition().X - PLAYER_FOV_DISTANCE; i < player.GetPosition().X + PLAYER_FOV_DISTANCE; i++)
            {
                for (int j = player.GetPosition().Y - PLAYER_FOV_DISTANCE; j < player.GetPosition().Y + PLAYER_FOV_DISTANCE; j++)
                {
                    Point point = new Point(i, j);
                    List<Point> line = Utility.Line(point, player.GetPosition());
                    for (int k = 0; k < line.Count; k++)
                    {
                        activeGrid.GetTile(line[k]).isSeen = true;
                        if (activeGrid.GetTile(line[k]).isWall)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
