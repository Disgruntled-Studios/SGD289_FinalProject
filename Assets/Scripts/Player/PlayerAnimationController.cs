using System;
using TMPro;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _anim;

    private const float MovementThreshold = 0.1f;

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
        var turnInput = Mathf.Clamp(_playerController.GetCurrentTurnInput(), -1f, 1f);

        _anim.SetFloat("MoveSpeed", forwardInput);
        _anim.SetFloat("TurnSpeed", turnInput);
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
}

