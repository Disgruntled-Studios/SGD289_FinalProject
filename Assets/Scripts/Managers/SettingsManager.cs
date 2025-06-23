using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle fullScreenTog;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fullScreenTog.isOn = Screen.fullScreen;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyGraphics()
    {
        Screen.fullScreen = fullScreenTog.isOn;
    }
}
