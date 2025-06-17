using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

public class FPSAudioManager : MonoBehaviour
{
    [SerializeField] private GameObject _audioSourcePrefab;
    [SerializeField] private AudioLibrary _audioLibrary;

    public void PlaySoundAtPosition(string soundName, Vector3 position, float volume = 1f)
    {
        var clip = _audioLibrary?.GetClip(soundName);

        if (!clip || !_audioSourcePrefab) return;

        var obj = Instantiate(_audioSourcePrefab, position, Quaternion.identity);
        var source = obj.GetComponent<AudioSource>();

        if (!source)
        {
            Destroy(obj);
            return;
        }

        source.clip = clip;
        source.volume = volume;
        source.Play();

        Destroy(obj, clip.length);
    }
}
