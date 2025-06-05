using UnityEngine;

public class CometScript : MonoBehaviour
{
    [SerializeField]
    private float cometSpeed;

    //This script is on comet prefab and controls its movement. It also handles collisions with the player.

    PlatformManager platformManager;

    void Start()
    {
        platformManager = GameObject.Find("Canvas").GetComponent<PlatformManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-1, -1, 0) * cometSpeed * Time.deltaTime;
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