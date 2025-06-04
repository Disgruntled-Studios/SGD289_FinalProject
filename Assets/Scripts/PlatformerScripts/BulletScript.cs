using UnityEngine;
using UnityEngine.Windows;

public class BulletScript : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed;

    Vector3 direction;

    //This script is on comet prefab and controls its movement. It also handles collisions with the player.

    PlatformManager platformManager;

    private GameObject player;

    void Start()
    {
        platformManager = GameObject.Find("Canvas").GetComponent<PlatformManager>();
        player = GameObject.Find("Player");

        var playerScale = player.transform.localScale;

        if (Mathf.Sign(playerScale.x) == 1)
        {
            direction = Vector3.right;
        }
        else if (Mathf.Sign(playerScale.x) == -1)
        {
            direction = Vector3.left;
        }
        else
        {
            print("problem in bullet direction if statement");
            direction = Vector3.up;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            platformManager.HandleDamage();
            platformManager.CheckGameOver();
            //Implement Object pooling
            this.gameObject.SetActive(false);
        }
    }
}