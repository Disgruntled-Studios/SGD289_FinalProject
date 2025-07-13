using System.Collections;
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public float minIntensity, maxIntensity, pulseLength;
    private float currentTime;
    private bool reachedMaxIntensity;
    private Light sceneLight => GetComponent<Light>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0;
        sceneLight.intensity = minIntensity;
        StartCoroutine(PulseLoop());
    }


    private IEnumerator PulseLoop()
    {
        reachedMaxIntensity = false;
        float t;
        // Debug.Log("Pulse Started");
        while (isActiveAndEnabled)
        {
            if (!reachedMaxIntensity)
            {
                //Debug.Log("have not reached max intensity increasing current time");
                currentTime += 1 * Time.deltaTime;
                t = currentTime / pulseLength;

                sceneLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
                yield return new WaitForEndOfFrame();

                if (currentTime >= pulseLength)
                {
                    //Debug.Log("Reached max intensity reversing");
                    reachedMaxIntensity = true;
                }
            }
            else if (reachedMaxIntensity)
            {
                //Debug.Log("have not reached min intensity decreasing current time");
                currentTime -= 1 * Time.deltaTime;
                t = currentTime / pulseLength;

                sceneLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
                yield return new WaitForEndOfFrame();

                if (currentTime < 0)
                {
                    //Debug.Log("Reached min intensity reversing");
                    reachedMaxIntensity = false;
                }
            }
        }
    }

}
