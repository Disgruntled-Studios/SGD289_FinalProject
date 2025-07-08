using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldTrigger : MonoBehaviour
{
    [SerializeField] private string _sceneSwitchName;
    [SerializeField] private string _cameraSwitchId;
    public bool useGameManagerExit;
    public bool nextScene;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (useGameManagerExit)
        {
            GameManager.Instance.ResetScene();
            return;
        }
        
        if (nextScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        
        TransitionManager.Instance.TransitionToScene(_sceneSwitchName, _cameraSwitchId);
    }
}
