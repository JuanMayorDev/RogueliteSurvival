using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    public Transform[] patrolPoints;      // Waypoints de patrullaje
    private int currentPatrolIndex = 0;
    private NavMeshAgent agent;

    public float detectionRange = 10f;      // Rango para detectar al jugador
    public float attackRange = 2f;          // Rango para iniciar el ataque
    public float chaseDistanceLimit = 15f;  // Si el jugador está más lejos, el enemigo abandona la persecución

    [Header("Attack Settings")]
    public float attackCooldown = 1.0f;     // Tiempo entre ataques
    private float attackTimer = 0f;

    public enum EnemyState { Patrolling, Chasing, Attacking }
    public EnemyState currentState = EnemyState.Patrolling;

    private Transform playerTransform;
    private IHealthController healthController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Se asume que el enemigo tiene un HealthController en el mismo GameObject
        healthController = GetComponent<HealthController>();

        // Se busca el jugador por su etiqueta "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
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

        // Si el jugador se aleja demasiado, vuelve a patrullar
        if (distanceToPlayer > chaseDistanceLimit)
        {
            currentState = EnemyState.Patrolling;
            agent.destination = patrolPoints[currentPatrolIndex].position;
            return;
        }

        // Si el jugador está lo suficientemente cerca, cambia a ataque
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

        // Hacer que el enemigo mire hacia el jugador
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            // Lógica de ataque (aquí podrías aplicar daño al jugador o reproducir una animación)
            Debug.Log("Enemy attacks the player!");
            attackTimer = 0f;
        }

        // Si el jugador se aleja, vuelve al estado de persecución
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chasing;
            agent.isStopped = false;
            agent.destination = playerTransform.position;
        }
    }

    // Método para aplicar daño al enemigo al recibir un ataque (por ejemplo, cuando colisiona con la espada del jugador)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {
            // Se aplica un daño fijo; puedes ajustar este valor según tus necesidades.
            float damage = 20f;
            healthController.TakeDamage(damage);
        }
    }
}
