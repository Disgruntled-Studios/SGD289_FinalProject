using UnityEngine;
using UnityEngine.Timeline;

public class DamageTrigger : MonoBehaviour
{
    public float damageAmount;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Debug.Log(other.gameObject.GetComponent<PlayerHealth>().CurrentHealth + " Should now be " + (other.gameObject.GetComponent<PlayerHealth>().CurrentHealth - damageAmount));
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
            Debug.Log(other.gameObject.GetComponent<PlayerHealth>().CurrentHealth);
        }
    }
}
