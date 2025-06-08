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
    private float speed =3.0f;

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




    [SerializeField]
    private GameObject enemy;

    PlatformManager platformManager;

    //used for calculating parabola
    float vertH;
    float vertK;

    float startX;
    float startY;
    float a;

    [SerializeField]
    private float stepDistance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPoint = startPointObj.transform;
        endPoint = endPointObj.transform;
        playerTransform = player.transform;

        a = FindEquation();


        //put actual spawn in a trigger object for now and add a label.

    }


    private void Update()
    {
        TravelAlongCurve();
    }
    private void TravelAlongCurve()
    {
        enemy.transform.position = CalculateStep();

        //Checks if endpoint has been reached. May replace with a death zone above camera if this doesn't work.
        if (Vector3.Distance(enemy.transform.position, endPoint.position) < 0.001f)
        {
            print("destination reached");
            this.gameObject.SetActive(false);
        }
    }

    //This 
    public float FindEquation()
    {
        //y = a(x-h)^2+k
        //we know vertex (h,k)
        //we know two points, but we just need 1 to find a.
        vertH = playerTransform.position.x;
        vertK = playerTransform.position.y;

        startX = startPoint.position.x;
        startY = startPoint.position.y;

        //rewrite the equation and put answer in solution as I go
        float side1 = 0;
        float side2 = 0;
        a = 0;


        //endY = a(endX - (vertH))^2 + vertK;
        side1 = Mathf.Pow(startX - vertH, 2);
        side2 = startY - vertK;
        a = side2 / side1;

        return a;

        //a has been found. Now we need to plug in X for each step in update.          
    }

    public Vector3 CalculateStep()
    {
        //x has moved since last step. If we move the next step, subtract step distance and find y and move it there.
        float x = enemy.transform.position.x - stepDistance;

        float y = 0;

        //use this equation to find y by plugging in x. y = a(x - h) ^ 2 + k

        vertH = playerTransform.position.x;
        vertK = playerTransform.position.y;

        startX = startPoint.position.x;
        startY = startPoint.position.y;

        //endY = a(endX - (vertH))^2 + vertK;
        float part1 = Mathf.Pow(startX - vertH, 2);
        y = (a * part1);
        y = y + vertK;

        //float step = speed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, patrolPoints[point].transform.position, step);

        x = x * speed * Time.deltaTime;
        y = y * speed * Time.deltaTime;

        Vector3 vector = new Vector3(x, y, enemy.transform.position.z);

        //vector = vector *speed * Time.deltaTime;

        return vector;
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //platformManager.HandleDamage();
            print("damaged player");
        }
    }
    //This enemy does not take damage from player.
}
