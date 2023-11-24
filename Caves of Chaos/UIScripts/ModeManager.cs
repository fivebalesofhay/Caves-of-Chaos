using Caves_of_Chaos.CreatureScripts;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.UIScripts
{
    public static class ModeManager
    {
        public enum modes {Grid, Examine};
        public static modes mode = modes.Grid;

        public static void HandleInput(Keyboard keyboard)
        {
            if (mode == modes.Grid)
            {
                // If key is a mode selector, change mode:
                if (keyboard.IsKeyPressed(Keys.X))
                {
                    mode = modes.Examine;
                    ExamineMode.pos = PlayerManager.player.GetPosition();
                }
                else // Send to PlayerManager
                {
                    PlayerManager.HandleInput(keyboard);
                }
            } 
            else if (mode == modes.Examine)
            {
                if (keyboard.IsKeyPressed(Keys.Escape))
                {
                    mode = modes.Grid;
                }
                else
                {
                    ExamineMode.HandleInput(keyboard);
                }
            }
        }
    }
}
