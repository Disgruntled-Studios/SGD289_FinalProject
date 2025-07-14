using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject _objectToSpawn;
    [SerializeField] private AudioSource _kickAudio;
    
    public void PlayKickSound()
    {
        if (_kickAudio)
        {
            _kickAudio.PlayOneShot(_kickAudio.clip);
        }
    }

    public void SpawnObjectOnDeath()
    {
        if (_objectToSpawn)
        {
            _objectToSpawn.SetActive(true);
        }
    }
}
