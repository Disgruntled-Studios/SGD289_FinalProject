using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    private string _currentSceneName;
    private string _previousSceneName;

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
        _previousSceneName = null;
        
        Debug.Log($"[Awake] Starting in scene: {_currentSceneName}");
    }

    public void TransitionToScene(string sceneName, string cameraId)
    {
        Debug.Log($"[TransitionToScene] Request to transition to '{sceneName}' from '{_currentSceneName}'");
        StartCoroutine(TransitionRoutine(sceneName, cameraId));
    }

    // TODO: Smooth out unload operation
    private IEnumerator TransitionRoutine(string sceneName, string cameraId)
    {
        Debug.Log($"[TOP] Starting transition. Current: {_currentSceneName}, Previous: {_previousSceneName}, Target: {sceneName}");

        // Load the new scene additively, only if it’s not already loaded
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (loadOp != null)
            {
                yield return new WaitUntil(() => loadOp.isDone);
            }
        }

        yield return new WaitForEndOfFrame();

        // List all loaded scenes for clarity
        Debug.Log("[LOADED SCENES]:");
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Debug.Log($"- {SceneManager.GetSceneAt(i).name}");
        }

        DestroyDuplicatePlayers();

        var loadedScene = SceneManager.GetSceneByName(sceneName);

        // LOG: before updating names
        Debug.Log($"[PRE-UPDATE] _currentSceneName: {_currentSceneName}, _previousSceneName: {_previousSceneName}");

        // Update previous name only if current is valid and different
        if (!string.IsNullOrEmpty(_currentSceneName) && _currentSceneName != sceneName)
        {
            _previousSceneName = _currentSceneName;
        }
        _currentSceneName = sceneName;

        // LOG: after updating names
        Debug.Log($"[POST-UPDATE] _currentSceneName: {_currentSceneName}, _previousSceneName: {_previousSceneName}");

        SetPlayerToSpawnPoint(loadedScene);

        SceneManager.SetActiveScene(loadedScene);

        // Wait for the new scene’s camera(s) to register
        yield return new WaitUntil(() => CameraManager.Instance && CameraManager.Instance.HasCamera(cameraId));

        CameraManager.Instance.TrySetCameraTarget(cameraId, GameManager.Instance.CameraTarget);
        CameraManager.Instance.TrySwitchToCamera(cameraId);

        yield return new WaitForSeconds(0.1f);

        // LOG: check unload block top-level condition
        Debug.Log($"[CHECK UNLOAD] _previousSceneName: {_previousSceneName}, _currentSceneName: {_currentSceneName}");

        if (!string.IsNullOrEmpty(_previousSceneName) && _previousSceneName != _currentSceneName)
        {
            Debug.Log($"[ENTERING UNLOAD BLOCK] _previousSceneName: {_previousSceneName}, _currentSceneName: {_currentSceneName}");

            var prevScene = SceneManager.GetSceneByName(_previousSceneName);
            if (prevScene.IsValid() && prevScene.isLoaded)
            {
                Debug.Log($"Unloading previous scene '{_previousSceneName}'");
                var unloadOp = SceneManager.UnloadSceneAsync(prevScene);
                if (unloadOp != null)
                {
                    yield return new WaitUntil(() => unloadOp.isDone);
                }
                else
                {
                    Debug.LogWarning($"Unload operation for '{_previousSceneName}' was null. It may already be unloaded.");
                }
            }
            else
            {
                Debug.LogWarning($"Previous scene '{_previousSceneName}' is not valid or not loaded.");
            }
        }
        else
        {
            Debug.Log("[SKIP UNLOAD] No previous scene to unload or same as current.");
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

                return;
            }
        }
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
