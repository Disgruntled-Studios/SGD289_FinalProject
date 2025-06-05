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

    [SerializeField] private Transform[] patrolPoints;

    private GameObject player; //in case we want to add an attack state since we are working in 2.5d.

    [SerializeField]
    private GameObject enemy;

    //private float distanceFromPlayer;

    //used to keep enemies from moving frame by frame but still allowing freedom from nav mesh pathing.
    [SerializeField]
    private float pathingDelay = 0.2f;
    private float pathingTime;

    private bool playerInRange = false;

    [SerializeField]
    private bool inSight = false;

    //[SerializeField]
   // private float sightRangeFloat =20f; //perhaps replace with box around player.

    private int point;




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
        player = GameObject.FindGameObjectWithTag("Player");
        point = 0;
    }


    private void Update()
    {

        DecideStates();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("InSight"))
        {
            inSight = true;
            this.enabled = true;
            //activated patrol script
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("InSight"))
        {
            inSight = false;
            this.enabled = false;
        }
    }

    public void DecideStates()
    {

        print("wandering");
        HandleWandering();

    }


    private void HandleWandering()
    {
        //Set so it moves to each patrol point in order

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[point].transform.position, step);

        print("Point is equal to " + point);

        // Checks if enemy reached destinatiton point and moves to next point or resets if so.
        if (Vector3.Distance(transform.position, patrolPoints[point].transform.position) < 0.001f)
        {
            if (point >= (patrolPoints.Length-1))
            {
                point = 0;
            }
            else
            {
                point++;
            }
        }

    }
    


    /*
    public void StopPatrolling()
    {
        //stopPatrol = true;
        print("stopped patrolling using delegate");
    }
    */
}
