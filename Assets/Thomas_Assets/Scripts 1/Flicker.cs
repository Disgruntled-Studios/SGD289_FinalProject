using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Speed (seconds)")]
    [Tooltip("Minimum time between flickers")]
    public float minFlickerSpeed = 0.05f;

    [Tooltip("Maximum time between flickers")]
    public float maxFlickerSpeed = 0.2f;

    [Header("Light Intensity Range")]
    [Tooltip("Minimum light intensity when flickering")]
    public float minIntensity = 0f;

    [Tooltip("Maximum light intensity when flickering")]
    public float maxIntensity = 1f;

    private Light flickerLight;

    void Start()
    {
        flickerLight = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    System.Collections.IEnumerator Flicker()
    {
        while (true)
        {
            // Random flicker timing and intensity
            float waitTime = Random.Range(minFlickerSpeed, maxFlickerSpeed);
            float intensity = Random.Range(minIntensity, maxIntensity);

            flickerLight.intensity = intensity;

            // Optional: disable light completely if intensity is below a small threshold
            flickerLight.enabled = intensity > 0.05f;

            yield return new WaitForSeconds(waitTime);
        }
    }
}
