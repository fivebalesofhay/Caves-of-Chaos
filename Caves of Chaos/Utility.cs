using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.GridScripts.GridManager;

namespace Caves_of_Chaos
{
    public static class Utility
    {
        public static Point RandomDirection()
        {
            Point direction = new Point(0, 0);
            if (Program.random.NextDouble() > 0.5)
            {
                direction = new Point(Program.random.Next(2) * 2 - 1, 0); // left/right
            }
            else
            {
                direction = new Point(0, Program.random.Next(2) * 2 - 1); // up/down
            }
            return direction;
        }

        public static Point RandomPoint()
        {
            return new Point(Program.random.Next(activeGrid.width), Program.random.Next(activeGrid.height));
        }
    }
}
