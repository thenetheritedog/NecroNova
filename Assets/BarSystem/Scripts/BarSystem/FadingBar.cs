using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Botaemic.Unity.BarSystem
{
    public class FadingBar : Bar
    {
        [SerializeField]
        private Image foreground = null;

        [SerializeField]
        private Image background = null;

        [SerializeField]
        private Text barText = null;

        [SerializeField]
        private float updateSpeedInSeconds = 0.1f;

        private float currentBackgroundValue = 1f;

        public override void Initialize(Stat newStat)
        {
            base.Initialize(newStat);
            if (barText != null) { barText.text = FormatString(); }
        }

        protected override void UpdateBar()
        {
            if (stat != null)
            {
                foreground.fillAmount = stat.ValuePercentage;
                if (barText != null) { barText.text = FormatString(); }
            }
        }


        private void LateUpdate()
        {
            if (stat.CurrentValue <= Mathf.Epsilon)
            {
                barText.text = stat.CurrentValue.ToString("-") + "/" + stat.MaxValue.ToString("#.");
                return;
            }
            if (background.fillAmount > foreground.fillAmount)
            {
                currentBackgroundValue -= Time.deltaTime * updateSpeedInSeconds;
                background.fillAmount = currentBackgroundValue;
            }

            if (background.fillAmount < foreground.fillAmount)
            {
                currentBackgroundValue = foreground.fillAmount;
                background.fillAmount = foreground.fillAmount;
            }

        }

        private string FormatString()
        {
            return stat.CurrentValue.ToString("#.") + "/" + stat.MaxValue.ToString("#.");
        }
    }
}