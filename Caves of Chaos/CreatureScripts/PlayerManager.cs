using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Input;
using static Caves_of_Chaos.GridScripts.GridManager;

namespace Caves_of_Chaos.CreatureScripts
{
    public static class PlayerManager
    {
        public static Creature player;

        public static void Init()
        {
            CreatureTemplate playerTemplate = new CreatureTemplate();
            playerTemplate.name = "Player";
            playerTemplate.symbol = "@";
            playerTemplate.color = "white";
            playerTemplate.health = 100;
            playerTemplate.strength = 10;
            playerTemplate.tags = new String[0];
            player = new Creature(new Point(Program.random.Next(activeGrid.width), Program.random.Next(activeGrid.height)),
                playerTemplate);
            while(activeGrid.GetTile(player.GetPosition()).isWall == true)
            {
                player.MoveTo(new Point(Program.random.Next(activeGrid.width), Program.random.Next(activeGrid.height)));
            }
        }

        public static void HandleInput(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Up))
            {
                PlayerManager.player.Move(new Point(0, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                PlayerManager.player.Move(new Point(0, 1));
            }

            if (keyboard.IsKeyPressed(Keys.Left))
            {
                PlayerManager.player.Move(new Point(-1, 0));
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                PlayerManager.player.Move(new Point(1, 0));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad7))
            {
                PlayerManager.player.Move(new Point(-1, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad9))
            {
                PlayerManager.player.Move(new Point(1, -1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad3))
            {
                PlayerManager.player.Move(new Point(1, 1));
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad1))
            {
                PlayerManager.player.Move(new Point(-1, 1));
            }
        }
    }
}
