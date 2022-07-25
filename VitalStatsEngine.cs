using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VitalStatsEngine
{
    public abstract class Stat
    {
        public delegate void OnValueChangedDelegate(Stat stat, float newvalue);

        public event OnValueChangedDelegate OnValueChanged;

        public Stat(string name)
        {
            this.name = name;
        }

        public Stat(string name, float startValue)
        {
            this.name = name;
            value = startValue;
        }

        private string name;
        public string Name { get { return name; } }

        protected float value;

        public abstract float Value
        {
            get;
            set;
        }

        protected List<StatRule> statRules = new List<StatRule>();

        protected void RaiseOnValueChanged()
        {
            OnValueChanged?.Invoke(this, value);
        }

        public void AddStatRuleAndStart(StatRule newrule)
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

    public class RangedStat : Stat
    {
        public RangedStat(string name, float maxvalue) : base(name)
        {
            maxValue = maxvalue;
        }

        private float maxValue;
        public float MaxValue { get { return maxValue; } }

        public override float Value
        {
            get { return value; }
            set 
            {
                float preValue = this.value;
                this.value = Mathf.Clamp(value, 0, MaxValue);
                if (preValue != this.value)
                    RaiseOnValueChanged();
            }
        }

    }

    public class UnlimitedStat : Stat
    {
        public UnlimitedStat(string name) : base(name)
        {

        }

        public UnlimitedStat(string name, float startValue) : base(name, startValue)
        {

        }

        public override float Value 
        {
            get { return value; }
            set
            {
                float preValue = this.value;
                this.value = value;
                if (preValue != this.value)
                    RaiseOnValueChanged();
            }
        }
    }

    public abstract class StatRule
    {
        public StatRule(MonoBehaviour coroutineHandler)
        {
            this.coroutineHandler = coroutineHandler;
        }

        protected MonoBehaviour coroutineHandler;

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

        private bool isWorking;

        public bool IsWorking { get { return isWorking; } }

        public void Start()
        {
            isWorking = true;
            OnStart();
        }

        protected abstract void OnStart();

        public void Stop()
        {
            isWorking = false;
            OnStop();
        }

        protected abstract void OnStop();
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
        public SimpleStatRule(float newtimestep, float newvalueperstep, MonoBehaviour coroutineHandler) : base(coroutineHandler)
        {
            timestep = newtimestep;
            ValueperStep = newvalueperstep;
        }
        protected override void OnStart()
        {
            coroutineHandler.StartCoroutine(coroutine());
        }

        private IEnumerator coroutine()
        {
            yield return new WaitForSecondsRealtime(timestep);
            if (stat != null)
            {
                if (IsWorking)
                {
                    stat.Value += ValueperStep;
                    coroutineHandler.StartCoroutine(coroutine());
                }
            }
            else
            {
                Stop();
            }
        }

        protected override void OnStop()
        {
            
        }
    }
}
