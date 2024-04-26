using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    // public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, hasFallen;

    Rigidbody rigidBody;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!hasFallen)
        {
            bool isMoving = agent.hasPath && agent.remainingDistance > agent.stoppingDistance;

            animator.SetBool("IsMoving", isMoving);
            //Check for sight and attack range
            playerInSightRange = CheckLineOfSight() && Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer) && playerInSightRange;

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
    }

    private bool CheckLineOfSight()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
        {
            if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Shield"))
            {
                return true; // Player or Shield is in sight
            }

        }
        return false; // No line of sight to the player
    }


    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        // Look at the player on the y-axis only
        Vector3 lookDirection = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookDirection);
        if (!alreadyAttacked && CheckLineOfSight())
        {
            // Attack Code, change for enemies with different attack types
            // Calculate the spawn position for the projectile a bit in front of the enemy and 1 unit higher
            Vector3 projectileSpawnPosition = transform.position + transform.forward * 1.0f + Vector3.up * 1.0f; // Add 1 unit higher along y-axis

            // Instantiate the projectile at the spawn position
            Rigidbody rb = Instantiate(projectile, projectileSpawnPosition, Quaternion.identity).GetComponent<Rigidbody>();

            // Add force to the projectile to shoot it forward
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            // rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void Fall()
    {
        hasFallen = true;
        if (animator != null)
        {
            animator.SetBool("IsFalling", true); // Trigger falling animation on the enemy
            agent.isStopped = true; // Stop the NavMeshAgent
            StartCoroutine(FreezeEnemyPosition(GetComponent<Rigidbody>(), 2f));
        }
    }
    public IEnumerator FreezeEnemyPosition(Rigidbody enemyRigidbody, float freezeDuration)
    {
        // Store the original constraints
        var originalConstraints = enemyRigidbody.constraints;

        // Freeze position
        enemyRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        // Wait for the specified duration
        yield return new WaitForSeconds(freezeDuration);

        // Unfreeze position and return to original constraints
        enemyRigidbody.constraints = originalConstraints;
        agent.isStopped = false; // Resume the NavMeshAgent

        if (animator != null)
        {
            animator.SetBool("IsFalling", false);
            hasFallen = false; // Enemy is no longer falling
        }
    }
}