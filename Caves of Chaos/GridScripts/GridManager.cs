using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;

namespace Caves_of_Chaos.GridScripts
{
    public static class GridManager
    {

        public static Grid activeGrid = new Grid(GameSettings.GRID_WIDTH, GameSettings.GRID_HEIGHT);
        public static void Init()
        {
            activeGrid.GenerateWalker();
            activeGrid.SpawnCreatures();
        }
    }
}
