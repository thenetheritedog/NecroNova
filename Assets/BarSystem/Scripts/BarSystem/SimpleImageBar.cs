using UnityEngine;
using UnityEngine.UI;

namespace Botaemic.Unity.BarSystem
{
    public class SimpleImageBar : Bar
    {
        [SerializeField]
        private Image bar = null;

        protected override void UpdateBar()
        {
            if (stat != null)
            {
                bar.fillAmount = stat.ValuePercentage;
            }
        }

    }
}
