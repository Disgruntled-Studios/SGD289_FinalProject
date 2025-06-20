using TMPro;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Serialization;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private string _camId;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController pc))
        {
            CameraManager.Instance.TrySwitchToCamera(_camId);
            CameraManager.Instance.TrySetCameraTarget(_camId, GameManager.Instance.CameraTarget);
        }
    }
}
