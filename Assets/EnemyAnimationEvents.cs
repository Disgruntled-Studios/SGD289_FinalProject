using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject _objectToSpawn;
    
    public void PlayKickSound()
    {
        // TODO: Kick Sound
    }

    public void SpawnObjectOnDeath()
    {
        if (_objectToSpawn)
        {
            _objectToSpawn.SetActive(true);
        }
    }
}
