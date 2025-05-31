using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _player;
    [SerializeField] private LayerMask _groundLayerMask;
    
    private PlayerController _playerController;
    private Rigidbody _playerRb;
    private GunFunctions _gunFunctions;
    
    private const float DefaultMovementSpeed = 5f;
    private const float DefaultRotationSpeed = 10f;
    
    public World? CurrentWorld { get; private set; } // Make current world nullable for initial world processing

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _playerController = _player.GetComponent<PlayerController>();
        _playerRb = _player.GetComponent<Rigidbody>();
        _gunFunctions = _player.GetComponent<GunFunctions>();

    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        if (_playerController.isTestingTank)
        {
            SwitchPlayerMode(World.Tank);
        }
        else if (_playerController.isTestingPlatform)
        {
            SwitchPlayerMode(World.Platform);
        }
        else if (_playerController.isTestingStealth)
        {
            SwitchPlayerMode(World.Stealth);
        }
        else
        {
            SwitchPlayerMode(World.Hub);
        }
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            SwitchPlayerMode(World.Hub);
            Debug.Log("DevKey: Switching PlayerMode to Hub");
        }
        if (Input.GetKey(KeyCode.F2))
        {
            SwitchPlayerMode(World.Tank);
            Debug.Log("DevKey: Switching PlayerMode to Tank");
        }
        if (Input.GetKey(KeyCode.F3))
        {
            SwitchPlayerMode(World.Platform);
            Debug.Log("DevKey: Switching PlayerMode to Platform");
        }
        if (Input.GetKey(KeyCode.F4))
        {
            SwitchPlayerMode(World.Stealth);
            Debug.Log("DevKey: Switching PlayerMode to Stealth");
        }
    }

#endif

    public void SwitchPlayerMode(World mode)
    {
        if (CurrentWorld == mode) return;
        CurrentWorld = mode;

        switch (mode)
        {
            case World.Hub:
                SwitchToHub();
                break;
            case World.Tank:
                SwitchToTank();
                break;
            case World.Platform:
                SwitchToPlatform();
                break;
            case World.Stealth:
                SwitchToStealth();
                break;
            case World.Mirror:
                SwitchToMirror();
                break;
        }
    }

    private void SwitchToHub()
    {
        _playerController.CurrentMode = new HubMovementMode(speed: DefaultMovementSpeed, rotationSpeed: DefaultRotationSpeed);
    }

    private void SwitchToTank()
    {
        _playerController.CurrentMode = new TankPlayerMode(speed: DefaultMovementSpeed, player: _playerController.transform, rotationSpeed: DefaultRotationSpeed, rbComponent: _playerRb, groundLayerMask: _groundLayerMask, gunRef: _gunFunctions);
    }

    private void SwitchToPlatform()
    {
        _playerController.CurrentMode =
            new PlatformPlayerMode(playerRb: _playerRb, speed: DefaultMovementSpeed, jumpForce: 7f, playerTransform: _player.transform);
    }

    private void SwitchToStealth()
    {
        _playerController.CurrentMode = new StealthPlayerMode(speed: DefaultMovementSpeed, rotationSpeed: DefaultRotationSpeed, playerTransform: _player.transform);
    }

    private void SwitchToMirror()
    {
        _playerController.CurrentMode = new MirrorPlayerMode(rotationSpeed: 100f);
    }
}
