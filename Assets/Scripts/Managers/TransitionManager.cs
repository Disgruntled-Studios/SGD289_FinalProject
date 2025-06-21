using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    private string _currentSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        var activeScene = SceneManager.GetActiveScene();
        _currentSceneName = activeScene.name;
    }

    public void TransitionToScene(string sceneName, string cameraId)
    {
        StartCoroutine(TransitionRoutine(sceneName, cameraId));
    }

    private IEnumerator TransitionRoutine(string sceneName, string cameraId)
    {
        // Load new scene additively if not already loaded
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (loadOp != null)
                yield return new WaitUntil(() => loadOp.isDone);
        }

        yield return new WaitForEndOfFrame();

        var loadedScene = SceneManager.GetSceneByName(sceneName);

        // Set new scene as active BEFORE unloading
        SceneManager.SetActiveScene(loadedScene);
        _currentSceneName = sceneName;

        // Unload all other loaded scenes except the active one
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && scene.name != _currentSceneName)
            {
                var unloadOp = SceneManager.UnloadSceneAsync(scene);
                if (unloadOp != null)
                {
                    yield return new WaitUntil(() => unloadOp.isDone);
                }
                else
                {
                    Debug.LogWarning($"Unload operation for '{scene.name}' was null!");
                }
            }
        }

        // Spawn & setup
        DestroyDuplicatePlayers();
        SetPlayerToSpawnPoint(loadedScene);

        CameraManager.Instance.TrySetCameraTarget(cameraId, GameManager.Instance.CameraTarget);
        CameraManager.Instance.TrySwitchToCamera(cameraId);

        yield return new WaitForSeconds(0.1f);
    }


    private void SetPlayerToSpawnPoint(Scene scene)
    {
        foreach (var rootObject in scene.GetRootGameObjects())
        {
            var spawnPoint = rootObject.GetComponentInChildren<LevelStartPoint>();
            if (spawnPoint)
            {
                var playerRb = GameManager.Instance.Player.GetComponent<Rigidbody>();
                if (playerRb)
                {
                    playerRb.MovePosition(spawnPoint.transform.position);
                    playerRb.MoveRotation(spawnPoint.transform.rotation);
                }
                
                return;
            }
        }

        Debug.LogWarning($"[SPAWN] No spawn point found in scene '{scene.name}'!");
    }

    private void DestroyDuplicatePlayers()
    {
        var allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in allPlayers)
        {
            if (player != GameManager.Instance.Player)
            {
                Destroy(player);
            }
        }
    }
}
