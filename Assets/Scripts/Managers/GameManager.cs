using UnityEngine;
using UnityEngine.EventSystems;
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
    
    [Header("Game Settings")]
    public bool IsGamePaused { get; private set; }
    
    [Header("UI")] 
    [SerializeField] private GameObject _mainGameUI;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject firstSelection;

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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _mainGameUI.SetActive(false);
            InputManager.Instance.SwitchToDefaultInput();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _mainGameUI.SetActive(true);
            eventSystem.firstSelectedGameObject = firstSelection;
            InputManager.Instance.SwitchToUIInput();
        }

        IsGamePaused = !IsGamePaused;
    }
}
