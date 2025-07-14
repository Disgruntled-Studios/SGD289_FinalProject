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

    [SerializeField] private AudioSource _footsteps;
    [SerializeField] private AudioSource _shuffleStep;
    
    private const float TurnThreshold = 0.1f;

    private int _lastStateHash = -1;
    private float _lastNormalizedTime = -1f;
    [SerializeField] private float _defaultMinStepGap = 0.3f;
    
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
        if (!_playerController || GameManager.Instance.IsGameOver) return;

        var forwardInput = Mathf.Clamp(_playerController.CurrentMoveInput, -1f, 1f);
        _currentAnimSpeed = Mathf.MoveTowards(_currentAnimSpeed, forwardInput, 5f * Time.deltaTime);
        _anim.SetFloat("MoveSpeed", _currentAnimSpeed);

        var turnInput = Mathf.Clamp(_playerController.GetCurrentTurnInput(), -1f, 1f);

        if (Mathf.Abs(_currentAnimSpeed) < 0.05f)
        {
            if (turnInput < -0.1f)
            {
                _anim.SetBool("IsTurningLeft", true);
                _anim.SetBool("IsTurningRight", false);
            }
            else if (turnInput > 0.1f)
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

    public void TriggerHit()
    {
        _anim.SetTrigger("Hit");
    }

    public void SetInjured(bool isInjured)
    {
        _anim.runtimeAnimatorController = isInjured ? _injuredOverrideController : _originalController;
    }

    public void PlayFootstepSound()
    {
        var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        var currentStateHash = stateInfo.shortNameHash;
        var normalizedTime = stateInfo.normalizedTime % 1;

        var minGap = GetMinStepGapForState(currentStateHash);

        if (_lastStateHash == currentStateHash && Mathf.Abs(normalizedTime - _lastNormalizedTime) < minGap) return;

        _lastStateHash = currentStateHash;
        _lastNormalizedTime = normalizedTime;

        if (_footsteps?.clip != null)
        {
            if (_shuffleStep.isPlaying)
            {
                _shuffleStep.Stop();
            }
            
            _footsteps.PlayOneShot(_footsteps.clip);
        }
    }

    public void PlayShuffleStep()
    {
        if (_shuffleStep?.clip != null)
        {
            _shuffleStep.PlayOneShot(_shuffleStep.clip);
        }
    }

    private float GetMinStepGapForState(int stateHash)
    {
        return _defaultMinStepGap;
    }

    public void OnHitAnimationComplete()
    {
        _playerController.IsMovementLocked = false;
    }
}

