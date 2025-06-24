using System;
using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    [SerializeField] private string _sceneSwitchName;
    [SerializeField] private string _cameraSwitchId;
    public bool useGameManagerExit;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (useGameManagerExit)
        {
            GameManager.Instance.ResetScene();
            return;
        }
        TransitionManager.Instance.TransitionToScene(_sceneSwitchName, _cameraSwitchId);
    }
}
