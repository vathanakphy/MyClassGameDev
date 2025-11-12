using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Damageable))]
public class AnimalAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float roamRadius = 15f;
    public float waitTime = 3f;
    public float obstacleAvoidanceDistance = 3f;

    private NavMeshAgent agent;
    private Vector3 startPosition;
    private float nextMoveTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        PickNewDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && Time.time > nextMoveTime)
        {
            nextMoveTime = Time.time + waitTime;
            PickNewDestination();
        }

        AvoidObstacles();
    }

    void PickNewDestination()
    {
        Vector3 randomPos = startPosition + new Vector3(
            Random.Range(-roamRadius, roamRadius),
            0,
            Random.Range(-roamRadius, roamRadius)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void AvoidObstacles()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        if (Physics.Raycast(ray, obstacleAvoidanceDistance))
        {
            transform.Rotate(Vector3.up, Random.Range(90f, 180f));
            PickNewDestination();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            Damageable dmg = GetComponent<Damageable>();
            dmg.TakeDamage(25f);
        }
    }
}
