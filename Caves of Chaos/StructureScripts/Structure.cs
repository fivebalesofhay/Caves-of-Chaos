using Caves_of_Chaos.GridScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.StructureScripts
{
    public class Structure
    {
        private Point position;

        public ColoredGlyph glyph;
        public String name;

        public Structure(Point initialPosition, Grid grid, StructureTemplate template)
        {
            if (template.backgroundColor == null)
            {
                template.backgroundColor = "black";
            }
            glyph = new ColoredGlyph(Palette.colors[template.color], Palette.colors[template.backgroundColor], template.symbol.ToCharArray()[0]);
            name = template.name;
            position = initialPosition;

            grid.tiles[position.X, position.Y].structure = this;
        }
    }
}
