﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.GridScripts.GridManager;
using SadConsole.Input;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;

namespace Caves_of_Chaos
{
    public static class Utility
    {
        public static Point[] directions =
        {
            new Point(1,-1),new Point(1,0),new Point(1,1),
            new Point(0,-1),new Point(0,1),
            new Point(-1,-1),new Point(-1,0),new Point(-1,1)
        };

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

        public static Point RandomPoint(GridScripts.Grid grid)
        {
            return new Point(Program.random.Next(grid.width), Program.random.Next(grid.height));
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

        public static int randRoundInt(double n)
        {
            if (Program.random.NextDouble() < n - Math.Floor(n))
            {
                return (int)Math.Floor(n);
            } else
            {
                return (int)Math.Ceiling(n);
            }
        }

        public static String Capitalize(String s) 
        {
            return s[0].ToString().ToUpper() + s.Substring(1,s.Length-1);
        }

        public static int Roll(int num, int die)
        {
            int total = 0;
            for (int i = 0; i < num; i++)
            {
                total += Program.random.Next(1, die + 1);
            }
            return total;
        }

        public static T[] Shuffle<T>(T[] array)
        {
            T[] arrayToReturn = (T[])array.Clone();
            int n = array.Length;
            while (n > 1)
            {
                int k = Program.random.Next(n--);
                T temp = arrayToReturn[n];
                arrayToReturn[n] = arrayToReturn[k];
                arrayToReturn[k] = temp;
            }
            return arrayToReturn;
        }
    }
}
