using UnityEngine;

public class PlayerAttackAndWeaponManager : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    public Collider weaponCollider;
    public Transform lockOnTransform;
    public bool isLockedOn;
    public CharacterManager currentTarget;
    public bool isHealing;
    [SerializeField] private float damage;
    [SerializeField] private float poiseDamage;
    [SerializeField] private bool isParrying;
    [SerializeField] private ParticleSystem imNotGoingToSugercoatIt;
    private ParticleSystem parryInstance;
    private void Awake()
    {
        
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();

    }
    public void HandleAttack()
    {
        if (playerManager.currentStamina < playerManager.attackStaminaConsumption) return;
        animatorManager.animator.SetInteger(("actionWanted"), 1);
        if (!playerManager.isInteracting && !playerManager.isUsingRootMotion)
            animatorManager.PlayTargetAnimation("LightAttack", true, true, 1);
    }
    public void TakeDamage(float enemyDamage, float enemyPoiseDamage, bool fury, bool grab, CharacterManager enemy)
    {
        if (isParrying && !grab)
        {
            if (fury) enemyPoiseDamage *= 2;
            enemy.TakeDamage(0, enemyPoiseDamage);
            parryInstance = Instantiate(imNotGoingToSugercoatIt, transform.position, Quaternion.LookRotation(transform.forward));
            playerManager.deflectSfx.Play();
        }
        else if (playerManager.isBlocking && playerManager.currentStamina > enemyPoiseDamage / 2 && !grab && !fury)
        {
            playerManager.currentStamina -= enemyPoiseDamage / 2;
            playerManager.recoverySpentTime = 0;
            playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
            playerManager.blockSfx.Play();
        }
        else
        {
            // the is blocking is stated to false incase the player is hit without enough stamina
            playerManager.isBlocking = false;
            playerManager.currentHealth -= enemyDamage;
            playerManager.currentStamina -= enemyPoiseDamage;
            playerManager.recoverySpentTime = 0;
            playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
            animatorManager.PlayTargetAnimation("Hit", true);
            playerManager.hitSfx.Play();
        }
        Vector3 playerVelocity = playerManager.playerLocomotion.playerRigidbody.linearVelocity;
        playerVelocity.x = 0;
        playerVelocity.z = 0;
        playerManager.playerLocomotion.playerRigidbody.linearVelocity = playerVelocity;


        playerManager.healthBar.UpdateHealthBar(playerManager.maxHealth, playerManager.currentHealth);
    }
    public void HandleBlock()
    {
        if ((!playerManager.isInteracting || animatorManager.animator.GetInteger("actionWanted") == 1) && playerManager.playerLocomotion.isGrounded)
        {
            if (animatorManager.animator.GetInteger("actionWanted") != 3)
            {
                animatorManager.PlayTargetAnimation("ParryIntoBlock", false, false, 3);
            }
            else 
            {
                animatorManager.animator.SetInteger(("actionWanted"), 3);
            }
        }
    }
    public void EnableWeaponCollider(string properties)
    {
        string[] values = properties.Split(';');
        damage = float.Parse(values[0]);
        poiseDamage = float.Parse(values[1]);
        weaponCollider.enabled = true;
        animatorManager.animator.SetInteger(("actionWanted"), 0);
        playerManager.currentStamina -= playerManager.attackStaminaConsumption;
        playerManager.recoverySpentTime = 0;
        playerManager.staminaBar.UpdatePostureBar(playerManager.maxStamina, playerManager.currentStamina);
    }
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
        if (animatorManager.animator.GetInteger("actionWanted") != 1) animatorManager.animator.SetBool(("isInteracting"), false);
    }

    public void SwingSound()
    {
        playerManager.attackSfx.Play();
    }

    public void EnableDodgeCollider()
    {
        playerManager.hitboxCollider.enabled = true;
    }
    public void DisableDodgeCollider()
    {
        playerManager.hitboxCollider.enabled = false;
    }
    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (newTarget != null)
        {
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
        }
        playerManager.cameraManager.SetLockCameraHeight();
    }

    public void EnableParry()
    {
        isParrying = true;
    }
    public void DisableParry()
    {
        isParrying = false;
        animatorManager.animator.SetFloat(("parryRepeat"), animatorManager.animator.GetFloat("parryRepeat") + .2f);
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            CharacterManager enemy = other.GetComponent<CharacterManager>();
            enemy.TakeDamage(damage, poiseDamage);
        }
    }

}
