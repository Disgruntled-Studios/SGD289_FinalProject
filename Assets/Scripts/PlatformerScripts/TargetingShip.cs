using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.XR.Haptics;

public class TargetingShip : MonoBehaviour
{
    //I want enemies with this script to patrol between points in the air or on the ground.

    [SerializeField]
    private float speed =3.0f;

    [SerializeField]
    private GameObject tShipPos2;

    [SerializeField]
    private GameObject leftDeathZone;

    //[SerializeField] private Transform[] patrolPoints;

    private GameObject player; //in case we want to add an attack state since we are working in 2.5d.

    Vector3 targetPoint;

    [SerializeField]
    private float movingRightSpeed;
    [SerializeField]
    private float findingPlayerSpeed;
    [SerializeField]
    private float settlingSpeed;
    [SerializeField]
    private float chargingForwardSpeed;

    [SerializeField]
    private float overshootDistance;

    //used to get level with the player because too low otherwise.
    [SerializeField]
    private float playerPosRefinement;
    private float playerPos;

    
    

    private string state = "movingRight";

    ShipSpawner shipSpawner;

    private void Start()
    {
        //ship spawns at start position using timedActionsTrigger
        //then it moves to position 2 in update using speed1MoveLeft
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
            tShipPos2 = GameObject.Find("TShipPos2");
            leftDeathZone = GameObject.Find("LeftBotDeathZone");
            shipSpawner = GameObject.Find("TimedActionsTrigger").GetComponent<ShipSpawner>();
        }
        catch
        {
            print("objects not found in start function of targeting ships");
        }
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        //Move from start to the right.
        //Once it reaches there, it moves down until it level with player


        float step = speed * Time.deltaTime;

        //this adds a little extra height in case disc is too high or too low when it finds the player.
        playerPos = player.transform.position.y + playerPosRefinement;

        //figures out what this enemy should currently be doing.
        if (state == "movingRight")
        {
            targetPoint = tShipPos2.transform.position;
            speed = movingRightSpeed;
        }
        else if (state == "findingPlayer")
        {
            targetPoint = new Vector3(transform.position.x, playerPos - overshootDistance, transform.position.z);
            speed = findingPlayerSpeed;
        }
        else if (state == "settling")
        {
            targetPoint = new Vector3(transform.position.x, playerPos, transform.position.z);
            speed = settlingSpeed;
        }
        else if (state == "chargingForward")
        {
            transform.position += new Vector3(-1, 0, 0) * chargingForwardSpeed * Time.deltaTime;
            speed = chargingForwardSpeed;
        }
        else
        {
            print("problem in Move function, assigning targetPoint");
        }

        if (state != "chargingForward") //as long as not using translate to move straight forward at player, use this move towards
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, step);
        }

        //print("Point is equal to " + point);

        // Checks if enemy reached destinatiton point and moves to next point or resets if so.
        if (state != "!chargingForward" && Vector3.Distance(transform.position, targetPoint) < 0.001f)
        {
            //maybe change this into a list of states or array of them or something.
            if (state == "movingRight")
            {
                state = "findingPlayer";
            }
            else if (state == "findingPlayer")
            {
                state = "settling";
            }
            else if (state == "settling")
            {
                state = "chargingForward";
            }
        }
        else if(state == "chargingForward" && (transform.position.x < leftDeathZone.transform.position.x))
        {
            print("destroying Targeting ship");
            //when a ship is destroyed. decrease shipCount in shipSpawner so a new one can be spawned.
            shipSpawner.DecreaseShipCount();

            Destroy(this.gameObject);
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
