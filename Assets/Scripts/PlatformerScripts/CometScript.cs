using UnityEngine;

public class CometScript : MonoBehaviour
{
    [SerializeField]
    private float cometSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    PlatformManager platformManager;

    void Start()
    {
        platformManager = GameObject.Find("Canvas").GetComponent<PlatformManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-1, -1, 0) * Time.deltaTime;
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