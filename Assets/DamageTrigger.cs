using UnityEngine;
using UnityEngine.Timeline;

public class DamageTrigger : MonoBehaviour
{
    public float damageAmount;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            other.gameObject.GetComponent<PlayerController>().playerHealth.Damage(damageAmount);
        }
    }
}
