using System;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private CutsceneManager _manager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        _manager?.StartCutscene();
        gameObject.SetActive(false);
    }
}
