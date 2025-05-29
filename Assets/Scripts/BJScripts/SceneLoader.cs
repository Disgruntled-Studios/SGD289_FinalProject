using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private string _sceneName = "BJ_Stealth";
    [SerializeField] private string _cameraId = "STEALTHCENTER";
    [SerializeField] private string _sceneToUnloadName = "BJ_Hub";

    public Transform screenCenter;
    public float suckDuration = 1.5f;
    public AnimationCurve scaleCurve;
    public AnimationCurve speedCurve;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Loading level");
            LoadLevel();
        }
    }
    
    public void LoadLevel()
    {
        StartCoroutine(LoadLevelRoutine());
    }

    private IEnumerator LoadLevelRoutine()
    {
        yield return StartCoroutine(SuckPlayerIntoScreen());
        
        var loadOp = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadOp.isDone);
        
        yield return new WaitForEndOfFrame();

        var retries = 10;
        while (!CameraManager.Instance.TrySwitchToCamera(_cameraId) && retries-- > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Unload previous level 
        // var unloadOp = SceneManager.UnloadSceneAsync(_sceneToUnloadName);
        // yield return new WaitUntil(() => unloadOp.isDone);
        
        if (retries <= 0)
        {
            Debug.Log("Failed to switch cameras");
        }
    }

    private IEnumerator SuckPlayerIntoScreen()
    {
        var startPos = _player.transform.position;
        var targetPos = screenCenter.position;
        var startScale = _player.transform.localScale;

        var timer = 0f;

        while (timer < suckDuration)
        {
            var t = timer / suckDuration;

            _player.transform.position = Vector3.Lerp(startPos, targetPos, speedCurve.Evaluate(t));
            _player.transform.localScale = startScale * scaleCurve.Evaluate(t);

            timer += Time.deltaTime;
            yield return null;
        }

        _player.transform.localScale = Vector3.zero;
    }
}
