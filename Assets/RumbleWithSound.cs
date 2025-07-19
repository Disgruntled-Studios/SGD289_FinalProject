using UnityEngine;

public class RumbleWithSound : MonoBehaviour
{
    [SerializeField] private float _rumbleLow = 0.5f;
    [SerializeField] private float _rumbleHigh = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayWithRumble();
    }

    public void PlayWithRumble()
    {
        if (_audioSource.clip == null) return;

        var clipDuration = _audioSource.clip.length;

        _audioSource.Play();
        RumbleController.Instance.TriggerPresetRumble(RumblePreset.EnemyGrowl);
    }
}
