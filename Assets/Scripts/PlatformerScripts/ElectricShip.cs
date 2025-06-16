using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;
using static Unity.Burst.Intrinsics.X86;

public class ElectricShip : MonoBehaviour
{
    //Hello Kristen Holcombe here! I wanted the electric ship to spawn in the top right corner above the camera and swoop down in a parabola
    //where the vertex is where the player was when it spawned. Because I had trouble finding information about how to do this easily
    //I decided to write this script myself. The object is spawned (triggered in another script) at the start point.
    //Then it uses FindEquation() to find the parabolic curve to aim at the player using that info.
    //FindEquation() uses the start point and vertex to find a in the vertex equation for parabola.
    //Then it uses TravelAlongCurve() in update to travel along that curve by plugging in an x value that is a small step in enemy movement direction. Step distance is set in inspector.


    [SerializeField]
    private float speed = 3.0f;

    //private Transform[] patrolPoints = new Transform[3]; //made an array with 3 points

    [SerializeField]
    private GameObject startPointObj;
    [SerializeField]
    private GameObject endPointObj;

    [SerializeField]
    private GameObject player;

    private Transform startPoint;
    private Transform endPoint;
    private Transform playerTransform;

    PlatformManager platformManager;

    //used for calculating parabola
    float vertH;
    float vertK;

    float startX;
    float startY;
    float a;

    [SerializeField]
    private float stepDistance;

    //[SerializeField]
    // private float timeBetweenSteps = 0.05f;

    ShipSpawner shipSpawner;
    
    private bool equationFound = false;

    private Vector3 destination;

    void OnDisable()
    {
        CancelInvoke();
        print("destroyed electric ship as it reached endpoint");
        shipSpawner.DecreaseShipCount();

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        CancelInvoke();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPointObj = GameObject.Find("EShipSpawnPos");
        endPointObj = GameObject.Find("EShipEndPos");
        shipSpawner = GameObject.Find("TimedActionsTrigger").GetComponent<ShipSpawner>();

        startPoint = startPointObj.transform;
        endPoint = endPointObj.transform;
        playerTransform = player.transform;

        try
        {
            a = FindEquation();
            //print("a is " + a);
            equationFound = true;
            destination = CalculateStep();
        }
        catch
        {
            print("unable to run FindEquation in Electric Ship");
        }
    }

    private void Update()
    {
        //makes sure player has found the correct a value
        if (equationFound)
        {
              TravelAlongCurve();
        }
    }


    //Uses the vertex equation of the parabola to calculate arc of ship and moves along it by calculating a point and moving towards it.
    private void TravelAlongCurve()
    {
        float stepSpeed = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, destination, stepSpeed);

        //print("Destination is equal to " + destination);
        //print("transform.position is " + transform.position);

        // Checks if enemy reached destinatiton point and moves to next point or resets if so.
        if (Vector3.Distance(transform.position, destination) < 0.001f)
        {
            //print("next point reached. Recalculating steps");
            try
            {
                destination = CalculateStep();
            }
            catch
            {
                print("unable to calculate next step");
            }
        }

        //Checks if endpoint has been reached. May replace with a death zone above camera if this doesn't work.
        if (transform.position.x < endPoint.position.x)
            //Vector3.Distance(transform.position, endPoint.position) < 0.001f)
        {
            //print("endpoint reached");
            CancelInvoke();
            //print("canceled travel along curve invoke repeating");
            print("destroyed electric ship as it reached endpoint");
            shipSpawner.DecreaseShipCount();

            Destroy(gameObject);

        }
    }

    //Uses the start or end point along with player as vertex to find a using vertex form of equation
    public float FindEquation()
    {
        //y = a(x-h)^2+k
        //we know vertex (h,k)
        //we know two points, but we just need 1 to find a.
        vertH = playerTransform.position.x;
        vertK = playerTransform.position.y;

        startX = startPoint.position.x;
        startY = startPoint.position.y;

        //Solving for a: endY = a(endX - (vertH))^2 + vertK;

        //solving for (x-h)^2
        float side1 = Mathf.Pow(startX - vertH, 2);

        //startY = a(side1)+k.
        //Then, subtract the k from startY and make it equal to side2.
        //startY-k = a(side1)
        float side2 = startY - vertK;

        //solve for side2 = a(side1)
        //divide side2 by side1 to put a by itself.
        float a = side2 / side1;

        return a;

        //a has been found. Now we need to plug in X for each step in update.          
    }

    public Vector3 CalculateStep()
    {
        //find x of new position along chart.
        //subtracting since we are going right to left. May change later.
        float x = transform.position.x - stepDistance;

        //y = a(x-h)^2+k. Find y.

        //solving (x-h)^2
        float part1 = Mathf.Pow(x - vertH, 2);
        //multiplying by a.
        float part2 = (a * part1);
        //adding k.
        float y = part2 + vertK;

        Vector3 vector = new Vector3(x, y, transform.position.z);

        return vector;
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            platformManager.HandleDamage();
            print("damaged player");
            CancelInvoke();
            print("canceled invoke");
            this.gameObject.SetActive(false);
        }
    }
    //This enemy does not take damage from player.
}
