using UnityEngine;

public class Boss1Manager : CharacterManager, IDataPersistence
{
    public Collider swordCollider;
    public Collider shieldCollider;
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public void LoadData(GameData data)
    {
        data.bossKilled.TryGetValue(id, out isDead);
        if (isDead) 
        {
            Destroy(gameObject);
        }
    }
    public void SaveData(ref GameData data)
    {
        if (data.bossKilled.ContainsKey(id))
        {
            data.bossKilled.Remove(id);
        }
        data.bossKilled.Add(id, isDead);
    }
    public override void AttackBehavior(string State)
    {
        if (State == "CloseRange")
        {
            float randomAttack = Random.Range(0f, 1f);
            switch (randomAttack)
            {
                case (<= 0.50f) and (> 0.25f):
                    PlayTargetAnimation("Attack", true);
                    timeAfterAttack = 0;
                    isAttacking = true;
                    break;
                case (> 0.50f) and (<= 0.75f):
                    PlayTargetAnimation("HeavyAttack", true);
                    timeAfterAttack = 0;
                    break;
                case (> 0.75f):
                    PlayTargetAnimation("Attack2", true);
                    timeAfterAttack = 0;
                    isAttacking = true;
                    break;
                default:
                    timeAfterAttack -= 0.5f;
                    break;
            }
        }
        if (State == "LongRange") 
        {
            distanceRootMult = Vector3.Distance(player.transform.position, transform.position);
            PlayTargetAnimation("Dash_Attack", true);
            timeAfterAttack = 0;
            isAttacking = true;
        }

    }
    public override void EnableWeaponCollider(string properties)
    {
        string[] values = properties.Split(';');
        string weapon = values[0];
        damage = float.Parse(values[1]);
        poiseDamage = float.Parse(values[2]);
        fury = bool.Parse(values[3]);
        grab = bool.Parse(values[4]);



        if (weapon == "sword")
            swordCollider.enabled = true;
        if (weapon == "shield")
            shieldCollider.enabled = true;
        isAttacking = true;
        Debug.Log("it attacked");
    }
    public override void DisableWeaponCollider(string weapon = "all")
    {
        if (weapon == "sword")
            swordCollider.enabled = false;
        if (weapon == "shield")
            shieldCollider.enabled = false;
        if (weapon == "all")
        {
            swordCollider.enabled = false;
            shieldCollider.enabled = false;
        }
        isAttacking = false;
        fury = false;
        grab = false;
        Debug.Log("it disabled");
    }
    public override void TakeDamage(float damage, float poiseDamage)
    {
        currentPoise -= poiseDamage;
        if (currentPoise < 0) { currentPoise = maxPoise; currentHealth += -50; }
        poiseBar.UpdatePostureBar(maxPoise, currentPoise);
        Debug.Log(currentPoise);
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            isDead = true;
            hitParticlesInstance = Instantiate(hitParticles, transform.position, Quaternion.identity);
            DataPersistenceManager.instance.SaveGame();
            Destroy(gameObject, 1);

        }
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
        if (damage > 0)
        {
            hitSfx.Play();
            hitParticlesInstance = Instantiate(hitParticles, transform.position, Quaternion.identity);
        }
        Debug.Log(currentHealth);
    }
}
