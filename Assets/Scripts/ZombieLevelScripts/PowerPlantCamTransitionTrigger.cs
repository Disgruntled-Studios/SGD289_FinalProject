using TMPro;
using UnityEngine;
using Unity.Cinemachine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PowerPlantCamTransitionTrigger : MonoBehaviour
{
    public GameCamera camRefOne;
    public GameCamera camRefTwo;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //Debug.Log("Player Detected");
            if (camRefOne.GetComponent<CinemachineCamera>().Priority > 0)
            {
                Debug.Log("Switching to cam two");
                CameraManager.Instance.TrySwitchToCamera(camRefTwo.CameraID);
                CameraManager.Instance.TrySetCameraTarget(camRefTwo.CameraID, GameManager.Instance.CameraTarget);
            }
            else if (camRefTwo.GetComponent<CinemachineCamera>().Priority > 0)
            {
                Debug.Log("Switching to cam one");
                CameraManager.Instance.TrySwitchToCamera(camRefOne.CameraID);
                CameraManager.Instance.TrySetCameraTarget(camRefOne.CameraID, GameManager.Instance.CameraTarget);
            }
        }
    }

    bool[] boolHandler(bool[] b, int index, bool setTo = true)
    {
        for (int i = 0; i < b.Length; i++)
        {
            if (i == index)
            {
                b[i] = setTo;
            }
            else
            {
                b[i] = !setTo;
            }
        }

        return b;

    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(PowerPlantCamTransitionTrigger))]
public class PowerPlantCamTransitionTrigger_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PowerPlantCamTransitionTrigger script = (PowerPlantCamTransitionTrigger)target;
    }
}

#endif
