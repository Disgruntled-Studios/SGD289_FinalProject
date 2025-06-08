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

        Debug.Log($"[Awake] Starting in scene: {_currentSceneName}");
    }

    public void TransitionToScene(string sceneName, string cameraId)
    {
        Debug.Log($"[TransitionToScene] Request to transition to '{sceneName}' from '{_currentSceneName}'");
        StartCoroutine(TransitionRoutine(sceneName, cameraId));
    }

    private IEnumerator TransitionRoutine(string sceneName, string cameraId)
    {
        Debug.Log($"[TOP] Starting transition. Current: {_currentSceneName}, Target: {sceneName}");

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
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && scene.name != _currentSceneName)
            {
                Debug.Log($"[UNLOADING] Scene: {scene.name}");
                var unloadOp = SceneManager.UnloadSceneAsync(scene);
                if (unloadOp != null)
                    yield return new WaitUntil(() => unloadOp.isDone);
                else
                    Debug.LogWarning($"Unload operation for '{scene.name}' was null!");
            }
        }

        // Spawn & setup
        DestroyDuplicatePlayers();
        SetPlayerToSpawnPoint(loadedScene);

        // Wait for camera registration
        yield return new WaitUntil(() => CameraManager.Instance && CameraManager.Instance.HasCamera(cameraId));

        CameraManager.Instance.TrySetCameraTarget(cameraId, GameManager.Instance.CameraTarget);
        CameraManager.Instance.TrySwitchToCamera(cameraId);

        yield return new WaitForSeconds(0.1f);

        Debug.Log("[LOADED SCENES AFTER CLEANUP]:");
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Debug.Log($"- {SceneManager.GetSceneAt(i).name}");
        }

        Debug.Log($"[COMPLETE] Transition finished. Current scene: {_currentSceneName}");
    }


    private void SetPlayerToSpawnPoint(Scene scene)
    {
        foreach (var rootObject in scene.GetRootGameObjects())
        {
            var spawnPoint = rootObject.GetComponentInChildren<LevelStartPoint>();
            if (spawnPoint != null)
            {
                var playerRb = GameManager.Instance.Player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.MovePosition(spawnPoint.transform.position);
                    playerRb.MoveRotation(spawnPoint.transform.rotation);
                }

                Debug.Log($"[SPAWN] Player moved to spawn in scene '{scene.name}'");
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
                Debug.Log("[DESTROY DUPLICATE] Removed duplicate player object");
            }
        }
    }
}
