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
        
        TransitionManager.Instance.TransitionToScene(_sceneSwitchName, _cameraSwitchId, _world);
    }
}
