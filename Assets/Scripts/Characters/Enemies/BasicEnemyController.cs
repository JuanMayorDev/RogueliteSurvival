using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    public Transform[] patrolPoints;      // Patrol waypoints
    private int currentPatrolIndex = 0;
    private NavMeshAgent agent;

    public float detectionRange = 10f;      // Range to detect the player
    public float attackRange = 2f;          // Range to initiate the attack
    public float chaseDistanceLimit = 15f;  // If the player is further away, the enemy stops chasing

    [Header("Attack Settings")]
    public float attackCooldown = 1.0f;    // Time between attacks
    private float attackTimer = 0f;
    private HealthController playerHealthController;

    public enum EnemyState { Patrolling, Chasing, Attacking }
    public EnemyState currentState = EnemyState.Patrolling;

    private Transform playerTransform;
    private IHealthController healthController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Assumes the enemy has a HealthController on the same GameObject
        healthController = GetComponent<HealthController>();

        // Finds the player by its "Player" tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        //Take the component HealthController of the player
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealthController = playerObj.GetComponent<HealthController>();
            if (playerHealthController == null)
            {
                Debug.LogWarning("Player no tiene un componente que implemente IHealthController");
            }
        }

        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[currentPatrolIndex].position;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                LookForPlayer();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[currentPatrolIndex].position;
        }
    }

    void LookForPlayer()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
    }

    void Chase()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // If the player moves too far away, the enemy goes back to patrolling
        if (distanceToPlayer > chaseDistanceLimit)
        {
            currentState = EnemyState.Patrolling;
            agent.destination = patrolPoints[currentPatrolIndex].position;
            return;
        }

        // If the player is close enough, switch to attack
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attacking;
            agent.isStopped = true;
            attackTimer = 0f;
            return;
        }

        agent.isStopped = false;
        agent.destination = playerTransform.position;
    }

    void Attack()
    {
        if (playerTransform == null)
            return;

        // Make the enemy look at the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            // Attack logic (here you could apply damage to the player or play an animation)
            Debug.Log("Enemy attacks the player!");
            if (playerHealthController != null)
            {
                float damageAmount = 20f;
                playerHealthController.TakeDamage(damageAmount);
            }

            attackTimer = 0f;
        }

        // If the player moves away, return to chasing state
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chasing;
            agent.isStopped = false;
            agent.destination = playerTransform.position;
        }
    }

    // Método para aplicar daño al enemigo al recibir un ataque (por ejemplo, cuando colisiona con la espada del jugador)
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("PlayerWeapon"))
    //    {
    //        Debug.Log("golpie con la espada");
    //        Se aplica un daño fijo; puedes ajustar este valor según tus necesidades.
    //        float damage = 20f;
    //        healthController.TakeDamage(damage);
    //    }
    //}
}
