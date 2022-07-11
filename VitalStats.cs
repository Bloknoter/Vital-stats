using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VitalStatsEngine
{
    public class VitalStats
    {
        public List<Indicator> indicators { get; private set; } = new List<Indicator>();

        public Indicator AddStat(string Name)
        {
            indicators.Add(new Indicator(Name, 100));
            return indicators[indicators.Count - 1];
        }

        public Indicator AddStat(string Name, int maxvalue)
        {
            indicators.Add(new Indicator(Name, maxvalue));
            return indicators[indicators.Count - 1];
        }

        public Indicator AddStat(string Name, int startvalue, int maxvalue)
        {
            indicators.Add(new Indicator(Name, maxvalue));
            indicators[indicators.Count - 1].Value = startvalue;
            return indicators[indicators.Count - 1];
        }

        public Indicator GetIndicator(string Name)
        {
            foreach (var i in indicators)
            {
                if (i.Name == Name)
                    return i;
            }
            return null;
        }

        public VitalStats()
        {
            //Indicator health = AddStat("health", 100);
        }

        public int Count
        {
            get
            {
                return indicators.Count;
            }
        }

    }

    public class Indicator
    {
        public delegate void OnValueChangedDelegate(Indicator indicator, int newvalue);

        public event OnValueChangedDelegate OnValueChangedEvent;

        public Indicator(string newName, int maxvalue)
        {
            Name = newName;
            MAX_VALUE = maxvalue;
        }

        public string Name { get; private set; }
        public int MAX_VALUE { get; private set; }
        private int value;
        public int Value
        {
            get { return value; }
            set { this.value = Mathf.Clamp(value, 0, MAX_VALUE); OnValueChangedEvent?.Invoke(this, this.value); }
        }

        private List<StatRule> statRules = new List<StatRule>();
        public void AddStatRule(StatRule newrule)
        {
            statRules.Add(newrule);
            newrule.Indicator = this;
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
        protected Indicator indicator;
        public Indicator Indicator
        {
            get { return indicator; } 
            set
            {
                if (value != null)
                    indicator = value;
            }
        }
        protected bool isWorking;
        public abstract void Start();
        public abstract void Continue();
        public abstract void Stop();
    }

    public class SimpleStatRule : StatRule
    {
        private int timestep;
        public int TimeStep
        {
            get { return timestep; }
            set { if (value > 0) { timestep = value; } }
        }
        public int ValueperStep;
        public SimpleStatRule(int newtimestep, int newvalueperstep)
        {
            timestep = newtimestep;
            ValueperStep = newvalueperstep;
        }
        public override void Start()
        {
            isWorking = true;
            CoroutineMaster.Wait(this, timestep);
        }
        public override void Continue()
        {
            if (indicator != null && isWorking)
            {
                indicator.Value += ValueperStep;
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
