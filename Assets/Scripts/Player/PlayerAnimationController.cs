using System;
using TMPro;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _anim;
    [SerializeField] private AnimatorOverrideController _injuredOverrideController;

    private RuntimeAnimatorController _originalController;
    private float _currentAnimSpeed;
    
    private const float TurnThreshold = 0.1f;

    private void Awake()
    {
        _originalController = _anim.runtimeAnimatorController;
    }
    
    private void OnEnable()
    {
        _anim.speed = 1f;
    }

    private void OnDisable()
    {
        _anim.speed = 0f;
    }

    private void Update()
    {
        if (!_playerController) return;

        var forwardInput = Mathf.Clamp(_playerController.CurrentMoveInput, -1f, 1f);
        _currentAnimSpeed = Mathf.MoveTowards(_currentAnimSpeed, forwardInput, 5f * Time.deltaTime);
        _anim.SetFloat("MoveSpeed", _currentAnimSpeed);

        var turnInput = Mathf.Clamp(_playerController.GetCurrentTurnInput(), -1f, 1f);

        if (turnInput < -TurnThreshold)
        {
            _anim.SetBool("IsTurningLeft", true);
            _anim.SetBool("IsTurningRight", false);
        }
        else if (turnInput > TurnThreshold)
        {
            _anim.SetBool("IsTurningLeft", false);
            _anim.SetBool("IsTurningRight", true);
        }
        else
        {
            _anim.SetBool("IsTurningLeft", false);
            _anim.SetBool("IsTurningRight", false);
        }
    }

    public void Crouch(bool isCrouching)
    {
        _anim.SetBool("IsCrouching", isCrouching);
    }

    public void SetGrounded(bool isGrounded)
    {
        _anim.SetBool("IsGrounded", isGrounded);
    }

    public void Jump()
    {
        _anim.SetTrigger("Jump");
    }

    public void Aim(bool isAiming)
    {
        _anim.SetBool("IsAiming", isAiming);
    }

    public void Sprint(bool isSprinting)
    {
        _anim.SetBool("IsSprinting", isSprinting);
    }

    public void Shoot()
    {
        _anim.SetTrigger("ShootTrigger");
    }

    public void SetInjured(bool isInjured)
    {
        _anim.runtimeAnimatorController = isInjured ? _injuredOverrideController : _originalController;
    }
}

