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





    private void Start()
    {
        pm = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();

        baseTime = (baseTime + Time.deltaTime) - resetTime;

        invObj.SetActive(false);
    }

    private void Update()
    {
        baseTime = (baseTime + Time.deltaTime) - resetTime;
        print("base time is" + baseTime + ". And endtime is " + endTime);

        if(invincible)
        {
            //Check when invincibility ends
            if(baseTime > endTime)
            {
                print("invincibility ended");
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PlatformCoin"))
        {
                //print("platformCoin");
                pm.CalculateCoins(1);
                other.gameObject.SetActive(false);
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
            }

            try
            {
                //print("set parent of enemy to false");
                other.transform.parent.gameObject.SetActive(false);
            }
            catch
            {
                //print("couldn't find parent of enemy, so set enemy false");
                other.gameObject.SetActive(false);
                //add functionality to turn enemies back on when they end up off screen but player returns maybe..
            }
        }
    }
}
