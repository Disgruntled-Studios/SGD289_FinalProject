using TMPro;
using UnityEngine;
using Unity.Cinemachine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PowerPlantCamTransitionTrigger : MonoBehaviour
{
    public GameCamera camRef;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Debug.Log("Switching to cam ref");
            CameraManager.Instance.TrySwitchToCamera(camRef.CameraID);
            CameraManager.Instance.TrySetCameraTarget(camRef.CameraID, GameManager.Instance.CameraTarget);
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
