using System;
using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    [SerializeField] private World _world;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        Debug.Log($"Transition to {_world}");
        GameManager.Instance.SwitchPlayerMode(_world);
    }
}
