using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using TMPro;

public class PowerPlantCamController : MonoBehaviour
{
    public static PowerPlantCamController Instance { get; set; }
    [SerializeField] private List<CinemachineCamera> sceneCameras;
    private TMP_Text cameraSelectionTxt;

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


        if (sceneCameras.Count == 0)
        {
            Debug.LogError("There are no cameras added to " + gameObject.name + "'s scene cameras list. Please add at minimun one camera to this list.");
            return;
        }
        cameraSelectionTxt = GameObject.Find("CameraSelectionTxt").GetComponent<TMP_Text>();
        cameraSelectionTxt.text = sceneCameras[0].gameObject.name;
        ResetSceneCameras();
    }

    public void ResetSceneCameras()
    {
        int Priority = sceneCameras.Count * 2;

        for (int i = 0; i <= sceneCameras.Count - 1; i++)
        {
            sceneCameras[i].Priority = Priority;

            if (i != 0)
            {
                sceneCameras[i].gameObject.SetActive(false);
            }

            Priority -= 2;
        }
    }


    public void SwitchSceneCameras(CinemachineCamera targetCamera)
    {
        for (int i = 0; i <= sceneCameras.Count - 1; i++)
        {
            if (sceneCameras[i] == targetCamera)
            {
                sceneCameras[i].gameObject.SetActive(true);
                cameraSelectionTxt.text = sceneCameras[i].gameObject.name;
            }
            else
            {
                sceneCameras[i].gameObject.SetActive(false);
            }
        }
    }

}
