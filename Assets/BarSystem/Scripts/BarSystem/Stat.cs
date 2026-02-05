using System;

namespace Botaemic.Unity.BarSystem
{
    public class Stat
    {
        private float currentValue = 10f;
        private float maxValue = 10f;

        public float ValuePercentage { get => (currentValue / maxValue); }
        public float CurrentValue { get => currentValue; private set => currentValue = value; }
        public float MaxValue { get => maxValue; private set => maxValue = value; }

        public event Action OnHealthChange;

        public Stat(float maximumValue)
        {
            this.maxValue = maximumValue;
            currentValue = maximumValue;

        }

        public void RemovePoints(float amount)
        {

            currentValue -= amount;

            if (currentValue < 0) { CurrentValue = 0; }

            TriggerActions();
        }

        public void AddPoints(float amount)
        {
            currentValue += amount;
            if (currentValue > maxValue) { currentValue = maxValue; }

            TriggerActions();
        }

        private void TriggerActions()
        {
            OnHealthChange?.Invoke();
        }
    }
}
