using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.XR.Haptics;

public class VeraScript : MonoBehaviour
{
    //I want enemies with this script to patrol between points in the air or on the ground.

    [SerializeField]
    private float speed =3.0f;

    [SerializeField] private Transform[] patrolPoints;


    private bool playerInRange = false;

    [SerializeField]
    private int state;

    //[SerializeField]
   // private float sightRangeFloat =20f; //perhaps replace with box around player.

    private int point;

    Animator anim;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 targetVector;

    private bool timerOn = false;


    //when item is disabled, stop moving anim.
    private void OnDisable()
    {
        //anim.SetBool("moving", false);
    }

    private void Start()
    {
        point = 0;
        if(anim == null)
            anim = this.gameObject.GetComponent<Animator>();
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
            this.enabled = false;
        }
    }

    public void DecideStates()
    {

        //print("wandering");
        if (state == 0) //walking
        {
            anim.SetFloat("MoveSpeed", moveSpeed);
            HandleWandering();
            //set so walking
        }
        else if(state == 1)
        {
            anim.SetBool("Turning", true);

            //set so turn, lift and shoot
            //transform.LookAt(target);
            if (targetVector == null)
            {
                targetVector = new Vector3(target.position.x, target.position.y, target.position.z);
            }

            //Vector3 currentDirection = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);

            //Vector3 direction = Vector3.RotateTowards(currentDirection, targetVector, Time.fixedDeltaTime, 0);
            //transform.rotation = Quaternion.RotateTowards(currentDirection, targetVector, 0);

            //anim.SetBool("Turning", true);

            //anim.SetTrigger("Turning", true);


            Vector3 current = transform.forward;
            Vector3 to = target.position - transform.position;
            transform.forward = Vector3.RotateTowards(current, to, Time.fixedDeltaTime, 2.5f);

            if (timerOn == false)
            {
                StartCoroutine("TurnTimer");
            }

        }
        else if (state == 2)
        {
            //anim.SetBool("Turning", false);
            anim.SetBool("IsAiming",true);
        }



    }


    IEnumerator TurnTimer()
    {
        timerOn = true;
        anim.SetFloat("MoveSpeed", 1f);
        //anim.SetBool("Turning", true);
        yield return new WaitForSeconds(1f);
        //anim.SetBool("Turning", false);
        anim.SetFloat("MoveSpeed", 0f);
        state = 2;
        timerOn = false;
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
                anim.SetFloat("MoveSpeed", 2f);
                state = 1;
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
