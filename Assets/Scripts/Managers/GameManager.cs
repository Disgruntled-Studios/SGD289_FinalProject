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
    public Rigidbody PlayerRb => _player.GetComponent<Rigidbody>();
    public PlayerAnimationController AnimationController => _player.GetComponent<PlayerAnimationController>();

    [SerializeField] private GameObject _groundCheckObject;
    [SerializeField] private GameObject _invCube;
    
    [SerializeField] private LayerMask _groundLayerMask;
    public TileSelection currentTileSelection;
    
    [Header("Gun Controllers")]
    [SerializeField] private TankGunController _tankGunController;
    [SerializeField] private GunScript _gunScript;
    [SerializeField] private FPSGunController _fpsGun;
    [SerializeField] private GameObject _tpGunModel;

    [FormerlySerializedAs("_playerCollisions")]
    [Header("Platformer")]
    [SerializeField] private PlatformingCollisions _platformingCollisions;


    [Header("Game Settings")]
    [SerializeField] private bool _isBulletTime; // With bullet time active, world slows down but player remains the same 
    public bool IsBulletTime => _isBulletTime;

    [Header("Colliders")] 
    [SerializeField] private CapsuleCollider _standingCollider;
    [SerializeField] private SphereCollider _crouchCollider;
    
    private const float DefaultMovementSpeed = 5f;
    private const float DefaultRotationSpeed = 10f;
    private const float HubRotationSpeed = 1f;

    public World? CurrentWorld { get; private set; } // Make current world nullable for initial world processing
    
    public bool IsInHub { get; private set; }
    public bool IsInTank { get; private set; }
    public bool IsInPlatform { get; private set; }
    public bool IsInFPS { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_player != null)
        {
            DontDestroyOnLoad(_player.gameObject);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        if (PlayerController.isTestingTank)
        {
            TransitionManager.Instance.TransitionToScene("PowerPlant", "CAM01", World.Tank);
        }
        else if (PlayerController.isTestingPlatform)
        {
            TransitionManager.Instance.TransitionToScene("Level2Mario", "PLATFORMMAIN", World.Platform);
        }
        else if (PlayerController.isTestingFPS)
        {
            TransitionManager.Instance.TransitionToScene("BJ_FPS", "FPSMAIN", World.FPS);
        }
        else
        {
            SwitchPlayerMode(World.Hub);
            TransitionManager.Instance.TransitionToScene("BJ_Hub", "HUBCAM", World.Hub);
        }
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            TransitionManager.Instance.TransitionToScene("BJ_Hub", "HUBMAIN", World.Hub);
            Debug.Log("DevKey: Switching PlayerMode to Hub");
        }
        if (Input.GetKey(KeyCode.F2))
        {
            TransitionManager.Instance.TransitionToScene("NateTest", "CAM01", World.Tank);
            Debug.Log("DevKey: Switching PlayerMode to Tank");
        }
        if (Input.GetKey(KeyCode.F3))
        {
            TransitionManager.Instance.TransitionToScene("Level2Mario", "PLATFORMMAIN", World.Platform);
            Debug.Log("DevKey: Switching PlayerMode to Platform");
        }
        if (Input.GetKey(KeyCode.F4))
        {
            TransitionManager.Instance.TransitionToScene("BJ_FPS", "FPSMAIN", World.FPS);
            Debug.Log("DevKey: Switching PlayerMode to FPS");
        }
    }

#endif

    public void SwitchPlayerMode(World mode)
    {
        PlayerController.CurrentMode = null;
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
                _tpGunModel.SetActive(false);
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
        PlayerController.CurrentMode = new HubMovementMode(speed: DefaultMovementSpeed, rotationSpeed: HubRotationSpeed, gunModel: _tpGunModel, animationController: AnimationController);
        PlayerController.CurrentMode?.OnModeEnter();
        _tankGunController.enabled = false;
        _gunScript.enabled = false;
        _fpsGun.enabled = false;
        IsInHub = true;
        IsInPlatform = false;
        IsInTank = false;
        IsInFPS = false;
    }

    private void SwitchToTank()
    {
        PlayerController.CurrentMode = new TankPlayerMode(speed: DefaultMovementSpeed, player: Player.transform, rotationSpeed: DefaultRotationSpeed, rbComponent: PlayerRb, groundLayerMask: _groundLayerMask, tankGunRef: _tankGunController, standingCollider: _standingCollider, crouchCollider: _crouchCollider, animationController: AnimationController);
        PlayerController.CurrentMode?.OnModeEnter();
        _tankGunController.enabled = true;
        _gunScript.enabled = false;
        _fpsGun.enabled = false;
        IsInHub = false;
        IsInPlatform = false;
        IsInTank = true;
        IsInFPS = false;
    }

    private void SwitchToPlatform()
    {
        PlayerController.CurrentMode =
            new PlatformPlayerMode(playerRb: PlayerRb, speed: DefaultMovementSpeed, jumpForce: 10f, playerTransform: _player.transform, gunScript: _gunScript, platformingCollisions: _platformingCollisions, gunModel: _tpGunModel, groundCheck: _groundCheckObject, invCube: _invCube, animationController: AnimationController);
        PlayerController.CurrentMode?.OnModeEnter();
        _gunScript.enabled = true;
        _tankGunController.enabled = false;
        _fpsGun.enabled = false;
        IsInHub = false;
        IsInPlatform = true;
        IsInTank = false;
        IsInFPS = false;
    }

    private void SwitchToFPS()
    {
        PlayerController.CurrentMode = new FPSPlayerMode(speed: DefaultMovementSpeed, playerTransform: _player.transform, cameraPivot: _cameraTarget, gunController: _fpsGun, isBulletTime: _isBulletTime, playerRb: PlayerRb, standingCollider: _standingCollider, crouchingCollider: _crouchCollider, animationController: AnimationController, groundLayer: _groundLayerMask);
        PlayerController.CurrentMode?.OnModeEnter();
        _fpsGun.enabled = true;
        _tankGunController.enabled = false;
        _gunScript.enabled = false;
        IsInHub = false;
        IsInPlatform = false;
        IsInTank = false;
        IsInFPS = true;
    }

    private void SwitchToMirror()
    {
        PlayerController.CurrentMode = new MirrorPlayerMode(rotationSpeed: 100f);
        PlayerController.CurrentMode?.OnModeEnter();
        _tankGunController.enabled = false;
        _gunScript.enabled = false;
        _fpsGun.enabled = false;
    }

    private void SwitchToPuzzle()
    {
        PlayerController.CurrentMode = new PowerPuzzleMode(currentTileSelection);
        PlayerController.CurrentMode?.OnModeEnter();
        _tankGunController.enabled = false;
        _gunScript.enabled = false;
        _fpsGun.enabled = false;
    }
}
