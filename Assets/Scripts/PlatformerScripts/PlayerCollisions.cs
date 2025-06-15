using System.Reflection;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{

    PlatformManager pm;

    //player is invincible for a short time after taking damage.
    [SerializeField]
    public bool invincible = false;

    //Invincibility timer
    private float baseTime; //holds current time
    [SerializeField]
    private float dmgInvTime = 3f;
    private float endTime;
    private float resetTime = 0;

    [SerializeField]
    private GameObject invObj;


    ShipSpawner shipSpawner;
    Rigidbody rb;

    [SerializeField]
    private float bounceForce = 3f;

    [SerializeField]
    public bool hasShip = false;

    //this holds a reference to the ship if player has one.
    private GameObject ship;


    private void Start()
    {
        hasShip = false;
        pm = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();
        shipSpawner = GameObject.Find("TimedActionsTrigger").GetComponent<ShipSpawner>();
        rb = this.gameObject.GetComponent<Rigidbody>();

        baseTime = (baseTime + Time.deltaTime) - resetTime;

        invObj.SetActive(false);
    }

    private void Update()
    {
        baseTime = (baseTime + Time.deltaTime) - resetTime;
        //print("base time is" + baseTime + ". And endtime is " + endTime);

        if(invincible)
        {
            //Check when invincibility ends
            if(baseTime > endTime)
            {
                //print("invincibility ended");
                invObj.SetActive(false);
                invincible = false;
            }
        }
    }

    public void BecomeInvincibleDamage()
    {
        //set player to a new color or add an effect for the invincibility.
        endTime = dmgInvTime + baseTime;
        invObj.SetActive(true);
        invincible = true;
        //I think this should also reset timer if player triggers this again while already invincible.

    }

    //Adds a little bounce after player squashes an enemy
    public void Bounce()
    {
        rb.AddForce(Vector3.up * bounceForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PlatformCoin"))
        {
                //print("platformCoin");
                pm.CalculateCoins(1);
                other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("LonelyShip"))
        {

            //Move player to same position as lonely ship.
            transform.position = other.transform.position;

            //Make ship a child of the player
            other.gameObject.transform.parent = gameObject.transform;

            ship = other.gameObject;

            //Turn "hasShip to true in Platform Manager
            hasShip = true;
        }




        if (other.gameObject.CompareTag("SquashObj"))
        {
            Bounce();

            //get point value script from parent and then set entire thing to false, maybe change for targeting
            GameObject pnt = other.gameObject.transform.parent.gameObject;
            
            PointValue pv = pnt.GetComponent<PointValue>();

            pv.GetPoints();

            if (pnt.name == "TargetingShip(Clone)")
            {
                //print("squashing Targeting ship");
                //shipSpawner.DecreaseShipCount();

                //Destroy(pnt);
                TargetingShip targetingShip = pnt.gameObject.GetComponent<TargetingShip>();
                targetingShip.ReplaceWithLonelyShip();
            }
            else
            {
                other.gameObject.transform.parent.gameObject.SetActive(false);
            }
            return;
        }
        else if (other.gameObject.CompareTag("PlatformEnemy"))
        {
            if (!invincible)
            {
                pm.HandleDamage();
            }
            else
            {
                print("player is invincible and cannot be damaged");

                //get point value script from parent and then set entire thing to false, maybe change for targeting

                PointValue pointV = other.gameObject.GetComponent<PointValue>();

                pointV.GetPoints();
            }

            KillEnemy(other.gameObject);

            
        }
    }

    public void ExitShip()
    {
        hasShip = false;
        if (ship)
        {

            ship.transform.parent = null;
            LonelyShip lonelyShipScript = ship.GetComponent<LonelyShip>();
            lonelyShipScript.CountDown();
        }
        ship = null;


    }

    private void KillEnemy(GameObject obj)
    {
        try
        {
            //print("set parent of enemy to false");
            obj.transform.parent.gameObject.SetActive(false);
        }
        catch
        {
            //print("couldn't find parent of enemy, so set enemy false");
            if (obj.name == "TargetingShip(Clone)")
            {
                //print("destroying Targeting ship");
                //shipSpawner.DecreaseShipCount();

                //Destroy(obj);
                TargetingShip targetingShip = obj.gameObject.GetComponent<TargetingShip>();
                targetingShip.ReplaceWithLonelyShip();
            }
            else
            {
                obj.gameObject.SetActive(false);
            }
            //add functionality to turn enemies back on when they end up off screen but player returns maybe..
        }
    }
}
