using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private enum BehaviorState
    {
        wondering,
        chasing,
        attacking
    }

    private BehaviorState currentState;
    [SerializeField] float chaseDistance = 10f;
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float randomSelectionRadius = 4f;
    [SerializeField] float minPatrolPauseTime = 0f;
    [SerializeField] float maxPatrolPauseTime = 5f;
    private NavMeshAgent meshAgent;
    private bool isLingering;
    private Vector3 nextPosition;
    private GameObject playerRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = FindFirstObjectByType<PlayerController>().gameObject;
        meshAgent = GetComponent<NavMeshAgent>();
        meshAgent.stoppingDistance = attackDistance;
        isLingering = false;
    }

    // Update is called once per frame
    void Update()
    {
        StateHandler();
    }

    private IEnumerator SetRandomNavMeshLocation()
    {
        //Generates a random point in the navmesh surface area that the enemy will navigate to.
        isLingering = true;

        Vector3 randomPoint = Random.insideUnitSphere * randomSelectionRadius;
        randomPoint += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomPoint, out hit, randomSelectionRadius, 1))
        {
            finalPosition = hit.position;
        }
        yield return new WaitForSeconds(Random.Range(minPatrolPauseTime + 0.5f, maxPatrolPauseTime));
        meshAgent.SetDestination(finalPosition);
        isLingering = false;
    }

    void StateHandler()
    {
        //Checks the current scenes context to understand what kind of behavior to switch to.
        float playerDist = Vector3.Distance(transform.position, playerRef.transform.position);

        if (playerDist <= attackDistance)
        {
            currentState = BehaviorState.attacking;
            Debug.Log("IM ATTACKING THE PLAYER ARE YA PROUD DAD!?!?!?!");
        }
        else if (playerDist < chaseDistance)
        {
            currentState = BehaviorState.chasing;
            meshAgent.SetDestination(playerRef.transform.position);
            Debug.Log("Current Behavior state Chasing");
        }
        else if (playerDist > chaseDistance)
        {
            currentState = BehaviorState.wondering;
            Debug.Log("Current Behavior state Wondering");
        }

        if (currentState == BehaviorState.wondering && Vector3.Distance(transform.position, meshAgent.destination) <= meshAgent.stoppingDistance && !isLingering)
        {
            StartCoroutine(SetRandomNavMeshLocation());
            Debug.Log("setting random Destination");
        }

    }

}
