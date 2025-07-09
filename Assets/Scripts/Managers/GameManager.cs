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

    public bool IsGameOver { get; private set; }

    [Header("Dev Level Cheat Vars")]
    [SerializeField] private string levelOneFileName;
    [SerializeField] private string levelOneStartingCam;
    [SerializeField] private string levelTwoFileName;
    [SerializeField] private string levelTwoStartingCam;
    [SerializeField] private string levelThreeFileName;
    [SerializeField] private string levelThreeStartingCam;

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
            Debug.Log("Loading Level 3");
            TransitionManager.Instance.TransitionToScene(levelThreeFileName, levelThreeStartingCam);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.Log("Player Gun unlocked.");
            Player.GetComponent<PlayerController>().GunController.HasGun = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGameOver()
    {
        Debug.Log("StartingGameOver");
        CallAnimationPause();
        IsGameOver = true;
        Debug.Log("Invoking Reset scene");
        Invoke("ResetScene", 4f);
    }

    public void ResetScene()
    {

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            TransitionManager.Instance.TransitionToScene(levelOneFileName, levelOneStartingCam);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            TransitionManager.Instance.TransitionToScene(levelTwoFileName, levelTwoStartingCam);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            TransitionManager.Instance.TransitionToScene(levelThreeFileName, levelThreeStartingCam);
        }

        ResetEnemies();
        CallAnimationUnpause();
        IsGameOver = false;

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
            Debug.Log(animPause.name + "'s animator is set to zero speed.");
            animPause.Unpause();
        }
    }
}
