using UnityEngine;
using UnityEngine.Timeline;

public class DamageTrigger : MonoBehaviour
{
    public float damageAmount;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Debug.Log(other.gameObject.GetComponent<PlayerController>().playerHealth.CurrentHealth + " Should now be " + (other.gameObject.GetComponent<PlayerController>().playerHealth.CurrentHealth - damageAmount));
            other.gameObject.GetComponent<PlayerController>().playerHealth.Damage(damageAmount);
            Debug.Log(other.gameObject.GetComponent<PlayerController>().playerHealth.CurrentHealth);
        }
    }
}
