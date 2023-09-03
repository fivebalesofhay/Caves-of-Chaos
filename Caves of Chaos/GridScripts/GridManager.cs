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

namespace Caves_of_Chaos.GridScripts
{
    public static class GridManager
    {

        public static List<Grid> grids = new List<Grid>();
        public static Grid activeGrid;

        public static void Init()
        {
            String[] raws = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Levels");
            List<GridTemplate> templates = new List<GridTemplate>();
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

            activeGrid = grids[0];
        }
    }
}
