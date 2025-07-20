using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    public enum BehaviorState
    {
        Patrolling,
        Chasing,
        Resting,
        Dead,
        Confused
    }

    public BehaviorState currentState;
    //[SerializeField] float chaseDistance = 10f;
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float attackStrength = 15f;
    [SerializeField] float chaseSpeed = 3f;
    [SerializeField] float investigationMovSpeed = 2f;
    [SerializeField] float patrolSpeed = 1f;
    //[SerializeField] float randomSelectionRadius = 4f;
    [SerializeField] float minPatrolPauseTime = 0f;
    [SerializeField] float maxPatrolPauseTime = 5f;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] UnityEvent onDamage;
    [SerializeField] Material damagedMat;
    [SerializeField] Material normalMat;
    [SerializeField] EnemyFOV fov;
    [SerializeField] float detectionRate = 15f;
    [SerializeField, Tooltip("The patrol pattern this enemy will naturally follow.")] private GameObject patrolPattern;
    [SerializeField] Light leftEyeLight, rightEyeLight;

    [Header("SFX Names")]
    [SerializeField] private string hurtSFX;
    [SerializeField] private string deathSFX;
    [SerializeField] private string breathingSFX;

    private const string CryingSFX = "Crying";

    private Transform[] patrolPoints;
    private Transform currentTargetPoint;
    private Transform currentPatrolPoint;
    private int patrolIndex = 0;

    public UnitHealth health;
    private NavMeshAgent meshAgent;
    private CapsuleCollider capsuleCollider;
    private GameObject playerRef;
    private Animator anim;
    private SoundComponent soundComponent;

    public UnityEvent onDeathExternal;

    void Awake()
    {
        if (patrolPattern != null)
        {
            patrolPoints = new Transform[patrolPattern.transform.childCount];
            for (int i = 0; i < patrolPattern.transform.childCount; i++)
            {
                patrolPoints[i] = patrolPattern.transform.GetChild(i).transform;
            }
            currentTargetPoint = patrolPoints[patrolIndex];
            currentPatrolPoint = patrolPoints[patrolIndex];
            fov = GetComponent<EnemyFOV>();
        }
        else
        {
            currentTargetPoint = transform;
            currentPatrolPoint = transform;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GetComponentInChildren<MeshRenderer>().material = normalMat;
        //onDamage.AddListener(ToggleEnemyMaterial);
        playerRef = GameManager.Instance.Player;
        meshAgent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<Animator>();
        soundComponent = GetComponent<SoundComponent>();
        onDamage.AddListener(StartAttackStunPause);
        health = new UnitHealth(maxHealth, onDamage);
        GetComponentInChildren<DamageTrigger>().damageAmount = attackStrength;
        meshAgent.speed = patrolSpeed;
        meshAgent.stoppingDistance = attackDistance - .5f;
        if (patrolPattern != null)
        {
            meshAgent.SetDestination(patrolPoints[patrolIndex].position);
            meshAgent.isStopped = false;
        }
        else { meshAgent.isStopped = true; }

        soundComponent.PlaySFX("Growl");

        StartCoroutine("EnemyDetectionWithDelay", .1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy && !GameManager.Instance.IsGameOver)
        {
            StateHandler();
            //anim.SetBool("IsMoving", !meshAgent.isStopped);
        }
        else if (GameManager.Instance.IsGameOver && currentState != BehaviorState.Patrolling)
        {
            currentState = BehaviorState.Patrolling;
            StartCoroutine("SetNextPatrolPoint");
            meshAgent.speed = patrolSpeed;
            return;
        }
    }


    void StateHandler()
    {
        //Checks the current scenes context to understand what kind of behavior to switch to.
        float playerDist = Vector3.Distance(transform.position, playerRef.transform.position);

        Vector3 direction = playerRef.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = lookRotation.eulerAngles;

        switch (currentState)
        {
            case BehaviorState.Patrolling:
                if (leftEyeLight.enabled == true || leftEyeLight.color == Color.red)
                {
                    leftEyeLight.enabled = false;
                    rightEyeLight.enabled = false;
                    leftEyeLight.color = Color.yellow;
                    rightEyeLight.color = Color.yellow;
                }
                if (!meshAgent.isStopped)
                {
                    anim.SetBool("IsMoving", true);
                    anim.SetBool("IsChasing", false);
                    anim.SetBool("Attacking", false);
                }
                else
                {
                    anim.SetBool("IsMoving", false);
                }
                
                if (currentTargetPoint == currentPatrolPoint && Vector3.Distance(transform.position, currentTargetPoint.position) <= attackDistance && patrolPattern != null)
                {
                    anim.SetBool("IsMoving", false);
                    Debug.Log("Made it to the patrol point goint to the next");
                    StartCoroutine("SetNextPatrolPoint");
                    meshAgent.speed = patrolSpeed;
                }
                else if (meshAgent.destination == playerRef.transform.position)
                {
                    anim.SetBool("IsMoving", false);
                    anim.SetBool("Attacking", false);
                    StartCoroutine("SetNextPatrolPoint");
                    meshAgent.speed = patrolSpeed;
                }

                break;

            case BehaviorState.Chasing:

                if (playerRef.GetComponent<PlayerHealth>().IsDead)
                {
                    Debug.Log("Player is dead moving on");
                    anim.SetBool("IsMoving", false);
                    anim.SetBool("Attacking", false);
                    meshAgent.SetDestination(patrolPoints[patrolIndex].position);
                    meshAgent.speed = patrolSpeed;
                    currentState = BehaviorState.Patrolling;
                    return;
                }


                if (leftEyeLight.color != Color.red || !leftEyeLight.enabled)
                {
                    leftEyeLight.color = Color.red;
                    rightEyeLight.color = Color.red;
                    leftEyeLight.enabled = true;
                    rightEyeLight.enabled = true;
                }

                if (playerDist <= attackDistance && !health.IsDead && !playerRef.GetComponent<PlayerHealth>().IsDead)
                {
                    transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
                    anim.SetBool("IsMoving", false);
                    anim.SetBool("IsChasing", false);
                    anim.SetBool("Attacking", true);
                    break;
                }

                if (!health.IsDead)
                {
                    anim.SetBool("Attacking", false);
                    // Debug.Log("ChasingPlayer");
                    meshAgent.SetDestination(playerRef.transform.position);
                    meshAgent.speed = chaseSpeed;
                }

                if (!meshAgent.isStopped)
                {
                    anim.SetBool("IsMoving", false);
                    anim.SetBool("IsChasing", true);
                }
                else
                {
                    anim.SetBool("IsChasing", false);
                }

                break;
            case BehaviorState.Resting:

                // Debug.Log(gameObject.name + " Is Resting");
                anim.SetBool("IsResting", true);
                meshAgent.enabled = false;
                capsuleCollider.enabled = false;
                rightEyeLight.enabled = false;
                leftEyeLight.enabled = false;

                break;
            case BehaviorState.Confused:
                soundComponent.PlaySFX(CryingSFX);
                anim.SetBool("IsConfused", true);
                meshAgent.enabled = false;
                rightEyeLight.enabled = false;
                leftEyeLight.enabled = false;
                break;
            default:
                break;
        }

        if (health.IsDead)
        {
            Debug.Log("Dying");
            anim.SetTrigger("IsDead");
            meshAgent.isStopped = true;
            meshAgent.enabled = false;
            capsuleCollider.enabled = false;
        }
    }

    IEnumerator SetNextPatrolPoint()
    {
        meshAgent.isStopped = true;

        patrolIndex++;
        if (patrolIndex >= patrolPoints.Length)
        {
            //if it is the last patrol point go back to 0.
            patrolIndex = 0;
        }
        currentTargetPoint = patrolPoints[patrolIndex];
        currentPatrolPoint = patrolPoints[patrolIndex];
        meshAgent.SetDestination(patrolPoints[patrolIndex].position);

        yield return new WaitForSeconds(Random.Range(minPatrolPauseTime + 0.5f, maxPatrolPauseTime));
        meshAgent.isStopped = false;
    }

    IEnumerator EnemyDetectionWithDelay(float delay = .1f)
    {
        while (gameObject.activeInHierarchy)
        {
            if (playerRef.GetComponent<PlayerHealth>().IsDead || GameManager.Instance.IsGameOver)
            {
                fov.enabled = false;
            }
            else if (!fov.enabled)
            {
                fov.enabled = true;
            }
            
            if (fov.isPlayerInSight && currentState == BehaviorState.Patrolling)
            {
                currentState = BehaviorState.Chasing;
                if (meshAgent.isStopped == true) meshAgent.isStopped = false;
            }
            
            yield return new WaitForSeconds(delay);
        }
    }

    void StartAttackStunPause()
    {
        if (!meshAgent.isStopped)
        {
            meshAgent.isStopped = true;
            //play animation for getting hit
            anim.SetTrigger("IsHit");

            //Instead of invoking here we call the function in animation events.
            //Invoke("EndAttackStunPause", 1.5f);
        }
    }

    public void EndAttackStunPause()
    {
        if (currentState != BehaviorState.Chasing) { meshAgent.SetDestination(playerRef.transform.position); }
        meshAgent.isStopped = false;
    }

    public void HandleDeath()
    {
        //If the enemy hp is 0 handle the death of the enemy.
        Debug.Log(gameObject.name + " is dead");
        currentState = BehaviorState.Dead;
        meshAgent.enabled = false;
        capsuleCollider.enabled = false;
        rightEyeLight.enabled = false;
        leftEyeLight.enabled = false;
        soundComponent.PlaySFX(deathSFX);
        this.enabled = false;
        onDeathExternal?.Invoke();
        //Destroy(gameObject);
    }

    public void SetEnemyToAttackPlayer()
    {
        meshAgent.SetDestination(playerRef.transform.position);
    }

    public void InitializeAfterSpawn()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (meshAgent == null)
            meshAgent = GetComponent<NavMeshAgent>();

        if (capsuleCollider == null)
            capsuleCollider = GetComponent<CapsuleCollider>();

        if (fov == null)
            fov = GetComponent<EnemyFOV>();

        // Animator rebinding + dummy evaluation
        anim.Rebind();
        anim.Update(0f); // Ensures it evaluates entry state

        // Ensure proper animation state based on AI
        switch (currentState)
        {
            case BehaviorState.Patrolling:
                anim.SetBool("IsMoving", true);
                anim.SetBool("IsChasing", false);
                anim.SetBool("Attacking", false);
                break;

            case BehaviorState.Chasing:
                anim.SetBool("IsMoving", false);
                anim.SetBool("IsChasing", true);
                anim.SetBool("Attacking", false);
                break;

            case BehaviorState.Resting:
                anim.SetBool("IsResting", true);
                break;

            case BehaviorState.Confused:
                anim.SetBool("IsConfused", true);
                break;

            case BehaviorState.Dead:
                anim.SetTrigger("IsDead");
                meshAgent.enabled = false;
                capsuleCollider.enabled = false;
                break;
        }

        // Ensure movement resumes
        meshAgent.enabled = true;
        meshAgent.ResetPath();
        meshAgent.isStopped = false;

        if (currentState == BehaviorState.Patrolling && patrolPattern != null)
            meshAgent.SetDestination(currentTargetPoint.position);

        capsuleCollider.enabled = true;
        fov.enabled = true;
        enabled = true;
    }

    public void PauseNavigation()
    {
        if (meshAgent && meshAgent.enabled)
        {
            meshAgent.isStopped = true;
        }
    }

    public void ResumeNavigation()
    {
        if (meshAgent && meshAgent.enabled && currentState != BehaviorState.Dead)
        {
            meshAgent.isStopped = false;
        }
    }
}
