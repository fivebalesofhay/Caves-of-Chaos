using System;
using System.Collections.Generic;
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
            for (int i = activeGrid.creatures.Count() - 1; i >= 0; i--)
            {
                Creature creature = activeGrid.creatures[i];
                creature.actionPoints++;
                creature.Update();
                if (creature == PlayerManager.player) continue;
                if (creature.actionPoints >= 0) {
                    creature.actionPoints = 0;
                    creature.actionPoints -= creature.Act();
                }
            }
        }
    }
}
