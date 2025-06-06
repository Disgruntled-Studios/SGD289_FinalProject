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
        if (other.gameObject.CompareTag("Enemy"))
        {
            print("player collided with enemy");
            pm.HandleDamage();
        }
    }
}
