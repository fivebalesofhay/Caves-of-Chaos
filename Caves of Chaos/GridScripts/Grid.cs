﻿using System;
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

        public List<Creature> creatures = new List<Creature>();

        public readonly int depth;
        public readonly int width;
        public readonly int height;
        public readonly double creatureDensity;

        public Grid(GridTemplate template)
        {
            depth = template.depth;
            width = template.width;
            height = template.height;
            creatureDensity = template.creatureDensity;

            tiles = new Tile[width, height];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }
        }

        public void Init()
        {
            GenerateWalker();
            SpawnCreatures();
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
            // Load creature raws:
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
                if (template.minDepth > depth || template.maxDepth < depth)
                {
                    continue;
                }
                templates.Add(template);
            }

            // Prepare for deciding to spawn creatures
            double totalSpawnRatio = 0.0;
            for (int i = 0; i < templates.Count; i++)
            {
                totalSpawnRatio += templates[i].spawnRatio;
            }

            // For every tile, maybe spawn creature:
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!GetTile(new Point(i, j)).isWall && Program.random.NextDouble() < creatureDensity) {
                        double randomIndex = Program.random.NextDouble() * totalSpawnRatio;
                        int chosenIndex = 0;
                        for (int k = 0; k < templates.Count; k++)
                        {
                            randomIndex -= templates[k].spawnRatio;
                            if (randomIndex < 0)
                            {
                                chosenIndex = k;
                                break;
                            }
                        }
                        Creature creature = new Creature(new Point(i, j), this, templates[chosenIndex]);
                        creatures.Add(creature);
                    }
                }
            }
        }

        public void GenerateWalker()
        {
            // Mapgen using a random walker method. Mediocre caves.
            // Starts at least one spot away from borders.
            Point walker = new Point(Program.random.Next(width - 2) + 1, Program.random.Next(height - 2) + 1);
            GetTile(walker).isWall = false;

            for (int i = 0; i < WALKER_STEPS; i++)
            {
                // Choose random direction:
                Point direction = Utility.RandomDirection();
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
