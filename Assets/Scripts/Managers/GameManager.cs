using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _player;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Transform _cameraPivot;

    private PlayerController _playerController;
    private Rigidbody _playerRb;
    public TileSelection currentTileSelection;
    
    [Header("Gun Controllers")]
    private TankGunController _tankGunController;
    private GunScript _gunScript;
    private FPSGunController _fpsGun;

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
        
        LinkGunComponents();
    }

    private void LinkGunComponents()
    {
        if (!_player.TryGetComponent(out TankGunController tank))
        {
            Debug.Log("Tank Gun Controller not attached to player object");
        }
        else
        {
            _tankGunController = tank;
        }

        if (!_player.TryGetComponent(out GunScript platform))
        {
            Debug.Log("Platform Gun Controller not attached to player object");
        }
        else
        {
            _gunScript = platform;
        }

        if (!_player.TryGetComponent(out FPSGunController fps))
        {
            Debug.Log("FPS Gun Controller not attached to player object");
        }
        else
        {
            _fpsGun = fps;
        }
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
        else if (_playerController.isTestingFPS)
        {
            SwitchPlayerMode(World.FPS);
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
            SwitchPlayerMode(World.FPS);
            Debug.Log("DevKey: Switching PlayerMode to FPS");
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
            case World.FPS:
                SwitchToFPS();
                break;
            case World.Mirror:
                SwitchToMirror();
                break;
            case World.Puzzle:
                SwitchToPuzzle();
                break;
        }
    }

    private void SwitchToHub()
    {
        _playerController.CurrentMode = new HubMovementMode(speed: DefaultMovementSpeed, rotationSpeed: 2f);
    }

    private void SwitchToTank()
    {
        _playerController.CurrentMode = new TankPlayerMode(speed: DefaultMovementSpeed, player: _playerController.transform, rotationSpeed: DefaultRotationSpeed, rbComponent: _playerRb, groundLayerMask: _groundLayerMask, tankGunRef: _tankGunController);
    }

    private void SwitchToPlatform()
    {
        _playerController.CurrentMode =
            new PlatformPlayerMode(playerRb: _playerRb, speed: DefaultMovementSpeed, jumpForce: 7f, playerTransform: _player.transform, gunScript: _gunScript);
    }

    private void SwitchToFPS()
    {
        _playerController.CurrentMode = new FPSPlayerMode(speed: DefaultMovementSpeed, rotationSpeed: DefaultRotationSpeed, playerTransform: _player.transform, cameraPivot: _cameraPivot, gunController: _fpsGun);
        CameraManager.Instance.TrySwitchToCamera("FPSMAIN");
    }

    private void SwitchToMirror()
    {
        _playerController.CurrentMode = new MirrorPlayerMode(rotationSpeed: 100f);
    }

    private void SwitchToPuzzle()
    {
        _playerController.CurrentMode = new PowerPuzzleMode(currentTileSelection);
        
    }
}
