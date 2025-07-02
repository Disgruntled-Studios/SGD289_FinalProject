using System;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Serialization;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private string _camId;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController pc))
        {
            CameraManager.Instance.RegisterActiveTrigger(_camId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController pc))
        {
            CameraManager.Instance.UnregisterActiveTrigger(_camId);
        }
    }
}
