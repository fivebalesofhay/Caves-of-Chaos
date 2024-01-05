using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caves_of_Chaos.ConditionScripts.Conditions;

namespace Caves_of_Chaos.ConditionScripts
{
    public class Condition
    {
        public conditions condition;
        public int duration;

        public Condition(conditions condition, int duration)
        {
            this.condition = condition;
            this.duration = duration;
        }
    }
}
