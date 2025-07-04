using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    public enum BehaviorState
    {
        patrolling,
        chasing,
        Resting,
        Dead,
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
        meshAgent.stoppingDistance = attackDistance - .25f;
        if (patrolPattern != null)
        {
            meshAgent.SetDestination(patrolPoints[patrolIndex].position);
            meshAgent.isStopped = false;
        }
        else { meshAgent.isStopped = true; }

        StartCoroutine("EnemyDetectionWithDelay", .1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            StateHandler();
            //anim.SetBool("IsMoving", !meshAgent.isStopped);
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
            case BehaviorState.patrolling:
                if (leftEyeLight.enabled == false)
                {
                    leftEyeLight.enabled = true;
                    rightEyeLight.enabled = true;
                }
                if (!meshAgent.isStopped)
                {
                    anim.SetBool("IsMoving", true);
                    anim.SetBool("IsChasing", false);
                }
                else
                {
                    anim.SetBool("IsMoving", false);
                }
                // else if (fov.isPlayerInSight && playerDist < fov.viewRadius)
                // {
                //     StopCoroutine(SetAgentDestToCurrentTarget(6));
                //     currentTargetPoint = fov.visibleTarget;
                //     Debug.Log("Staring at player");
                //     transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
                //     meshAgent.speed = investigationMovSpeed;
                //     meshAgent.SetDestination(currentTargetPoint.position);
                // }
                // else if (!fov.isPlayerInSight && currentTargetPoint != currentPatrolPoint && detectionLvl > 0 && Vector3.Distance(transform.position, fov.visibleTargetLastPos) > attackDistance)
                // {
                //     Debug.Log("Staring at player, and wandering towards them with a detection level of : " + detectionLvl);
                //     transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
                //     meshAgent.SetDestination(fov.visibleTargetLastPos);
                // }
                // else if (!fov.isPlayerInSight && currentTargetPoint != currentPatrolPoint && Vector3.Distance(transform.position, fov.visibleTargetLastPos) <= attackDistance + .3f)
                // {
                //     currentTargetPoint = currentPatrolPoint;
                //     Debug.Log("Invoking set destination in 6 seconds");
                //     StartCoroutine(SetAgentDestToCurrentTarget(6));
                // }
                if (currentTargetPoint == currentPatrolPoint && Vector3.Distance(transform.position, currentTargetPoint.position) <= attackDistance && patrolPattern != null)
                {
                    anim.SetBool("IsMoving", false);
                    Debug.Log("Made it to the patrol point goint to the next");
                    StartCoroutine("SetNextPatrolPoint");
                    meshAgent.speed = patrolSpeed;
                }
                break;

            case BehaviorState.chasing:
                if (leftEyeLight.color != Color.red || !leftEyeLight.enabled)
                {
                    leftEyeLight.color = Color.red;
                    rightEyeLight.color = Color.red;
                    leftEyeLight.enabled = true;
                    rightEyeLight.enabled = true;
                }

                if (playerDist <= attackDistance && !health.IsDead)
                {
                    transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
                    anim.SetBool("IsMoving", false);
                    anim.SetBool("IsChasing", false);
                    anim.SetBool("Attacking", true);
                    Debug.Log("IM ATTACKING THE PLAYER ARE YA PROUD DAD!?!?!?!");
                    break;
                }
                else if (!health.IsDead)
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

    // IEnumerator SetAgentDestToCurrentTarget(float delay)
    // {
    //     //Debug.Log("Starting delay");
    //     yield return new WaitForSeconds(delay);
    //     meshAgent.SetDestination(currentTargetPoint.position);
    //     //Debug.Log("Stopping delay");
    // }

    // void ToggleEnemyMaterial()
    // {
    //     MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
    //     //Debug.Log("ToggleMatIsCalled " + meshRenderer.gameObject.name);
    //     if (meshRenderer.sharedMaterial == normalMat)
    //     {
    //         //Debug.Log("Switching material to damaged");
    //         meshRenderer.material = damagedMat;
    //         Invoke("ToggleEnemyMaterial", 0.5f);
    //     }
    //     else if (meshRenderer.sharedMaterial == damagedMat)
    //     {
    //         //Debug.Log("Switching material to normal");
    //         meshRenderer.material = normalMat;
    //     }
    // }

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
            if (fov.isPlayerInSight && currentState == BehaviorState.patrolling)
            {
                currentState = BehaviorState.chasing;
                if (meshAgent.isStopped == true) meshAgent.isStopped = false;
            }

            // switch (fov.isPlayerInSight)
            // {
            //     case true:
            //         /*
            //             if (playerRef.GetComponent<PlayerController>().IsCrouching)
            //             {
            //                 // Debug.Log("Enemy sees player crouched");
            //                 detectionLvl += (detectionRate / 2) * Time.deltaTime;
            //             }
            //             else if (Vector3.Distance(transform.position, playerRef.transform.position) <= fov.viewRadius / 2)
            //             {
            //                 // Debug.Log("Player in close proximity");
            //                 detectionLvl = (detectionRate * 2) * Time.deltaTime;
            //             }
            //             else
            //             {
            //                 // Debug.Log("Player within sight range");
            //                 detectionLvl += detectionRate * Time.deltaTime;
            //             }
            //         */

            //             break;
            //     case false:
            //         /*
            //         yield return new WaitForSeconds(Random.Range(2, 7));
            //         if (detectionLvl - detectionRate <= 0)
            //         {
            //             // Debug.Log("Detection dropped to 0");
            //             detectionLvl = 0;
            //         }
            //         else
            //         {
            //             // Debug.Log("Detection dropping");
            //             detectionLvl -= detectionRate * Time.deltaTime;
            //         }
            //         */
            //         break;
            // }

            /*
            if (awarenessImage.fillAmount <= detectionLvl / 1 && detectionLvl / 1 != 1)
            {
                // Debug.Log(detectionLvl + " = detection lvl / " + (detectionLvl / 1) + " = FillAmount");
                awarenessImage.fillAmount = detectionLvl / 1;
            }

            if (detectionLvl >= 1)
            {
                detectionLvl = 1;
                awarenessImage.fillAmount = 1;
                // Debug.Log("Player has been detected with a detectionLvl of : " + detectionLvl);
                currentState = BehaviorState.chasing;
                anim.SetBool("IsMoving", false);
                anim.SetBool("IsChasing", true);
            }
            else if (detectionLvl <= 0 && currentState != BehaviorState.patrolling)
            {
                currentState = BehaviorState.patrolling;
                anim.SetBool("IsMoving", true);
                anim.SetBool("IsChasing", false);
                Debug.Log("Player has escaped returning to patrol");
            }
            */



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
        if (currentState != BehaviorState.chasing) { meshAgent.SetDestination(playerRef.transform.position); }
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
        //Destroy(gameObject);
    }

    public void WakeEnemy()
    {
        anim.SetBool("IsResting", false);
        meshAgent.enabled = true;
        capsuleCollider.enabled = true;
        currentState = BehaviorState.chasing;
        Invoke("SetEnemyToAttackPlayer", 4f);
        Debug.Log(name + " is waking up");
    }

    public void SetEnemyToAttackPlayer()
    {
        meshAgent.SetDestination(playerRef.transform.position);
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
