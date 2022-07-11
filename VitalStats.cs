using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Coroutines;


namespace VitalStatsEngine
{
    public class VitalStats
    {
        public List<Stat> stats { get; private set; } = new List<Stat>();

        public Stat AddStat(string Name)
        {
            stats.Add(new Stat(Name, 100));
            return stats[stats.Count - 1];
        }

        public Stat AddStat(string Name, float maxvalue)
        {
            stats.Add(new Stat(Name, maxvalue));
            return stats[stats.Count - 1];
        }

        public Stat AddStat(string Name, float startvalue, float maxvalue)
        {
            stats.Add(new Stat(Name, maxvalue));
            stats[stats.Count - 1].Value = startvalue;
            return stats[stats.Count - 1];
        }

        public Stat GetStat(string Name)
        {
            foreach (var i in stats)
            {
                if (i.Name == Name)
                    return i;
            }
            throw new System.Exception($"Indicator '{Name}' does not exist");
        }

        public int Count
        {
            get
            {
                return stats.Count;
            }
        }

    }

    public class Stat
    {
        public delegate void OnValueChangedDelegate(Stat stat, float newvalue);

        public event OnValueChangedDelegate OnValueChangedEvent;

        public Stat(string newName, float maxvalue)
        {
            Name = newName;
            MAX_VALUE = maxvalue;
        }

        public string Name { get; private set; }
        public float MAX_VALUE { get; private set; }
        private float value;
        public float Value
        {
            get { return value; }
            set 
            {
                float preValue = this.value;
                this.value = Mathf.Clamp(value, 0, MAX_VALUE);
                if (preValue != this.value)
                    OnValueChangedEvent?.Invoke(this, this.value);
            }
        }

        private List<StatRule> statRules = new List<StatRule>();
        public void AddStatRule(StatRule newrule)
        {
            statRules.Add(newrule);
            newrule.Stat = this;
            newrule.Start();
        }

        public void RemoveStatRule(StatRule rule)
        {
            if (rule != null)
            {
                rule.Stop();
                statRules.Remove(rule);
            }
        }
    }

    public abstract class StatRule : ICoroutineable
    {
        protected Stat stat;
        public Stat Stat
        {
            get { return stat; } 
            set
            {
                if (value != null)
                    stat = value;
            }
        }
        protected bool isWorking;
        public abstract void Start();
        public abstract void Continue();
        public abstract void Stop();
    }

    public class SimpleStatRule : StatRule
    {
        private float timestep;
        public float TimeStep
        {
            get { return timestep; }
            set { if (value > 0) { timestep = value; } }
        }
        public float ValueperStep;
        public SimpleStatRule(float newtimestep, float newvalueperstep)
        {
            timestep = newtimestep;
            ValueperStep = newvalueperstep;
        }
        public override void Start()
        {
            isWorking = true;
            CoroutineMaster.WaitStatic(this, timestep);
        }
        public override void Continue()
        {
            if (stat != null && isWorking)
            {
                stat.Value += ValueperStep;
                Start();
            }
            else
            {
                isWorking = false;
                Stop();
            }
        }

        public override void Stop()
        {
            isWorking = false;
        }
    }
}
