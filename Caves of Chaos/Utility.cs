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

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y-p2.Y) * (p1.Y - p2.Y));
        }

        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        public static List<Point> Line(Point start, Point end)
        {
            List<Point> points = new List<Point>();
            int x0 = start.X;
            int y0 = start.Y;
            int x1 = end.X;
            int y1 = end.Y;
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if ((!steep && start.X > end.X) || (steep && start.Y > end.Y))
                {
                    points.Add(steep ? new Point(y, x) : new Point(x, y));
                } else
                {
                    points.Insert(0, steep ? new Point(y, x) : new Point(x, y));
                }
                err = err - dY;
                if (err < 0) { y += ystep; err += dX; }
            }
            return points;
        }
    }
}
