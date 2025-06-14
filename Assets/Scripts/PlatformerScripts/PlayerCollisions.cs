using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{

    PlatformManager pm;


    private void Start()
    {
        pm = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PlatformCoin"))
        {
                print("platformCoin");
                pm.CalculateCoins(1);
                other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("PlatformEnemy"))
        {
            //print("player collided with enemy");
            pm.HandleDamage();

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
