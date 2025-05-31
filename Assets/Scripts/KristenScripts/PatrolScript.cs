using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.XR.Haptics;

public class PatrolScript : MonoBehaviour
{
    //I want enemies with this script to patrol between points in the air or on the ground.

    [SerializeField]
    private float speed =3.0f;

    private AIState nowState;
    enum AIState{attacking, wandering}; //perhaps change to be enum later

    [SerializeField] private Transform[] patrolPoints;

    private GameObject player; //in case we want to add an attack state since we are working in 2.5d.

    [SerializeField]
    private GameObject enemy;

    private float distanceFromPlayer;

    //used to keep enemies from moving frame by frame but still allowing freedom from nav mesh pathing.
    [SerializeField]
    private float pathingDelay = 0.2f;
    private float pathingTime;

    private bool playerInRange = false;
    private bool InSight;

    private float sightRangeFloat; //perhaps replace with box around player.




    /*
    [SerializeField] private Transform enemy;

    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft;

    [SerializeField] private float waitDuration; //amount of time enemy pauses at end of path.
    private float waitTimer;

    [SerializeField] private Animator anim;

    //Enemies enemiesScript;
    private bool stopPatrol;
    */

    //when item is disabled, stop moving anim.
    private void OnDisable()
    {
        //anim.SetBool("moving", false);
    }

    private void Start()
    {
        nowState = AIState.wandering;
        player = GameObject.FindGameObjectWithTag("Player");
    }


    private void Update()
    {

        DecideStates();
        
        /*
        if (stopPatrol != true) //used when turning enemies off.
        {
            if (movingLeft)
            {
                if (enemy.position.x >= leftEdge.position.x)
                {
                    MoveInDirection(-1);
                }
                else
                {
                    DirectionChange();
                }
            }
            else
            {
                if (enemy.position.x <= rightEdge.position.x)
                {
                    MoveInDirection(1);
                }
                else
                {
                    DirectionChange();
                }
            }
        }

        */
    }

    public void DecideStates()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);


        if(distanceFromPlayer > sightRangeFloat)
        {
            InSight = true;
        }
        else
        {
            InSight = false;
        }


        if ((!playerInRange) && (!InSight))
        {
            print("deactivated patrolScript");
            
            this.enabled = false;
        }


        if (playerInRange)
        {
            nowState = AIState.attacking;
        }
        else if((!playerInRange) && (InSight))
        {
            nowState = AIState.wandering;
        }
        else
        {
            print("error in DecideStates() if statement");
        }

        HandleStates();
    }


    //Remove this handle states function??
    private void HandleStates()
    {
        switch(nowState)
        {
            case AIState.wandering:
                Debug.Log("Wandering");

                //HandleWandering();

                break;
            case AIState.attacking:
                Debug.Log("Attacking");
                break;
            default:
                Debug.LogError("Error in Handle States switch");
                break;
        }
    }

    /*
    private void HandleWandering()
    {
        //Set it so it randomly chooses a patrol point and ues 
        patrolPoints[Random.Range[0,patrolPoints.Length)

        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            //chooses another patrol Point as destination. Also use pathing delay like that video suggested to cut down on lag. 
        }
    }
    */

    private void MoveTowardsDestination(GameObject target)
    {
        var step = speed * Time.deltaTime;
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, target.transform.position, step);
    }
    


    /*
    public void StopPatrolling()
    {
        //stopPatrol = true;
        print("stopped patrolling using delegate");
    }
    */
}
