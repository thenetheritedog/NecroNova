using Botaemic.Unity.BarSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleBar : Bar
{
    [SerializeField]
    private Transform bar = default(Transform);

    protected override void UpdateBar()
    {
        if (bar != null)
        {
            bar.localScale = new Vector3(stat.ValuePercentage, 1);
        }
    }
}
