using UnityEngine;

public class LonelyShip : MonoBehaviour
{
    //Timer for ship to disappear when by itself after a few seconds.
    private float baseTime; //holds current time
    [SerializeField]
    private float existTime = 100f;
    private float detachedTime = 3f;
    private float endTime;
    private float resetTime = 0;

    //separated is changed when player attaches and detaches from cloud
    private bool separated = true;

    //code a claimed variable so ship can be claimed and separated countdown stopped.

    void Start()
    {
        baseTime = (baseTime + Time.deltaTime) - resetTime;
        //sets timer for how long player has to claim abandoned cloud.
        endTime = baseTime + detachedTime;
        separated = true;
    }

    void Update()
    {
        baseTime = (baseTime + Time.deltaTime) - resetTime;

        if(separated)
        {
            //CountDown();
        }

        
    }

    public void CountDown()
    {
        //if ship is separated from player, player has this long to reclaim it.
        //check if ship has enough existing time for full separated countdown
        //otherwise set separated countdown to existing time.
        print("separatedCountdown is not yet coded");

        if (baseTime > endTime)
        {
            print("destroying LonelyShip in countdown");
            Destroy(gameObject);
        }
    }

}
