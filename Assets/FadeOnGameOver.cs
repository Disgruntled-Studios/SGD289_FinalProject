using System.Collections;
using UnityEngine;

public class FadeOnGameOver : MonoBehaviour
{
    private AudioSource audioSource;
    private float startingVolume;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        startingVolume = audioSource.volume;
    }
    
    public IEnumerator FadeSoundIn()
    {
        float currentTime = 1f;

        Debug.Log("Starting FadeIn");
        while (audioSource.volume < startingVolume)
        {
            currentTime -= 1 * Time.deltaTime;
            float percentage = currentTime / 1;

            audioSource.volume = Mathf.Lerp(startingVolume, 0, percentage);
            yield return new WaitForEndOfFrame();
        }

    }

    public IEnumerator FadeSoundOut()
    {
        float currentTime = 1f;

        Debug.Log("Starting FadeOut");
        while (audioSource.volume > 0)
        {
            currentTime -= 1 * Time.deltaTime;
            float percentage = currentTime / 1;

            audioSource.volume = Mathf.Lerp(0, startingVolume, percentage);
            yield return new WaitForEndOfFrame();
        }
    }
}
