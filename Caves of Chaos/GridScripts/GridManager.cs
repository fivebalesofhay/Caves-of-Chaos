using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using Caves_of_Chaos.CreatureScripts;
using System.Text.Json;
using Caves_of_Chaos.StructureScripts;

namespace Caves_of_Chaos.GridScripts
{
    public static class GridManager
    {

        public static List<Grid> grids = new List<Grid>();
        public static int gridCount = 0;

        public static void Init()
        {
            String[] raws = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Levels");
            List<GridTemplate> templates = new List<GridTemplate>();
            gridCount = raws.Length;
            for (int i = 0; i < raws.Length; i++)
            {
                String text = File.ReadAllText(raws[i]);
                GridTemplate? template = JsonSerializer.Deserialize<GridTemplate>(text);
                if (template == null)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid level template");
                    continue;
                }
                templates.Add(template);
            }

            for (int i = 0; i < templates.Count; i++)
            {
                Grid grid = new Grid(templates[i]);
                grids.Add(grid);
                grid.Init();
            }

            // Link stairs:
            for (int i = 0; i < gridCount - 1; i++)
            {
                for (int j = 0; j < grids[i].downStairPositions.Length; j++)
                {
                    Point stairPos = grids[i].downStairPositions[j];
                    Structure? stair = grids[i].tiles[stairPos.X, stairPos.Y].structure;
                    Point nextStairPos = grids[i+1].upStairPositions[j];
                    Structure? nextStair = grids[i + 1].tiles[nextStairPos.X, nextStairPos.Y].structure;
                    if (stair != null && nextStair != null)
                    {
                        stair.linkedStair = nextStair;
                        nextStair.linkedStair = stair;
                    }
                    
                }
            }
        }
    }
}
