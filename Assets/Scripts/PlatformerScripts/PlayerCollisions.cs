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


    private void Start()
    {
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
        
        if (other.gameObject.CompareTag("SquashObj"))
        {
            Bounce();

            //get point value script from parent and then set entire thing to false, maybe change for targeting
            GameObject pnt = other.gameObject.transform.parent.gameObject;
            
            PointValue pv = pnt.GetComponent<PointValue>();

            pv.GetPoints();

            if (pnt.name == "TargetingShip(Clone)")
            {
                print("squashing Targeting ship");
                shipSpawner.DecreaseShipCount();

                Destroy(pnt);
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
                shipSpawner.DecreaseShipCount();

                Destroy(obj);
            }
            else
            {
                obj.gameObject.SetActive(false);
            }
            //add functionality to turn enemies back on when they end up off screen but player returns maybe..
        }
    }
}
