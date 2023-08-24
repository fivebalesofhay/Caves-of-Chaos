using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.Program;
using static GameSettings;

namespace Caves_of_Chaos.UIScripts
{
    public static class LogConsole
    {
        public static List<String> log = new List<String>();

        public static void UpdateLog(String message)
        {
            container.logConsole.Clear();
            log.Insert(0, message);
            for (int i = 0; i < log.Count && i < container.logConsole.Height - 3; i++)
            {
                container.logConsole.Print(0, container.logConsole.Height - i - 1, log[i]);
            }
            // Redraw borders and stuff:
            container.logConsole.Print(0, 1, "Event Log:");
            for (int i = 0; i < GAME_WIDTH; i++)
            {
                if (i == GRID_CONSOLE_WIDTH)
                {
                    // infoConsole/logConsole intersection:
                    container.logConsole.SetCellAppearance(i, 0, new ColoredGlyph(Palette.white, Palette.black, 202));
                }
                else
                {
                    container.logConsole.SetCellAppearance(i, 0, new ColoredGlyph(Palette.white, Palette.black, 205));
                }
            }
        }
    }
}
