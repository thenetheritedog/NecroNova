using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;

public class CharacterManager : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public HealthBar healthBar;
    public float currentPoise;
    public float maxPoise;
    public PostureBar poiseBar;
    public float recoverySpeed;
    public float recoveryTime;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    public ParticleSystem hitParticles;
    public ParticleSystem hitParticlesInstance;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    [SerializeField] private LayerMask Default, Player;

    [SerializeField] private float sightRange, attackRange;

    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange;
    public float timeBetweenAttacks;
    public float timeAfterAttack;
    public Transform lockOnTransform;
    public bool isDead = false;
    private float recoverySpentTime;
    [SerializeField] private bool isUsingRootMotion;
    public bool isAttacking;
    public Rigidbody bossRigidbody;
    public PlayerManager player;
    public float damage;
    public float poiseDamage;
    public bool grab;
    public bool fury;
    public float distanceRootMult;
    public AudioSource hitSfx;
    public AudioSource attackSfx;
    public AudioSource cinemaSfx;

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
        currentPoise = maxPoise;
        poiseBar.UpdatePostureBar(maxPoise, currentPoise);
        player = FindFirstObjectByType<PlayerManager>();
        agent.stoppingDistance = attackRange;
        
    }
    public virtual void TakeDamage(float damage, float poiseDamage)
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
    public void Update()
    {
        if (currentPoise != maxPoise)
        {
            recoverySpentTime += Time.deltaTime;
            if (recoverySpentTime > recoveryTime)
            {
                currentPoise += recoverySpeed;
                currentPoise = Mathf.Clamp(currentPoise, 0, maxPoise);
                poiseBar.UpdatePostureBar(maxPoise, currentPoise);
            }
        }
        else { recoverySpentTime = 0; }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);

        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (playerInSightRange && playerInAttackRange) AttackPlayer();
        else Patroling();
        if (!isAttacking)
        {
            timeAfterAttack += Time.deltaTime;
            DisableWeaponCollider();
        }
        else { timeAfterAttack = 0; }


    }
    private void LateUpdate()
    {
        if (isUsingRootMotion != animator.GetBool("isUsingRootMotion"))
        {
            agent.nextPosition = transform.position;
        }

        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        if (!isUsingRootMotion)
        {
            distanceRootMult = 1;
            agent.updatePosition = true;
            agent.updateRotation = true;
        }
        else
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            walkPoint = new Vector3
            (transform.position.x + UnityEngine.Random.Range(-walkPointRange, walkPointRange), transform.position.y, transform.position.z + UnityEngine.Random.Range(-walkPointRange, walkPointRange));
            if (Physics.Raycast(walkPoint, -transform.up, 2f, Default))
                walkPointSet = true;
        }
        else
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void ChasePlayer()
    {


        transform.LookAt(new Vector3(player.transform.position.x, 0, player.transform.position.z));

        if (player.playerAttackAndWeaponManager.isHealing && timeAfterAttack > timeBetweenAttacks)
        {
            AttackBehavior("LongRange");
        }
        else if (!isAttacking)
        {
            agent.SetDestination(player.transform.position);
            animator.SetFloat("nonAttackState", 1);
        }
    }

    private void AttackPlayer()
    {
        transform.LookAt(new Vector3(player.transform.position.x, 0, player.transform.position.z));
        animator.SetFloat("nonAttackState", 0);

        if (timeAfterAttack > timeBetweenAttacks)
        {
            AttackBehavior("CloseRange");

        }


    }
    public void PlayTargetAnimation(string targetAnimation, bool useRootMotion = false)
    {
        animator.SetBool("isUsingRootMotion", useRootMotion);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    private void OnAnimatorMove()
    {
        if (isUsingRootMotion)
        {


            bossRigidbody.linearDamping = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition * distanceRootMult / (Time.deltaTime) * 0.39f;
            if (distanceRootMult > 1)
            {
                velocity += player.playerLocomotion.playerRigidbody.linearVelocity * 1.5f;
            }

            bossRigidbody.linearVelocity = velocity;
        }
    }

    public virtual void EnableWeaponCollider(string properties)
    {
        
    }
    public virtual void DisableWeaponCollider(string weapon = "both")
    {
        
    }
    public void SwingSound()
    {
        attackSfx.Play();
    }
    private void OnTriggerEnter(Collider hitboxCollider)
    {
        if (hitboxCollider.CompareTag("Player Hitbox"))
        {
            PlayerAttackAndWeaponManager player = FindObjectOfType<PlayerAttackAndWeaponManager>();
            player.TakeDamage(damage, poiseDamage, fury, grab, gameObject.GetComponent<CharacterManager>());

        }
    }

    public virtual void AttackBehavior(string State)
    {
        
    }
}
