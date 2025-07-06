using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.XR.Haptics;

public class VeraScript : MonoBehaviour
{
    //I want enemies with this script to patrol between points in the air or on the ground.

    [SerializeField]
    private float speed =3.0f;

    [SerializeField] private Transform[] patrolPoints;

    [SerializeField]
    private int state;

    //[SerializeField]
   // private float sightRangeFloat =20f; //perhaps replace with box around player.

    private int point;


    //when item is disabled, stop moving anim.
    private void OnDisable()
    {
        //anim.SetBool("moving", false);
    }

    private void Start()
    {
        point = 0;
    }


    private void Update()
    {

        DecideStates();
    }
    

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("InSight"))
        {
            print("not in sight");
            inSight = false;
            this.enabled = false;
        }
    }

    public void DecideStates()
    {

        //print("wandering");
        if (state == 1) //walking
        {
            HandleWandering();
            //set so walking
        }
        else if(state == 2)
        {
            //set so turn, lift and shoot
        }

    }


    private void HandleWandering()
    {
        //Set so it moves to each patrol point in order

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[point].transform.position, step);

        //print("Point is equal to " + point);

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
