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
using Caves_of_Chaos.ItemScripts;
using Caves_of_Chaos.UIScripts;

namespace Caves_of_Chaos.CreatureScripts
{
    public static class PlayerManager
    {
        public static Creature player;
        public static int exp = 0;

        public static void Init()
        {
            // Initalize the player (note: find a better way to do this)
            CreatureTemplate playerTemplate = new CreatureTemplate();
            playerTemplate.name = "Player";
            playerTemplate.symbol = "@";
            playerTemplate.color = "white";
            playerTemplate.level = 1;
            playerTemplate.health = 12;
            playerTemplate.strength = 0;
            playerTemplate.dexterity = 0;
            playerTemplate.movementSpeed = 1;
            playerTemplate.actionSpeed = 1;
            playerTemplate.tags = new String[0];

            Point point = Utility.RandomPoint();

            while (activeGrid.GetTile(point).isWall == true
                || activeGrid.GetTile(point).occupant != null)
            {
                point = Utility.RandomPoint();
            }

            player = new Creature(point, activeGrid, playerTemplate);

            Item club = new Item(null, null, ItemManager.GetTemplate("club"));
            player.EquipItem(club);
        }

        public static void GainExp(int expGained)
        {
            exp += expGained;
            if (exp >= player.level * player.level * EXP_COEFFICIENT)
            {
                exp = exp - player.level * player.level * EXP_COEFFICIENT;
                LevelUp();
            }
        }

        public static void LevelUp()
        {
            player.level++;
            player.maxHealth = 6 + 6 * player.level;
            if (player.level % LEVELS_PER_STAT_INCREASE == 0)
            {
                ModeManager.IncreaseStats();
            }
        }

        public static void HandleInput(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8))
            {
                player.actionPoints -= player.Move(new Point(0, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2))
            {
                player.actionPoints -= player.Move(new Point(0, 1));
            }
            else if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.NumPad4))
            {
                player.actionPoints -= player.Move(new Point(-1, 0));
            }
            else if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.NumPad6))
            {
                player.actionPoints -= player.Move(new Point(1, 0));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad7))
            {
                player.actionPoints -= player.Move(new Point(-1, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad9))
            {
                player.actionPoints -= player.Move(new Point(1, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad3))
            {
                player.actionPoints -= player.Move(new Point(1, 1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad1))
            {
                player.actionPoints -= player.Move(new Point(-1, 1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad5))
            {
                player.actionPoints -= Math.Min(10, player.GetMovementTime());
            }
            else if (keyboard.IsKeyPressed(Keys.OemPeriod) && activeGrid.GetTile(player.GetPosition()).structure != null)
            {
                if (activeGrid.GetTile(player.GetPosition()).structure.HasTag("DOWN_STAIR"))
                {
                    activeGrid.GetTile(player.GetPosition()).occupant = null;
                    activeGrid.creatures.Remove(player);
                    activeGrid = grids[activeGrid.depth + 1];

                    Point point = Utility.RandomPoint();

                    while (activeGrid.GetTile(point).isWall == true
                        || activeGrid.GetTile(point).occupant != null)
                    {
                        point = Utility.RandomPoint();
                    }

                    player.MoveTo(point);
                    activeGrid.GetTile(player.GetPosition()).occupant = player;
                    activeGrid.creatures.Add(player);
                }
            }
            else if (keyboard.IsKeyPressed(Keys.OemComma) && activeGrid.GetTile(player.GetPosition()).structure != null)
            {
                if (activeGrid.GetTile(player.GetPosition()).structure.HasTag("UP_STAIR"))
                {
                    activeGrid.GetTile(player.GetPosition()).occupant = null;
                    activeGrid.creatures.Remove(player);
                    activeGrid = grids[activeGrid.depth - 1];

                    Point point = Utility.RandomPoint();

                    while (activeGrid.GetTile(point).isWall == true
                        || activeGrid.GetTile(point).occupant != null)
                    {
                        point = Utility.RandomPoint();
                    }

                    player.MoveTo(point);
                    activeGrid.GetTile(player.GetPosition()).occupant = player;
                    activeGrid.creatures.Add(player);
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
