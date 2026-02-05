using Botaemic.Unity.BarSystem;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    private float maximumHealthpoints = 10f;

    [SerializeField]
    private float maximumManapoints = 10f;
    [SerializeField]
    private float manaRechargeRate = 1f;

    [SerializeField]
    private Bar healthBar = null;
    [SerializeField]
    private Bar manaBar = null;


    private Stat health = null;
    private Stat mana = null;
    private Stat shield = null;


    private void Awake()
    {
        health = new Stat(maximumHealthpoints);
        mana = new Stat(maximumManapoints);

        if (healthBar != null) { healthBar.Initialize(health); }
        if (manaBar != null) { manaBar.Initialize(mana); }
    }

    void Update()
    {
        if (health.CurrentValue < Mathf.Epsilon)
        {
            Destroy(gameObject);
        }

        RechargeMana();

    }

#region Button Handlers

    public void OnDamagePressed(float amount)
    {
        health.RemovePoints(amount);
    }

    public void OnHealPressed(float amount)
    {
        health.AddPoints(amount);
    } 


    public void OnUseManaPressed(float amount)
    {
        mana.RemovePoints(amount);
    }

#endregion

    private void RechargeMana()
    {
        mana.AddPoints(manaRechargeRate * Time.deltaTime);
    }


    //public void RechargeShield(float amount)
    //{
    //    healthSystem.RechargeShield(amount);
    //}

}
