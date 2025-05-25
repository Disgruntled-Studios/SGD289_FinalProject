using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _player;
    private PlayerController _playerController;
    private Rigidbody _playerRb;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _playerController = _player.GetComponent<PlayerController>();
        _playerRb = _player.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        SwitchPlayerMode(World.Hub);
    }

    public void SwitchPlayerMode(World mode)
    {
        switch (mode)
        {
            case World.Hub:
                SwitchToHub();
                break;
            case World.FPS:
                SwitchToFPS();
                break;
            case World.Platform:
                SwitchToPlatform();
                break;
            case World.Stealth:
                SwitchToStealth();
                break;
        }
    }

    private void SwitchToHub()
    {
        _playerController.CurrentMode = new HubMovementMode(speed: 5f, rotationSpeed: 10f);
        CameraManager.Instance.SwitchTo(World.Hub);
    }

    private void SwitchToFPS()
    {
        _playerController.CurrentMode = new FPSPlayerMode(speed: 6f, player: _playerController.transform);
        CameraManager.Instance.SwitchTo(World.FPS);
    }

    private void SwitchToPlatform()
    {
        _playerController.CurrentMode =
            new PlatformPlayerMode(playerRb: _playerRb, speed: 5f, jumpForce: 7f, playerTransform: _player.transform);
        CameraManager.Instance.SwitchTo(World.Platform);
    }

    private void SwitchToStealth()
    {
        _playerController.CurrentMode = new StealthPlayerMode(speed: 3.5f, playerTransform: _player.transform);
        CameraManager.Instance.SwitchTo(World.Stealth);
    }
}
