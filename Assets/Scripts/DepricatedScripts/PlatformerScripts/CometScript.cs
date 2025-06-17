using UnityEngine;

public class CometScript : MonoBehaviour
{
    [SerializeField]
    private float cometSpeed;
    [SerializeField]
    private GameObject leftBotDeathZone;

    //This script is on comet prefab and controls its movement. It also handles collisions with the player.

    PlatformManager platformManager;

    void Start()
    {
        platformManager = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();
        leftBotDeathZone = GameObject.Find("LeftBotDeathZone");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-1, -1, 0) * cometSpeed * Time.deltaTime;

        if(transform.position.x < leftBotDeathZone.transform.position.x)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            platformManager.HandleDamage();
            //platformManager.CheckGameOver();
            //Implement Object pooling
            this.gameObject.SetActive(false);
        }
    }
}