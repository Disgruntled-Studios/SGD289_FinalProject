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
        
        // Having fun with animations
        // if (Input.GetKeyDown(KeyCode.Y))
        // {
        //     _anim?.SetTrigger("Dance");
        // }
    }
}
