using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.CreatureScripts.PlayerManager;
using static GameSettings;
using static Caves_of_Chaos.Program;

namespace Caves_of_Chaos.UIScripts
{
    public static class InfoConsole
    {
        public static void updateStats()
        {
            container.infoConsole.Clear();
            container.infoConsole.Print(1, 0, "Health: " + player.health);

            // Redraw borders:
            for (int i = 0; i < GRID_CONSOLE_HEIGHT; i++)
            {
                container.infoConsole.SetCellAppearance(0, i, new ColoredGlyph(Palette.white, Palette.black, 186));
            }
        }
    }
}
