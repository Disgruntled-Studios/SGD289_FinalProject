using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    private Scene _currentScene;
    private Scene? _previousScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _currentScene = SceneManager.GetActiveScene();
        _previousScene = null;
    }

    public void TransitionToScene(string sceneName, string cameraId)
    {
        StartCoroutine(TransitionRoutine(sceneName, cameraId));
    }

    // TODO: Smooth out unload operation
    private IEnumerator TransitionRoutine(string sceneName, string cameraId)
    {
        var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (loadOp != null)
        {
            yield return new WaitUntil(() => loadOp.isDone);
        }

        yield return new WaitForEndOfFrame();

        DestroyDuplicatePlayers();
        
        var loadedScene = SceneManager.GetSceneByName(sceneName);
        _previousScene = _currentScene;
        _currentScene = loadedScene;
        SetPlayerToSpawnPoint(_currentScene);

        SceneManager.SetActiveScene(_currentScene);

        yield return new WaitUntil(() => CameraManager.Instance && CameraManager.Instance.HasCamera(cameraId));

        CameraManager.Instance.TrySetCameraTarget(cameraId, GameManager.Instance.CameraTarget);
        CameraManager.Instance.TrySwitchToCamera(cameraId);

        yield return new WaitForSeconds(0.1f);

        if (_previousScene.HasValue)
        {
            var unloadOp = SceneManager.UnloadSceneAsync(_previousScene.Value);
            if (unloadOp != null)
            {
                yield return new WaitUntil(() => unloadOp.isDone);
            }
        }
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
