using UnityEngine;
using UnityEngine.Timeline;

public class DamageTrigger : MonoBehaviour
{
    public float damageAmount;
    private PlayerHealth playerRef;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            playerRef = other.gameObject.GetComponent<PlayerHealth>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (playerRef != null && other.gameObject == playerRef.gameObject)
        {
            playerRef = null;
        }
    }

    public void DamagePlayer()
    {
        if (playerRef != null)
        {
            Debug.Log(playerRef.CurrentHealth + " Should now be " + (playerRef.CurrentHealth - damageAmount));
            playerRef.TakeDamage(damageAmount);
            Debug.Log(playerRef.CurrentHealth);
        }
    }

    public void CallDeathFunc()
    {
        GetComponentInParent<EnemyBehavior>().HandleDeath();
    }

    public void CallEndStun()
    {
        GetComponentInParent<EnemyBehavior>().EndAttackStunPause();
    }
}
