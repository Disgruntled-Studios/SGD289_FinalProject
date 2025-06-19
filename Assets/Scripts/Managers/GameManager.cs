using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player")] 
    [SerializeField] private GameObject _player;
    public GameObject Player => _player;
    [SerializeField] private Transform _cameraTarget;
    public Transform CameraTarget => _cameraTarget;
    public PlayerController PlayerController => _player.GetComponent<PlayerController>();
    
    [Header("Game Settings")]
    public bool IsGamePaused { get; private set; }

    [Header("UI")] 
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _settingsMenuUI;

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

    public void TogglePauseGame()
    {
        if (InputManager.Instance.IsInPuzzle) return;

        if (IsGamePaused)
        {
            _pauseMenuUI.SetActive(false);
            _settingsMenuUI.SetActive(false);
            InputManager.Instance.SwitchToDefaultInput();
        }
        else
        {
            _pauseMenuUI.SetActive(true);
            _settingsMenuUI.SetActive(false);
            InputManager.Instance.SwitchToUIInput();
        }

        IsGamePaused = !IsGamePaused;
    }
}
