using TMPro;
using UnityEngine;

public class PowerPlant_UI_Manager : MonoBehaviour
{
    public static PowerPlant_UI_Manager Instance { get; private set; }
    public TMP_Text healthText;
    public TMP_Text cameraTxt;

    public TMP_Text ammoCountTxt;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
