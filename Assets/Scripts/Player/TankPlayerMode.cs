using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
    private float groundDrag = 8f;
    private float playerHeight = 2f;
    LayerMask _groundLayerMask;
    CapsuleCollider _standingCollider;
    SphereCollider _crouchCollider;

    private TankGunController _tankGunReference;
    private PlayerAnimationController _animationController;
    private LineRenderer _laser;

    private float _currentMoveInput;
    private const float AccelerationRate = 10f;
    private const float DecelerationRate = 15f;

    private float _currentRotationInput;
    private const float RotationAccelerationRate = 8f;
    private const float RotationDecelerationRate = 10f;
    private const float RotationDeadZone = 0.05f;

    private readonly float _aimRotationSpeedMultiplier = 0.25f;
    
    /// <summary>
    /// The TankPlayerMode will be the movement system based on the orientation of the player model not camera.
    /// </summary>
    /// <param name="speed">How fast the player can go.</param>
    /// <param name="player">The transform of the player object.</param>
    /// <param name="rotationSpeed">How fast the player will rotate the character model.</param>
    /// <param name="rbComponent">The Rigidbody component that is attached to the player object.</param>
    /// <param name="groundLayerMask">The Rigidbody component that is attached to the player object.</param>
    public TankPlayerMode(float speed, Transform player, float rotationSpeed, Rigidbody rbComponent, LayerMask groundLayerMask, TankGunController tankGunRef, CapsuleCollider standingCollider, SphereCollider crouchCollider, PlayerAnimationController animationController, LineRenderer laser)
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
        _laser = laser;
    }


    public void Move(Rigidbody rb, float input, Transform context)
    {
        if (InputManager.Instance.IsInPuzzle) return;

        var targetSpeed = _currentSpeed;

        if (_tankGunReference.isReloading || _isCrouching)
        {
            targetSpeed = _halfSpeed;
        }
        else
        {
            targetSpeed = _normalSpeed;
        }

        // Smooth acceleration
        if (Mathf.Abs(input) > 0.01f)
        {
            _currentMoveInput = Mathf.MoveTowards(_currentMoveInput, input, Time.deltaTime * AccelerationRate);
        }
        else
        {
            _currentMoveInput = Mathf.MoveTowards(_currentMoveInput, 0f, Time.deltaTime * DecelerationRate);
        }

        var targetVelocity = context.forward * (_currentMoveInput * targetSpeed);
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;
    }

    public void Rotate(float input, Transform context)
    {
        if (InputManager.Instance.IsInPuzzle) return;

        if (Mathf.Abs(input) > 0.01f)
        {
            _currentRotationInput =
                Mathf.MoveTowards(_currentRotationInput, input, Time.deltaTime * RotationAccelerationRate);
        }
        else
        {
            _currentRotationInput =
                Mathf.MoveTowards(_currentRotationInput, 0f, Time.deltaTime * RotationDecelerationRate);
        }

        if (Mathf.Abs(_currentRotationInput) < RotationDeadZone)
        {
            _currentRotationInput = 0f;
        }

        if (!Mathf.Approximately(_currentRotationInput, 0f))
        {
            var rotationAmount = _currentRotationInput * currentRotationSpeed * Time.deltaTime;
            var deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
            _rb.MoveRotation(_rb.rotation * deltaRotation);
        }
    }

    private void SetRotationSpeedForAim(bool isAiming)
    {
        currentRotationSpeed = isAiming ? _rotationSpeed * _aimRotationSpeedMultiplier : _rotationSpeed;
    }

    public void Look(Vector2 input, Transform context)
    {
        
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
            _animationController.Aim(true);
            SetRotationSpeedForAim(true);
        }
        
        if (context.canceled)
        {
            _tankGunReference.EndGunAim();
            _animationController.Aim(false);
            SetRotationSpeedForAim(false);
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
        _laser.enabled = true;
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
