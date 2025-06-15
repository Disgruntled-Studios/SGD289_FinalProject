using UnityEngine;
using UnityEngine.InputSystem;

public class TankPlayerMode : IPlayerMode
{
    private readonly float _normalSpeed;
    private readonly float _halfSpeed;
    private float _currentSpeed;
    private readonly Transform _player;
    private readonly Rigidbody _rb;
    public float _rotationSpeed;
    private float currentRotationSpeed;
    //private bool _isAiming;
    private bool isGrounded;
    private bool _isCrouching;
    private float groundDrag = 5f;
    private float playerHeight = 2f;
    LayerMask _groundLayerMask;
    CapsuleCollider _standingCollider;
    SphereCollider _crouchCollider;

    private TankGunController _tankGunReference;
    private PlayerAnimationController _animationController;


    /// <summary>
    /// The TankPlayerMode will be the movement system based on the orientation of the player model not camera.
    /// </summary>
    /// <param name="speed">How fast the player can go.</param>
    /// <param name="player">The transform of the player object.</param>
    /// <param name="rotationSpeed">How fast the player will rotate the character model.</param>
    /// <param name="rbComponent">The Rigidbody component that is attached to the player object.</param>
    /// <param name="groundLayerMask">The Rigidbody component that is attached to the player object.</param>
    public TankPlayerMode(float speed, Transform player, float rotationSpeed, Rigidbody rbComponent, LayerMask groundLayerMask, TankGunController tankGunRef, CapsuleCollider standingCollider, SphereCollider crouchCollider, PlayerAnimationController animationController)
    {
        _normalSpeed = speed;
        _halfSpeed = speed / 2f;
        _currentSpeed = speed;
        _player = player;
        _rotationSpeed = rotationSpeed;
        currentRotationSpeed = rotationSpeed;
        _rb = rbComponent;
        _groundLayerMask = groundLayerMask;
        _tankGunReference = tankGunRef;
        _standingCollider = standingCollider;
        _crouchCollider = crouchCollider;
        _crouchCollider.enabled = false;
        _animationController = animationController;
    }


    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        if (InputManager.Instance.IsInPuzzle) return;

        if (_tankGunReference.isReloading)
        {
            _currentSpeed = _halfSpeed;
        }
        else if (!_tankGunReference.isReloading && _currentSpeed == _halfSpeed && !_isCrouching)
        {
            _currentSpeed = _normalSpeed;
        }

        rb.AddForce(context.forward * (input.y * _currentSpeed * 10f), ForceMode.Force);
        
        if (input.x != 0 && InputManager.Instance.IsUsingKeyboard)
        {
            context.transform.Rotate(new Vector3(0, input.x, 0) * currentRotationSpeed);
        }
        else
        {
            _rb.angularVelocity = Vector3.zero;
        }

    }

    public void ToggleRotationSpeed()
    {
        if (InputManager.Instance.IsInPuzzle) return;
        if (Mathf.Approximately(currentRotationSpeed, _rotationSpeed))
        {
            currentRotationSpeed *= 0.25f;
        }
        else
        {
            currentRotationSpeed = _rotationSpeed;
        }
    }

    public void Rotate(Vector2 input, Transform context)
    {
        if (input.x != 0 && InputManager.Instance.IsUsingController)
        {
            context.transform.Rotate(new Vector3(0, input.x, 0) * currentRotationSpeed);
        }
        else
        {
            _rb.angularVelocity = Vector3.zero;
        }
    }

    public void Jump()
    {
        return;
    } // Not used in Tank

    public void Crouch(bool isPressed)
    {

        if (InputManager.Instance.IsInPuzzle) return;

        RaycastHit hitTest;

        

        if (_standingCollider.enabled)
        {
            _isCrouching = true;
            _animationController.Crouch(_isCrouching);
            _currentSpeed = _halfSpeed;
            _standingCollider.enabled = false;
            _crouchCollider.enabled = true;
            Debug.Log(_isCrouching);
        }
        else if (!Physics.Raycast(_player.TransformPoint(_crouchCollider.center), Vector3.up, out hitTest, 1)) 
        {
            _isCrouching = false;
            _animationController.Crouch(_isCrouching);
            _currentSpeed = _normalSpeed;
            _standingCollider.enabled = true;
            _crouchCollider.enabled = false;
            Debug.Log("Not Crouching");
        }
    }
    public void Tick()
    {
        if (InputManager.Instance.IsInPuzzle) return;
        //Each frame we check if the player is grounded or not.
        isGrounded = Physics.Raycast(_player.position, Vector3.down, playerHeight * 0.5f + 0.2f, _groundLayerMask);
        Debug.DrawLine(_player.position, new Vector3(_player.position.x, (-1 * playerHeight * 0.5f + 0.2f), _player.position.z), Color.blue);
        Debug.DrawLine(_player.TransformPoint(_crouchCollider.center), new Vector3(_player.TransformPoint(_crouchCollider.center).x,_player.TransformPoint(_crouchCollider.center).y + 1, _player.TransformPoint(_crouchCollider.center).z), Color.blue);

        //If the player is grounded and has Velocity stored on the Y axis we reset their vertical velocity.X
        if (isGrounded)
        {
            _rb.linearDamping = groundDrag;
        }
        else
        {
            _rb.linearDamping = 0;
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.IsInPuzzle) return;
        if (context.started)
        {
            _tankGunReference.StartGunAim();
            ToggleRotationSpeed();
        }
        
        if (context.canceled)
        {
            _tankGunReference.EndGunAim();
            ToggleRotationSpeed();
        }
    }

    public void Attack()
    {
        if (InputManager.Instance.IsInPuzzle) return;
        _tankGunReference.HandleShoot();
    }

    public void Special()
    {
        return;
    }
    
    public void OnModeEnter()
    {
        return;
    }
    
    public void OnModeExit()
    {
        return;
    }
    
    public void Sprint(InputAction.CallbackContext context)
    {
        return;
    }
}
