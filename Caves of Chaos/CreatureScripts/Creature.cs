using Caves_of_Chaos.GridScripts;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.GridScripts.GridManager;

namespace Caves_of_Chaos.CreatureScripts
{
    public class Creature
    {
        private Point position = new Point();

        public ColoredGlyph glyph = new ColoredGlyph();
        public String name;
        public int maxHealth;
        public int baseAttackStrength;
        public String[] tags;

        public Creature(Point initialPosition, CreatureTemplate template) 
        { 
            position = initialPosition;
            this.glyph = new ColoredGlyph(Palette.colors[template.color], Palette.black, template.symbol.ToCharArray()[0]);

            this.name = template.name;
            this.maxHealth = template.health;
            this.baseAttackStrength = template.strength;
            this.tags = template.tags;

            GridManager.activeGrid.tiles[position.X, position.Y].occupant = this;
        }

        public void Move(Point direction)
        {
            Point newPosition = position + direction;
            if (activeGrid.GetTile(newPosition).isWall) { return; }
            activeGrid.GetTile(position).occupant = null;
            position = newPosition;
            activeGrid.GetTile(position).occupant = this;
        }
        
        public void MoveTo(Point location)
        {
            if (activeGrid.GetTile(location).isWall) { return; }
            activeGrid.GetTile(position).occupant = null;
            position = location;
            activeGrid.GetTile(position).occupant = this;
        }

        public Point GetPosition()
        {
            return position;
        }
    }
}
