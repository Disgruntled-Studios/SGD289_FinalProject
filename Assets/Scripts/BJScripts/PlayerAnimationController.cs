using TMPro;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _anim;

    private const float MovementThreshold = 0.1f;
    
    private void Update()
    {
        var input = _playerController?.MovementInput ?? Vector2.zero;
        var isMoving = input.magnitude > MovementThreshold;

        _anim?.SetBool("IsMoving", isMoving);
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _anim?.SetTrigger("Dance");
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
}

