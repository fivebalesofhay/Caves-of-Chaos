using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.GridScripts.GridManager;

namespace Caves_of_Chaos.CreatureScripts
{
    public static class CreatureManager
    {
        public static void UpdateCreatures()
        {
            for (int i = activeGrid.creatures.Count() - 1; i >= 0; i--)
            {
                Creature creature = activeGrid.creatures[i];
                creature.actionPoints++;
                creature.Update();
                if (creature == PlayerManager.player) continue;
                if (creature.actionPoints >= 0) {
                    creature.actionPoints -= creature.Act();
                }
            }
        }
    }
}
