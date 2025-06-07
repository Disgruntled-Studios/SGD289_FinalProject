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

        _currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void TransitionToScene(string sceneName, string cameraId)
    {
        StartCoroutine(TransitionRoutine(sceneName, cameraId));
    }

    private IEnumerator TransitionRoutine(string sceneName, string cameraId)
    {
        var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadOp.isDone);

        yield return new WaitForEndOfFrame();

        var loadedScene = SceneManager.GetSceneByName(sceneName);
        
        DestroyDuplicatePlayers();
        
        SetPlayerToSpawnPoint(loadedScene);

        var retries = 10;
        while (!CameraManager.Instance.TrySwitchToCamera(cameraId) && retries-- > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (retries <= 0)
        {
            Debug.Log("failed to switch cameras");
        }

        if (_currentSceneName != sceneName)
        {
            var unloadOp = SceneManager.UnloadSceneAsync(_currentSceneName);
            yield return new WaitUntil(() => unloadOp.isDone);
        }

        _currentSceneName = sceneName;
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
