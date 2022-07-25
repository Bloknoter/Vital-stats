using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VitalStatsEngine
{
    public class EntityStats : MonoBehaviour
    {
        private List<Stat> stats = new List<Stat>();

        public Stat[] Stats { get { return stats.ToArray(); } }

        public Stat AddRangedStat(string Name)
        {
            stats.Add(new RangedStat(Name, 100));
            return stats[stats.Count - 1];
        }

        public Stat AddRangedStat(string Name, float maxvalue)
        {
            stats.Add(new RangedStat(Name, maxvalue));
            return stats[stats.Count - 1];
        }

        public Stat AddRangedStat(string Name, float startvalue, float maxvalue)
        {
            stats.Add(new RangedStat(Name, maxvalue));
            stats[stats.Count - 1].Value = startvalue;
            return stats[stats.Count - 1];
        }

        public void AddStat(Stat stat)
        {
            if(stat != null)
            {
                stats.Add(stat);
            }
        }

        public Stat GetStat(string Name)
        {
            foreach (var i in stats)
            {
                if (i.Name == Name)
                    return i;
            }
            throw new System.Exception($"Stat '{Name}' does not exist");
        }

        public void ApplySimpleStatRule(Stat stat, float timeStep, float deltaValue)
        {
            SimpleStatRule statRule = new SimpleStatRule(timeStep, deltaValue, this);
            stat.AddStatRuleAndStart(statRule);
        }

        public int Count
        {
            get
            {
                return stats.Count;
            }
        }
    }
}
