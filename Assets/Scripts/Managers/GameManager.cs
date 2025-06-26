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

    [Header("Game Settings")]
    public bool IsGamePaused { get; private set; }
    
    [Header("UI")]
    [SerializeField] private GameObject _pausePanel;
    [FormerlySerializedAs("_eventSystem")] [SerializeField] private EventSystem _gameEventSystem;
    public EventSystem GameEventSystem => _gameEventSystem;
    [SerializeField] private GameObject _firstSelection;

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
            TransitionManager.Instance.TransitionToScene("L1PowerPlant", "CAM11");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            TransitionManager.Instance.TransitionToScene("L2MaintenanceTunnel", "CAM21");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            TransitionManager.Instance.TransitionToScene("L3Reactor", "CAM31");
        }
    }

    public void OpenPauseMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _pausePanel.SetActive(true);
        IsGamePaused = true;

        StartCoroutine(EnablePauseMenuNextFrame());
    }

    public void ClosePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _pausePanel.SetActive(false);
        IsGamePaused = false;
        
        InputManager.Instance.SwitchToDefaultInput();
    }

    private IEnumerator EnablePauseMenuNextFrame()
    {
        yield return null;

        _gameEventSystem.SetSelectedGameObject(null);
        _gameEventSystem.SetSelectedGameObject(_firstSelection);
        
        InputManager.Instance.SwitchToUIInput();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGameOver()
    {
        Invoke("ResetScene", 4f);
    }

    public void ResetScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
        Destroy(TransitionManager.Instance.gameObject);
        Destroy(_player);
        Destroy(UIManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        Destroy(CameraManager.Instance.gameObject);
        Destroy(gameObject);
    }
}
