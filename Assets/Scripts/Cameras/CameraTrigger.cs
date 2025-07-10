using System;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private string _camId;
    public UnityEvent onEnterEvent;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController pc))
        {
            CameraManager.Instance.RegisterActiveTrigger(_camId);
            onEnterEvent?.Invoke();
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
