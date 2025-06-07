using System;
using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    [SerializeField] private World _world;
    [SerializeField] private string _sceneSwitchName;
    [SerializeField] private string _cameraSwitchId;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        Debug.Log($"Transition to {_world}");
        GameManager.Instance.SwitchPlayerMode(_world);
        TransitionManager.Instance.TransitionToScene(_sceneSwitchName, _cameraSwitchId);
    }
}
