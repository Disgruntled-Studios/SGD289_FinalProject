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

    private readonly float _rotationSpeed;
    private float _currentRotationSpeed;
    
    private bool _isGrounded;
    private bool _isCrouching;

    private const float GroundDrag = 5f;
    private const float PlayerHeight = 2f;

    private readonly LayerMask _groundLayerMask;
    private readonly CapsuleCollider _standingCollider;
    private readonly SphereCollider _crouchCollider;

    private readonly TankGunController _tankGunReference;
    private readonly PlayerAnimationController _animationController;
    private readonly LineRenderer _laser;

    private float _currentMoveInput;
    private float _currentRotationInput;

    private const float RotationAccelerationRate = 8f;
    private const float RotationDecelerationRate = 10f;
    private const float RotationDeadZone = 0.05f;
    private const float AimRotationSpeedMultiplier = 0.25f;

    private const float MoveResponsiveness = 10f;
    private const float StopResponsiveness = 15f;

    private bool _forcedInitialVelocityApplied;
    
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
        _currentRotationSpeed = rotationSpeed;
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
        
        _currentMoveInput = input;
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
    }

    private void SetRotationSpeedForAim(bool isAiming)
    {
        _currentRotationSpeed = isAiming ? _rotationSpeed * AimRotationSpeedMultiplier : _rotationSpeed;
    }

    private void UpdateSpeedBasedOnState()
    {
        _currentSpeed = (_tankGunReference.isReloading || _isCrouching) ? _halfSpeed : _normalSpeed;
    }

    public void Look(Vector2 input, Transform context)
    {
        // Camera controls? 
    }

    public void Jump()
    {
        return;
    } 

    public void Crouch(bool isPressed)
    {
        if (InputManager.Instance.IsInPuzzle) return;

        if (_standingCollider.enabled)
        {
            _isCrouching = true;
            _animationController.Crouch(_isCrouching);
            _currentSpeed = _halfSpeed;
            _standingCollider.enabled = false;
            _crouchCollider.enabled = true;
            Debug.Log(_isCrouching);
        }
        else if (!Physics.Raycast(_player.TransformPoint(_crouchCollider.center), Vector3.up, out var hitTest, 1)) 
        {
            _isCrouching = false;
            _animationController.Crouch(_isCrouching);
            _currentSpeed = _normalSpeed;
            _standingCollider.enabled = true;
            _crouchCollider.enabled = false;
            Debug.Log("Not Crouching");
        }
        
        UpdateSpeedBasedOnState();
    }
    public void Tick()
    {
        if (InputManager.Instance.IsInPuzzle) return;

        _isGrounded = Physics.Raycast(_player.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, _groundLayerMask);
        //Debug.Log("IsGrounded = " + _isGrounded);
        //_rb.linearDamping = _isGrounded ? GroundDrag : 0f;

        // Apply rotation
        if (!Mathf.Approximately(_currentRotationInput, 0f))
        {
            var rotationAmount = _currentRotationInput * _currentRotationSpeed * Time.deltaTime;
            var deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
            _rb.MoveRotation(_rb.rotation * deltaRotation);
        }
        
        // Apply movement
        var targetVelocity = _rb.transform.forward * (_currentMoveInput * _currentSpeed);

        if (_currentMoveInput != 0f)
        {
            if (_rb.linearVelocity.magnitude < 0.01f && !_forcedInitialVelocityApplied)
            {
                _rb.linearVelocity = targetVelocity;
                _forcedInitialVelocityApplied = true;
            }
            else
            {
                _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, targetVelocity, Time.deltaTime * MoveResponsiveness);
            }
        }
        else
        {
            _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, Vector3.zero, Time.deltaTime * StopResponsiveness);
        }
        
        // if (Mathf.Approximately(_currentMoveInput, 0f))
        // {
        //     _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, Vector3.zero, Time.deltaTime * StopResponsiveness);
        // }
        // else
        // {
        //     _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, targetVelocity, Time.deltaTime * MoveResponsiveness);
        // }

        Debug.Log(_rb.linearVelocity.z);
        
        Debug.DrawLine(_player.position, _player.position + Vector3.down * (PlayerHeight * 0.5f + 0.2f), Color.blue);
        Debug.DrawLine(_player.TransformPoint(_crouchCollider.center),
            _player.TransformPoint(_crouchCollider.center) + Vector3.up, Color.blue);
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.IsInPuzzle) return;
        
        if (context.started)
        {
            _tankGunReference.StartGunAim();
            _animationController.Aim(true);
            SetRotationSpeedForAim(true);
            UpdateSpeedBasedOnState();
        }
        
        if (context.canceled)
        {
            _tankGunReference.EndGunAim();
            _animationController.Aim(false);
            SetRotationSpeedForAim(false);
            UpdateSpeedBasedOnState();
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
        
        _rb.WakeUp();
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.linearDamping = GroundDrag;

        _currentSpeed = _normalSpeed;
        _forcedInitialVelocityApplied = false;
        
        Physics.SyncTransforms();
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
