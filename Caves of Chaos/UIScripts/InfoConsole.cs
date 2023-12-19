using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.CreatureScripts.PlayerManager;
using static GameSettings;
using static Caves_of_Chaos.Program;
using static Caves_of_Chaos.UIScripts.ModeManager;
using Caves_of_Chaos.GridScripts;
using SadConsole;
using static Caves_of_Chaos.GridScripts.GridManager;

namespace Caves_of_Chaos.UIScripts
{
    public static class InfoConsole
    {
        public static void UpdateStats()
        {
            container.infoConsole.Clear();
            if (mode == modes.Examine)
            {
                int index = 0;
                container.infoConsole.Print(1, index, "This is a " + 
                    (activeGrid.tiles[ExamineMode.pos.X,ExamineMode.pos.Y].isWall ? "wall." : "floor."));
                if (activeGrid.tiles[ExamineMode.pos.X, ExamineMode.pos.Y].structure != null)
                {
                    index++;
                    container.infoConsole.Print(1, index, 
                        Utility.Capitalize(activeGrid.tiles[ExamineMode.pos.X, ExamineMode.pos.Y].structure.name));
                }
                if (activeGrid.tiles[ExamineMode.pos.X, ExamineMode.pos.Y].occupant != null)
                {
                    index++;
                    container.infoConsole.Print(1, index, 
                        Utility.Capitalize(activeGrid.tiles[ExamineMode.pos.X, ExamineMode.pos.Y].occupant.name));
                }
                for (int i = 0; i < activeGrid.tiles[ExamineMode.pos.X,ExamineMode.pos.Y].items.Count;i++)
                {
                    index++;
                    container.infoConsole.Print(1, index,
                        Utility.Capitalize(activeGrid.tiles[ExamineMode.pos.X, ExamineMode.pos.Y].items[i].name));
                }
            }
            else
            {
                int index = 0;
                container.infoConsole.Print(1, index, "Health: " + player.health);
                index++;
                container.infoConsole.Print(1, index, "EXP: " + exp + "/" + (player.level * player.level * EXP_COEFFICIENT));
                index += 2;

                container.infoConsole.Print(1, index, "Strength: " + player.strength);
                index++;
                container.infoConsole.Print(1, index, "Dexterity: " + player.dexterity);
            }

            // Redraw borders:
            for (int i = 0; i < GRID_CONSOLE_HEIGHT; i++)
            {
                container.infoConsole.SetCellAppearance(0, i, new ColoredGlyph(Palette.white, Palette.black, 186));
            }
        }
    }
}
