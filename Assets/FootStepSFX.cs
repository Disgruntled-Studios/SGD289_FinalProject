using System;
using System.Collections;
using UnityEngine;

public class FootStepSFX : MonoBehaviour
{
    public Sound[] sounds;
    public bool isControlledByAnimator = false;
    private Coroutine footstepCo;
    public float footSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.outputAudioMixerGroup = s.mixerOutput;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.isLooping;
            s.source.spatialBlend = s.spatialBlend;
        }
        if (!isControlledByAnimator)
        {
            footstepCo = StartCoroutine(PlayRandomFootstep());
        }
    }

    void Update()
    {
        if (!isControlledByAnimator && footstepCo == null)
        {
            StopAllCoroutines();
            footstepCo = StartCoroutine(PlayRandomFootstep());
        }
        else if (isControlledByAnimator && footstepCo != null)
        {
            StopAllCoroutines();
            footstepCo = null;
        }
    }

    public void PlayFootStep()
    {
        int randInt = UnityEngine.Random.Range(0, sounds.Length);
        Sound s = sounds[randInt];
        s.source.Play();
    }

    public IEnumerator PlayRandomFootstep()
    {
        while (!isControlledByAnimator)
        {
            PlayFootStep();
            yield return new WaitForSeconds(footSpeed);
        }
    }

}
