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
using Caves_of_Chaos.StructureScripts;

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

            Grid initialGrid = grids[0];
            Point point = Utility.RandomPoint(initialGrid);

            while (initialGrid.GetTile(point).isWall == true
                || initialGrid.GetTile(point).occupant != null)
            {
                point = Utility.RandomPoint(initialGrid);
            }

            player = new Creature(point, initialGrid, playerTemplate);

            Item club = new Item(null, null, ItemManager.GetTemplate("club"));
            player.EquipItem(club);
        }

        public static void GainExp(int expGained)
        {
            exp += expGained;
            if (exp >= (player.level+1) * (player.level + 1) * EXP_COEFFICIENT)
            {
                exp = exp - (player.level + 1) * (player.level + 1) * EXP_COEFFICIENT;
                LevelUp();
            }
        }

        public static void LevelUp()
        {
            player.level++;
            player.maxHealth = 6 + 6 * player.level;
            if (player.level % LEVELS_PER_STAT_INCREASE == 0 && !ModeManager.lockedMessage)
            {
                ModeManager.IncreaseStats();
            }
        }

        public static void HandleInput(Keyboard keyboard)
        {
            Structure stair = player.grid.GetTile(player.GetPosition()).structure;
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
            else if (keyboard.IsKeyPressed(Keys.OemPeriod) && stair != null)
            {
                if (stair.HasTag("DOWN_STAIR"))
                {
                    player.UseStair(stair);
                    player.actionPoints -= player.GetMovementTime();
                }
            }
            else if (keyboard.IsKeyPressed(Keys.OemComma) && stair != null)
            {
                if (stair.HasTag("UP_STAIR"))
                {
                    player.UseStair(stair);
                    player.actionPoints -= player.GetMovementTime();
                }
            }

            // Field of View:
            for (int i = 0; i < player.grid.tiles.GetLength(0); i++)
            {
                for (int j = 0; j < player.grid.tiles.GetLength(1); j++)
                {
                    if (player.grid.GetTile(new Point(i, j)).isSeen) {
                        player.grid.GetTile(new Point(i, j)).isSeen = false;
                        player.grid.GetTile(new Point(i, j)).wasSeen = true;
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
                        player.grid.GetTile(line[k]).isSeen = true;
                        if (player.grid.GetTile(line[k]).isWall)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
