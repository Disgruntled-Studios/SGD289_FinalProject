using UnityEngine;

public class LonelyShip : MonoBehaviour
{
    //Timer for ship to disappear when by itself after a few seconds.
    private float baseTime; //holds current time
    [SerializeField]
    private float existTime = 3f;
    private float detachedTime = 3f;
    private float endTime;
    private float resetTime = 0;

    //code a claimed variable so ship can be claimed and separated countdown stopped.

    void Start()
    {
        baseTime = (baseTime + Time.deltaTime) - resetTime;
        //sets timer for how long player has to claim abandoned cloud.
        endTime = baseTime + detachedTime;
    }

    void Update()
    {
        baseTime = (baseTime + Time.deltaTime) - resetTime;
        if (baseTime > endTime)
        {
            print("destroying LonelyShip");
            Destroy(gameObject);
        }
    }

    public void CountDown()
    {
        //if ship is separated from player, player has this long to reclaim it.
        //check if ship has enough existing time for full separated countdown
        //otherwise set separated countdown to existing time.
        print("separatedCountdown is not yet coded");
    }

}
