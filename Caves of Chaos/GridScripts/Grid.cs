using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameSettings;
using Caves_of_Chaos.CreatureScripts;
using System.Text.Json;
using System.Collections;

namespace Caves_of_Chaos.GridScripts
{
    public class Grid
    {
        // All map tiles:
        public Tile[,] tiles;
        // Default tile (used to block movement off edge of map):
        public Tile defaultTile = new Tile(-1,-1);

        public readonly int width = 0;
        public readonly int height = 0;

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;

            tiles = new Tile[width, height];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }
        }

        public Tile GetTile(Point position)
        {
            if (position.X >= width || position.X < 0 || position.Y >= height || position.Y < 0)
            {
                return defaultTile;
            }

            return tiles[position.X, position.Y];
        }

        public void SpawnCreatures()
        {
            String[] raws = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Creatures");
            List<CreatureTemplate> templates = new List<CreatureTemplate>();
            for (int i = 0; i < raws.Length; i++)
            {
                String text = File.ReadAllText(raws[i]);
                CreatureTemplate? template = JsonSerializer.Deserialize<CreatureTemplate>(text);
                if (template == null)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid creature template");
                    continue;
                }
                templates.Add(template);
            }

            for (int i = 0; i < templates.Count; i++)
            {
                Creature creature = new Creature(new Point(Program.random.Next(width), Program.random.Next(height)),
                    templates[i]);
                while (GetTile(creature.GetPosition()).isWall == true)
                {
                    creature.MoveTo(new Point(Program.random.Next(width), Program.random.Next(height)));
                }
            }
        }

        public void GenerateWalker()
        {
            // Mapgen using a random walker method. Somewhat good-looking caves.
            // Starts at least one spot away from borders.
            Point walker = new Point(Program.random.Next(width - 2) + 1, Program.random.Next(height - 2) + 1);
            GetTile(walker).isWall = false;

            for (int i = 0; i < WALKER_STEPS; i++)
            {
                // Choose random direction:
                Point direction = new Point(0, 0);
                if (Program.random.NextDouble() > 0.5)
                {
                    direction = new Point(Program.random.Next(2) * 2 - 1, 0); // left/right
                } else
                {
                    direction = new Point(0, Program.random.Next(2) * 2 - 1); // up/down
                }
                int length = Program.random.Next(6) + 1;

                while (walker.X + direction.X * length <= 0 || walker.X + direction.X * length >= width - 1
                    || walker.Y + direction.Y * length <= 0 || walker.Y + direction.Y * length >= height - 1)
                {
                    if (Program.random.NextDouble() > 0.5)
                    {
                        direction = new Point(Program.random.Next(2) * 2 - 1, 0); // left/right
                    }
                    else
                    {
                        direction = new Point(0, Program.random.Next(2) * 2 - 1); // up/down
                    }
                    length = Program.random.Next(8) + 1;
                }

                for (int j = 0; j < length; j++)
                {
                    walker += direction;
                    GetTile(walker).isWall = false;
                }
            }
        }
    }
}
