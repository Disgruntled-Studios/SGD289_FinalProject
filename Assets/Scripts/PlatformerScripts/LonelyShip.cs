using UnityEngine;

public class LonelyShip : MonoBehaviour
{
    //Timer for ship to disappear when by itself after a few seconds.
    private float baseTime; //holds current time
    [SerializeField]
    private float waitTime = 3f;
    private float endTime;
    private float resetTime = 0;

    void Start()
    {
        baseTime = (baseTime + Time.deltaTime) - resetTime;
    }

    void Update()
    {
        baseTime = (baseTime + Time.deltaTime) - resetTime;
        if (baseTime > endTime)
        {
            Destroy(gameObject);
        }
    }

}
