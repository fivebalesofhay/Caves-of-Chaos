using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.StructureScripts
{
    public class Structure
    {
        public ColoredGlyph glyph;
        public String name;

        public Structure(StructureTemplate template)
        {
            glyph = new ColoredGlyph(Palette.colors[template.color], Palette.black, template.symbol.ToCharArray()[0]);
            name = template.name;
        }
    }
}
