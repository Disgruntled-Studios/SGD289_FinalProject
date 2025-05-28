using TMPro;
using UnityEngine;
using Unity.Cinemachine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PowerPlantCamTransitionTrigger : MonoBehaviour
{
    public CinemachineCamera camRefOne;
    public CinemachineCamera camRefTwo;

    public bool switchToCamRefTwoFirst;
    public bool switchToCamRefOneFirst;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            if (switchToCamRefTwoFirst)
            {
                PowerPlantCamController.Instance.SwitchSceneCameras(camRefTwo);
                switchToCamRefTwoFirst = false;
                switchToCamRefOneFirst = true;
            }
            else
            {
                PowerPlantCamController.Instance.SwitchSceneCameras(camRefOne);
                switchToCamRefTwoFirst = true;
                switchToCamRefOneFirst = false;
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

        if (script.switchToCamRefTwoFirst)
        {
            //script.Is_GO_Two_First = EditorGUILayout.Toggle(script.Is_GO_Two_First, typeof(bool), true) as bool;
        }
    }
}

#endif
