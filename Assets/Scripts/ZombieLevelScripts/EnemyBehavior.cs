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
    [SerializeField] float attackStrength = 15f;
    [SerializeField] float randomSelectionRadius = 4f;
    [SerializeField] float minPatrolPauseTime = 0f;
    [SerializeField] float maxPatrolPauseTime = 5f;
    [SerializeField] float maxHealth = 100f;

    public UnitHealth health;
    private NavMeshAgent meshAgent;
    private bool isLingering;
    private GameObject playerRef;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = FindFirstObjectByType<PlayerController>().gameObject;
        meshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        health = new UnitHealth(maxHealth);
        meshAgent.stoppingDistance = attackDistance;
        GetComponentInChildren<DamageTrigger>().damageAmount = attackStrength;
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
            Vector3 direction = playerRef.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = lookRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            anim.SetTrigger("Attacking");
            Debug.Log("IM ATTACKING THE PLAYER ARE YA PROUD DAD!?!?!?!");
        }
        else if (playerDist < chaseDistance)
        {
            currentState = BehaviorState.chasing;
            meshAgent.SetDestination(playerRef.transform.position);
            StopCoroutine(SetRandomNavMeshLocation());
            //Debug.Log("Current Behavior state Chasing");
        }
        else if (playerDist > chaseDistance)
        {
            currentState = BehaviorState.wondering;
            //Debug.Log("Current Behavior state Wondering");
        }

        if (currentState == BehaviorState.wondering && Vector3.Distance(transform.position, meshAgent.destination) <= meshAgent.stoppingDistance && !isLingering)
        {
            StartCoroutine(SetRandomNavMeshLocation());
            //Debug.Log("setting random Destination");
        }

        if (health.IsDead)
        {
            HandleDeath();
        }

    }

    void HandleDeath()
    {
        //If the enemy hp is 0 handle the death of the enemy.
        Debug.Log(gameObject.name + " is dead");
        Destroy(gameObject);
    }


    /*
    #region SmoothRotationToLookAtPlayer

    float xVel;
    float yVel;
    float zVel;
    Vector3 newRotation;

    private void SmoothRotationLook(Vector3 targetPos, Vector3 rotatingObjPosition)
    {
        Vector3 direction = targetPos - rotatingObjPosition;
        Vector3 targetRotation = Quaternion.LookRotation(direction).eulerAngles;

        newRotation = new Vector3(
            Mathf.SmoothDampAngle(newRotation.x, targetRotation.x, ref xVel, 1f),
            Mathf.SmoothDampAngle(newRotation.y, targetRotation.y, ref yVel, 1f),
            Mathf.SmoothDampAngle(newRotation.z, targetRotation.z, ref zVel, 1f)
        );

        transform.eulerAngles = newRotation;
    }

    #endregion
    */
}
