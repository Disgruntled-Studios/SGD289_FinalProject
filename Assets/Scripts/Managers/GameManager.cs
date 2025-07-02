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
