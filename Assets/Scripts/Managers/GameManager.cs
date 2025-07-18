using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject _player;
    public GameObject Player => _player;
    [SerializeField] private Transform _cameraTarget;
    public Transform CameraTarget => _cameraTarget;
    public PlayerController PlayerController => _player.GetComponent<PlayerController>();
    public PlayerInventory PlayerInventory => _player.GetComponent<PlayerInventory>();
    public RumbleController RumbleController => _player.GetComponent<RumbleController>();
    public GameObject gameOverScreen;

    public bool IsGameOver { get; private set; }

    [Header("Dev Level Cheat Vars")]
    [SerializeField] private string levelOneFileName;
    [SerializeField] private string levelOneStartingCam;
    [SerializeField] private string levelTwoFileName;
    [SerializeField] private string levelTwoStartingCam;

    [SerializeField] private bool _isTesting = true;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_player)
        {
            DontDestroyOnLoad(_player.gameObject);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        if (_isTesting)
        {
            Player.GetComponent<PlayerController>().GunController.HasGun = true;
            
        }
        else
        {
                        
            TransitionManager.Instance.SetPlayerToSpawnPoint(SceneManager.GetActiveScene());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("Loading Level 1");
            TransitionManager.Instance.TransitionToScene(levelOneFileName, levelOneStartingCam);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Loading Level 2");
            TransitionManager.Instance.TransitionToScene(levelTwoFileName, levelTwoStartingCam);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("Player Gun unlocked.");
            Player.GetComponent<PlayerController>().GunController.HasGun = true;
        }
    }

    public void ReturnToMainMenu()
    {
        IsGameOver = false;
        CallAnimationUnpause();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UIManager.Instance.CloseGameOverScreen();
        UIManager.Instance.DestroyUI();
        Destroy(Player);
        Destroy(CameraManager.Instance.gameObject);
        Destroy(SelectionManager.Instance.gameObject);
        Destroy(TransitionManager.Instance.gameObject);

        SceneManager.LoadScene("Scenes/GOLD_FINAL/MainMenu");
        Destroy(gameObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGameOver()
    {
        CallAnimationPause();
        CallFadeOutOnGameOver();
        IsGameOver = true;

        UIManager.Instance.OpenGameOverScreen();
    }

    public void ResetScene()
    {
        var activeIndex = SceneManager.GetActiveScene().buildIndex;

        if (activeIndex == 1)
        {
            TransitionManager.Instance.TransitionToScene(levelOneFileName, levelOneStartingCam);
        }
        else if (activeIndex == 2)
        {
            TransitionManager.Instance.TransitionToScene(levelTwoFileName, levelTwoStartingCam);
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        UIManager.Instance.CloseGameOverScreen();
        Player.GetComponent<PlayerHealth>().ResetVignette();
        ResetEnemies();
        CallAnimationUnpause();
        CallFadeInOnGameOver();
        IsGameOver = false;
        PlayerController.IsMovementLocked = false;
        UIManager.Instance.ActivateHudPanel();
    }

    public void ResetEnemies()
    {
        EnemyBehavior[] enemies = FindObjectsByType<EnemyBehavior>(FindObjectsSortMode.None);

        foreach (EnemyBehavior enemy in enemies)
        {
            Debug.Log(enemy + " is in the list");
            if (enemy.currentState == EnemyBehavior.BehaviorState.chasing)
            {
                enemy.currentState = EnemyBehavior.BehaviorState.patrolling;
            }
        }
    }

    public void CallAnimationPause()
    {
        AnimationPause[] animPauses = FindObjectsByType<AnimationPause>(FindObjectsSortMode.None);

        foreach (AnimationPause animPause in animPauses)
        {
            Debug.Log(animPause.name + "'s animator is set to zero speed.");
            animPause.Pause();
        }
    }

    public void CallAnimationUnpause()
    {
        AnimationPause[] animPauses = FindObjectsByType<AnimationPause>(FindObjectsSortMode.None);

        foreach (AnimationPause animPause in animPauses)
        {
            Debug.Log(animPause.name + "'s animator is set to one speed.");
            animPause.Unpause();
        }
    }

    public void CallFadeInOnGameOver()
    {
        FadeOnGameOver[] fadeObjs = FindObjectsByType<FadeOnGameOver>(FindObjectsSortMode.None);

        foreach (FadeOnGameOver fader in fadeObjs)
        {
            fader.StartCoroutine(fader.FadeSoundIn());
        }
    }

    public void CallFadeOutOnGameOver()
    {
        FadeOnGameOver[] fadeObjs = FindObjectsByType<FadeOnGameOver>(FindObjectsSortMode.None);

        foreach (FadeOnGameOver fader in fadeObjs)
        {
            fader.StartCoroutine(fader.FadeSoundOut());
        }
    }
}
