using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameSettings;
using Caves_of_Chaos.CreatureScripts;
using System.Text.Json;
using System.Collections;
using Caves_of_Chaos.StructureScripts;
using Caves_of_Chaos.ItemScripts;
using System.Diagnostics;

namespace Caves_of_Chaos.GridScripts
{
    public class Grid
    {
        // All map tiles:
        public Tile[,] tiles;
        // Default tile (used to block movement off edge of map):
        public Tile defaultTile = new Tile(-1,-1);
        public Point[] upStairPositions = new Point[STAIRS_PER_LEVEL];
        public Point[] downStairPositions = new Point[STAIRS_PER_LEVEL];

        public List<Creature> creatures = new List<Creature>();

        public readonly int depth;
        public readonly int width;
        public readonly int height;
        public readonly double creatureDensity;
        public readonly double structureDensity;
        public readonly double itemDensity;

        public Grid(GridTemplate template)
        {
            depth = template.depth;
            width = template.width;
            height = template.height;
            creatureDensity = template.creatureDensity;
            structureDensity = template.structureDensity;
            itemDensity = template.itemDensity;

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
            GenerateStructures();
            SpawnCreatures();
            SpawnItems();
        }

        public Tile GetTile(Point position)
        {
            if (position.X >= width || position.X < 0 || position.Y >= height || position.Y < 0)
            {
                return defaultTile;
            }

            return tiles[position.X, position.Y];
        }

        public void GenerateStructures()
        {
            // Load structure raws:
            String[] raws = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Structures");
            List<StructureTemplate> templates = new List<StructureTemplate>();
            for (int i = 0; i < raws.Length; i++)
            {
                String text = File.ReadAllText(raws[i]);
                StructureTemplate? template = JsonSerializer.Deserialize<StructureTemplate>(text);
                if (template == null)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid structure template");
                    continue;
                }
                if (template.minDepth > depth || template.maxDepth < depth)
                {
                    continue;
                }
                templates.Add(template);
            }

            // Prepare for deciding to create structures
            double totalSpawnRatio = 0.0;
            for (int i = 0; i < templates.Count; i++)
            {
                totalSpawnRatio += templates[i].spawnRatio;
            }

            // Special generation for stairs
            int upStair = 0;
            int downStair = 0;
            for (int i = 0; i < templates.Count; i++)
            {
                if (templates[i].name == "up stair")
                {
                    upStair = i;
                }
                if (templates[i].name == "down stair")
                {
                    downStair = i;
                }
            }
            for (int i = 0; i < STAIRS_PER_LEVEL; i++)
            {
                if (depth > 0)
                {
                    Point point = new Point(Program.random.Next(width), Program.random.Next(height));
                    while (tiles[point.X, point.Y].structure != null || tiles[point.X, point.Y].isWall)
                    {
                        point = new Point(Program.random.Next(width), Program.random.Next(height));
                    }
                    Structure structure = new Structure(point, this, templates[upStair]);
                    upStairPositions[i] = point;
                }
                if (depth < GridManager.gridCount-1)
                {
                    Point point = new Point(Program.random.Next(width), Program.random.Next(height));
                    while (tiles[point.X, point.Y].structure != null || tiles[point.X, point.Y].isWall)
                    {
                        point = new Point(Program.random.Next(width), Program.random.Next(height));
                    }
                    Structure structure = new Structure(point, this, templates[downStair]);
                    downStairPositions[i] = point;
                }
            }

            // For every tile, maybe create structure:
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!GetTile(new Point(i, j)).isWall && Program.random.NextDouble() < structureDensity)
                    {
                        double randomIndex = Program.random.NextDouble() * totalSpawnRatio;
                        int chosenIndex = 0;
                        for (int k = 0; k < templates.Count; k++)
                        {
                            randomIndex -= templates[k].spawnRatio;
                            if (randomIndex <= 0)
                            {
                                chosenIndex = k;
                                break;
                            }
                        }
                        Structure structure = new Structure(new Point(i, j), this, templates[chosenIndex]);
                    }
                }
            }
        }

        public void SpawnCreatures()
        {
            // Prepare for deciding to spawn creatures
            List<CreatureTemplate> levelTemplates = new List<CreatureTemplate>();
            for (int i = 0; i < CreatureManager.templates.Count; i++)
            {
                if (CreatureManager.templates[i].minDepth <= depth && CreatureManager.templates[i].maxDepth >= depth)
                {
                    levelTemplates.Add(CreatureManager.templates[i]);
                }
            }
            double totalSpawnRatio = 0.0;
            for (int i = 0; i < levelTemplates.Count; i++)
            {
                totalSpawnRatio += levelTemplates[i].spawnRatio;
            }

            // For every tile, maybe spawn creature:
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!GetTile(new Point(i, j)).isWall && Program.random.NextDouble() < creatureDensity) {
                        double randomIndex = Program.random.NextDouble() * totalSpawnRatio;
                        int chosenIndex = 0;
                        for (int k = 0; k < levelTemplates.Count; k++)
                        {
                            randomIndex -= levelTemplates[k].spawnRatio;
                            if (randomIndex <= 0)
                            {
                                chosenIndex = k;
                                break;
                            }
                        }
                        Creature creature = new Creature(new Point(i, j), this, levelTemplates[chosenIndex]);
                    }
                }
            }
        }

        public void SpawnItems()
        {
            // To do: weighted item spawning
            // Prepare for deciding to spawn items
            List<ItemTemplate> levelTemplates = new List<ItemTemplate>();
            for (int i = 0; i < ItemManager.templates.Count; i++)
            {
                if (ItemManager.templates[i].minDepth <= depth && ItemManager.templates[i].maxDepth >= depth)
                {
                    levelTemplates.Add(ItemManager.templates[i]);
                }
            }
            double totalSpawnRatio = 0.0;
            for (int i = 0; i < levelTemplates.Count; i++)
            {
                totalSpawnRatio += 1;
            }

            // For every tile, maybe spawn item:
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!GetTile(new Point(i, j)).isWall && Program.random.NextDouble() < itemDensity)
                    {
                        double randomIndex = Program.random.NextDouble() * totalSpawnRatio;
                        int chosenIndex = 0;
                        for (int k = 0; k < levelTemplates.Count; k++)
                        {
                            randomIndex -= 1;
                            if (randomIndex <= 0)
                            {
                                chosenIndex = k;
                                break;
                            }
                        }
                        Item item = new Item(new Point(i, j), this, levelTemplates[chosenIndex]);
                        // Spawn some items enchanted:
                        if (item.HasTag("ARMOR") || item.HasTag("WEAPON")) {
                            if (Program.random.NextDouble() < ENCHANTMENT_CHANCE_ONE)
                            {
                                item.enchantment = 1;
                            }
                            else if (Program.random.NextDouble() < ENCHANTMENT_CHANCE_TWO)
                            {
                                item.enchantment = 2;
                            }
                            else if (Program.random.NextDouble() < ENCHANTMENT_CHANCE_THREE)
                            {
                                item.enchantment = 3;
                            }
                        }
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
