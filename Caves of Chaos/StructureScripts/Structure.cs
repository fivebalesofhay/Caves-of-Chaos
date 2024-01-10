using Caves_of_Chaos.GridScripts;
using System;
using System.Collections;
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
        public String[] tags;

        public Structure? linkedStair = null;

        public Structure(Point initialPosition, Grid grid, StructureTemplate template)
        {
            if (template.backgroundColor == null)
            {
                template.backgroundColor = "black";
            }
            if (template.symbolIndex != null)
            {
                glyph = new ColoredGlyph(Palette.colors[template.color], Palette.colors[template.backgroundColor], 
                    (int)template.symbolIndex);
            } else
            {
                glyph = new ColoredGlyph(Palette.colors[template.color], Palette.colors[template.backgroundColor], 
                    template.symbol.ToCharArray()[0]);
            }
            name = template.name;
            position = initialPosition;
            tags = template.tags;

            grid.tiles[position.X, position.Y].structure = this;

            if (template.growth != null)
            {
                Grow((int)template.growth, grid);
            }
        }

        public Point GetPosition()
        {
            return position;
        }

        public Boolean HasTag(String s)
        {
            return tags.Contains(s);
        }

        private void Grow(int steps, Grid grid)
        {
            List<Point> points = new List<Point> { position };
            for (int i = 0; i < steps; i++)
            {
                Point pos = points[Program.random.Next(points.Count)];
                Point dir = Utility.RandomDirection();
                if (grid.GetTile(pos + dir).isWall || grid.GetTile(pos + dir).structure != null) continue;
                grid.GetTile(pos + dir).structure = this;
                points.Add(pos + dir);
            }
        }
    }
}
