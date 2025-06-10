using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    private enum BehaviorState
    {
        patrolling,
        chasing,
    }

    private BehaviorState currentState;
    [SerializeField] float chaseDistance = 10f;
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float attackStrength = 15f;
    [SerializeField] float randomSelectionRadius = 4f;
    [SerializeField] float minPatrolPauseTime = 0f;
    [SerializeField] float maxPatrolPauseTime = 5f;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] UnityEvent onDamage;
    [SerializeField] Material damagedMat;
    [SerializeField] Material normalMat;
    [SerializeField] EnemyFOV fov;
    [SerializeField] float detectionRate = 15f;
    [SerializeField, Tooltip("The patrol pattern this enemy will naturally follow.")] private GameObject patrolPattern;
    [SerializeField] Image awarenessImage;

    private Transform[] patrolPoints;
    private Transform currentTargetPoint;
    private int patrolIndex = 0;

    public UnitHealth health;
    private NavMeshAgent meshAgent;
    private bool isLingering;
    private GameObject playerRef;
    private Animator anim;
    float detectionLvl;

    void Awake()
    {
        patrolPoints = new Transform[patrolPattern.transform.childCount];
        for (int i = 0; i < patrolPattern.transform.childCount; i++)
        {
            patrolPoints[i] = patrolPattern.transform.GetChild(i).transform;
        }
        currentTargetPoint = patrolPoints[patrolIndex];
        fov = GetComponent<EnemyFOV>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = GameManager.Instance.Player;
        meshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        onDamage.AddListener(ToggleEnemyMaterial);
        health = new UnitHealth(maxHealth, onDamage);
        meshAgent.stoppingDistance = attackDistance;
        GetComponentInChildren<DamageTrigger>().damageAmount = attackStrength;
        GetComponentInChildren<MeshRenderer>().material = normalMat;
        isLingering = false;
        meshAgent.SetDestination(patrolPoints[patrolIndex].position);
        StartCoroutine("EnemyDetectionWithDelay", 1f);
        currentState = BehaviorState.patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        StateHandler();
    }


    void StateHandler()
    {
        //Checks the current scenes context to understand what kind of behavior to switch to.
        float playerDist = Vector3.Distance(transform.position, playerRef.transform.position);

        if (isLingering)
        {
            meshAgent.isStopped = true;
        }
        else
        {
            meshAgent.isStopped = false;
        }

        if (playerDist <= attackDistance && currentState == BehaviorState.chasing)
        {
            Vector3 direction = playerRef.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = lookRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            anim.SetTrigger("Attacking");
            //Debug.Log("IM ATTACKING THE PLAYER ARE YA PROUD DAD!?!?!?!");
        }
        else if (currentState == BehaviorState.chasing)
        {
            Debug.Log("ChasingPlayer");
            meshAgent.SetDestination(playerRef.transform.position);
        }
        else if (currentState == BehaviorState.patrolling && Vector3.Distance(transform.position, currentTargetPoint.position) <= meshAgent.stoppingDistance)
        {
            //Debug.Log("Made it to the patrol point goint to the next");
            StartCoroutine("SetNextPatrolPoint");
        }

        //DepricatedCode
        /*
        if (currentState == BehaviorState.patrolling && Vector3.Distance(transform.position, meshAgent.destination) <= meshAgent.stoppingDistance && !isLingering)
        {
            StartCoroutine(SetRandomNavMeshLocation());
            //Debug.Log("setting random Destination");
        }
        */

        if (health.IsDead)
        {
            HandleDeath();
        }

    }

    void ToggleEnemyMaterial()
    {
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        Debug.Log("ToggleMatIsCalled " + meshRenderer.gameObject.name);
        if (meshRenderer.sharedMaterial == normalMat)
        {
            Debug.Log("Switching material to damaged");
            meshRenderer.material = damagedMat;
            Invoke("ToggleEnemyMaterial", 0.5f);
        }
        else if (meshRenderer.sharedMaterial == damagedMat)
        {
            Debug.Log("Switching material to normal");
            meshRenderer.material = normalMat;
        }
    }

    IEnumerator SetNextPatrolPoint()
    {
        isLingering = true;

        patrolIndex++;
        if (patrolIndex >= patrolPoints.Length)
        {
            //if it is the last patrol point go back to 0.
            patrolIndex = 0;
        }
        currentTargetPoint = patrolPoints[patrolIndex];
        meshAgent.SetDestination(patrolPoints[patrolIndex].position);

        yield return new WaitForSeconds(Random.Range(minPatrolPauseTime + 0.5f, maxPatrolPauseTime));
        isLingering = false;
    }

    IEnumerator EnemyDetectionWithDelay(float delay = 1f)
    {
        while (gameObject.activeInHierarchy)
        {
            switch (fov.isPlayerInSight)
            {
                case true:
                    if (playerRef.GetComponent<PlayerController>()._isCrouching)
                    {
                        detectionLvl += (detectionRate / 2) - Vector3.Distance(transform.position, playerRef.transform.position);
                    }
                    else
                    {
                        detectionLvl += detectionRate - Vector3.Distance(transform.position, playerRef.transform.position);
                    }
                    break;
                case false:

                    if (detectionLvl - detectionRate <= 0)
                    {
                        detectionLvl = 0;
                    }
                    else
                    {
                        detectionLvl -= detectionRate;
                    }
                    break;
            }

            awarenessImage.fillAmount = 100 / detectionLvl;

            if (detectionLvl >= 100)
            {
                detectionLvl = 100;
                Debug.Log("Player has been detected with a detectionLvl of : " + detectionLvl);
                currentState = BehaviorState.chasing;
            }
            else if (detectionLvl <= 0 && currentState == BehaviorState.chasing)
            {
                detectionLvl = 0;
                currentState = BehaviorState.patrolling;
                Debug.Log("Player has escaped returning to patrol");
            }

            yield return new WaitForSeconds(delay);
        }
    }

    //Deprecated Code
    /*
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
    */

    void HandleDeath()
    {
        //If the enemy hp is 0 handle the death of the enemy.
        Debug.Log(gameObject.name + " is dead");
        Destroy(gameObject);
    }

    //Deprecated Code
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
