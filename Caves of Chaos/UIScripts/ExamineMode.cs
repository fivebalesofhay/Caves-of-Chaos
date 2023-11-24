using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Input;

namespace Caves_of_Chaos.UIScripts
{
    public static class ExamineMode
    {
        public static Point pos = new Point(0,0);
        public static void HandleInput(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8))
            {
                pos += new Point(0, -1);
            }
            else if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2))
            {
                pos += new Point(0, 1);
            }
            else if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.NumPad4))
            {
                pos += new Point(-1, 0);
            }
            else if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.NumPad6))
            {
                pos += new Point(1, 0);
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad7))
            {
                pos += new Point(-1, -1);
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad9))
            {
                pos += new Point(1, -1);
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad3))
            {
                pos += new Point(1, 1);
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad1))
            {
                pos += new Point(-1, 1);
            }
        }
    }
}
