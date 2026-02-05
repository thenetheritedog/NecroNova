using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class FindPath : MonoBehaviour
{
    [SerializeField] PlayerManager player;
    [SerializeField] private NavMeshAgent agent;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    [SerializeField] private LayerMask Default, Player;

    [SerializeField] private float sightRange, attackRange;

    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange;
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    
    private void Awake()
    {
        
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);

        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (playerInSightRange && playerInAttackRange) AttackPlayer();
        else Patroling();

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
        Debug.Log(walkPointSet);
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.transform.position);

        transform.LookAt(player.transform.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player.transform.position);
    }
}
