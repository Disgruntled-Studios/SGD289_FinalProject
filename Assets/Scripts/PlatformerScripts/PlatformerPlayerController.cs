using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerPlayerController : MonoBehaviour
{

    public UnitHealth playerHealth;
    [SerializeField] private float maxHealth; 
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private bool _isCrouching; //decided to keep crouching and shooting for platformer.

    //Plans for script writing:
    //plan to add jumping
    //add health and scoring as well, though scoring in another script perhaps.
    //player can destroy small enemies by jumping on them (which causes player to bounce and bounce higher if they click jump button at same time)
    //if time, will also adding kicking which allows player to kick enemies or objects
    //if player destroys a lightning ship, the enemy will disappear or fall off and player can jump onto their ship
    //and ride it for 20 (adjustable in outliner) seconds before it flashes and disappears.
    //While riding ship, shooting button instead fires a lazer beam forwards same as ship enemy in firing mode 
    //Enemy ships in the game are either in dive bomb mode or firing mode. Dive bomb ships dive straight at player and can travel through platforms and other terrain.
    //They are also invincible in this mode, but in firing mode, they travel along where player is and also can travel through platforms and other terrain. 
    //Player cannot dive through terrain while riding them though and perhaps cannot shoot them while they are in terrain (error check later)
    //Player can access some otherwise inaccessible locations in the sky while riding the ship.
    //Health system. Heath is represented by 3 astronaut helmets in top left corner of screen. There is a score in top right.
    //Player collects alien coins throughout the level that loook like mini spaceships.

    private Rigidbody _rb;

    public IPlayerMode CurrentMode { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        playerHealth = new UnitHealth(maxHealth);
    }

    public void OnMove(InputAction.CallbackContext context) => _movementInput = context.ReadValue<Vector2>();
    public void OnRotate(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

    public void OnAim(InputAction.CallbackContext context)
    {
        CurrentMode?.Aim(context);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CurrentMode?.Attack();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CurrentMode?.Jump();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isCrouching = !_isCrouching;
            CurrentMode?.Crouch(_isCrouching);
        }
    }

    private void FixedUpdate()
    {
        CurrentMode?.Move(_rb, _movementInput, transform);
    }

    private void Update()
    {
        CurrentMode?.Rotate(_lookInput);
        CurrentMode?.Tick();
    }
}

