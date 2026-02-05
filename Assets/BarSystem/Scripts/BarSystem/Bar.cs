using UnityEngine;

namespace Botaemic.Unity.BarSystem
{
    public abstract class Bar :  MonoBehaviour
    {
        protected Stat stat = null;

        public virtual void Initialize(Stat newStat)
        {
            this.stat = newStat;
            this.stat.OnHealthChange += UpdateBar;
        }

        protected abstract void UpdateBar();
    }
}
