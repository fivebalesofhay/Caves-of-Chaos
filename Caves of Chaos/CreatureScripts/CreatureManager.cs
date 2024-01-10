using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Caves_of_Chaos.GridScripts.GridManager;

namespace Caves_of_Chaos.CreatureScripts
{
    public static class CreatureManager
    {
        public static List<CreatureTemplate> templates = new List<CreatureTemplate>();

        public static void Init()
        {
            // Load creature templates:
            String[] raws = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Creatures");
            for (int i = 0; i < raws.Length; i++)
            {
                String text = File.ReadAllText(raws[i]);
                CreatureTemplate? template = JsonSerializer.Deserialize<CreatureTemplate>(text);
                if (template == null)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid creature template");
                    continue;
                }
                templates.Add(template);
            }
        }

        public static void UpdateCreatures()
        {
            for (int i = PlayerManager.player.grid.creatures.Count() - 1; i >= 0; i--)
            {
                Creature creature = PlayerManager.player.grid.creatures[i];
                creature.actionPoints++;
                creature.Update();
                if (creature == PlayerManager.player) continue;
                if (creature.actionPoints >= 0) {
                    creature.actionPoints = 0;
                    creature.actionPoints -= creature.Act();
                }
            }
            
            if (PlayerManager.player.grid.depth > 0)
            {
                for (int i = grids[PlayerManager.player.grid.depth-1].creatures.Count() - 1; i >= 0; i--)
                {
                    Creature creature = grids[PlayerManager.player.grid.depth-1].creatures[i];
                    creature.actionPoints++;
                    creature.Update();
                    if (creature == PlayerManager.player) continue;
                    if (creature.actionPoints >= 0)
                    {
                        creature.actionPoints = 0;
                        creature.actionPoints -= creature.Act();
                    }
                }
            }

            if (PlayerManager.player.grid.depth < grids.Count-1)
            {
                for (int i = grids[PlayerManager.player.grid.depth + 1].creatures.Count() - 1; i >= 0; i--)
                {
                    Creature creature = grids[PlayerManager.player.grid.depth + 1].creatures[i];
                    creature.actionPoints++;
                    creature.Update();
                    if (creature == PlayerManager.player) continue;
                    if (creature.actionPoints >= 0)
                    {
                        creature.actionPoints = 0;
                        creature.actionPoints -= creature.Act();
                    }
                }
            }
        }
    }
}
