using Caves_of_Chaos.CreatureScripts;
using Caves_of_Chaos.StructureScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.GridScripts
{
    public class Tile
    {
        public int x;
        public int y;

        public bool isWall = true;

        public Creature? occupant;
        public Structure? structure;

        public bool isSeen = false;
        public bool wasSeen = false;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
