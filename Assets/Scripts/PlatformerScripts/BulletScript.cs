using UnityEngine;
using UnityEngine.Windows;

public class BulletScript : MonoBehaviour
{
    //This script is on bullet prefab and controls its movement. It also handles collisions with enemies and destroys bullets that go offscreen.

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private GameObject rightTopDeathZone;
    [SerializeField]
    private GameObject leftBotDeathZone;

    Vector3 direction;

    PlatformManager platformManager;

    private GameObject player;

    private Rigidbody rb;

    [SerializeField]
    private bool usingAddForce = false;

    void Start()
    {
        platformManager = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();
        rightTopDeathZone = GameObject.Find("RightTopDeathZone");
        leftBotDeathZone = GameObject.Find("LeftBotDeathZone");

        player = GameObject.Find("Player");

        var playerScale = player.transform.localScale;

        if (Mathf.Sign(playerScale.z) == 1)
        {
            direction = Vector3.right;
        }
        else if (Mathf.Sign(playerScale.z) == -1)
        {
            direction = Vector3.left;
        }
        else
        {
            print("problem in bullet direction if statement");
            direction = Vector3.up;
        }

        if (usingAddForce)
        {
            rb = this.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(direction * bulletSpeed);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (usingAddForce)
        {
            rb.AddForce(direction * bulletSpeed);
        }
        else
        {
            transform.position += direction * bulletSpeed * Time.deltaTime;
        }

        //destroys bullet if it goes off screen. Change to use object pooling later.
        if(transform.position.x < leftBotDeathZone.transform.position.x || transform.position.y > rightTopDeathZone.transform.position.x)
        {
            Destroy(gameObject);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlatformEnemy"))
        {
            try
            {
                print("set parent of enemy to false");
                other.transform.parent.gameObject.SetActive(false);
            }
            catch
            {
                print("couldn't find parent of enemy, so set enemy false");
                other.gameObject.SetActive(false);
            }
        }
    }
    
}