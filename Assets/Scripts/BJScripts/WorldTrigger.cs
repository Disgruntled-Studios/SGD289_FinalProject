using System;
using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    [SerializeField] private World _world;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraManager.Instance.SwitchTo(_world);
        }
    }
}
