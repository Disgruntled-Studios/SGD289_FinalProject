using UnityEngine;
using UnityEngine.InputSystem;

public class PowerPuzzleMode : IPlayerMode
{
    private TileSelection playerSelection;

    public PowerPuzzleMode(TileSelection _playerSelection)
    {
        playerSelection = _playerSelection;
    }
    public void Aim(InputAction.CallbackContext context)
    {
        return;
    }

    public void Attack()
    {
        return;
    }

    public void Crouch(bool isPressed)
    {
        return;
    }

    public void Look(Vector2 input, Transform context)
    {
        throw new System.NotImplementedException();
    }

    public void Jump()
    {
        return;
    }

    public void Move(Rigidbody rb, Vector2 input, Transform context)
    {
        //playerSelection.MoveSelection(input.x, input.y);
    }

    public void Move(Rigidbody rb, float input, Transform context)
    {
        throw new System.NotImplementedException();
    }

    public void Rotate(float input, Transform context)
    {
        throw new System.NotImplementedException();
    }

    public void Rotate(Vector2 input, Transform context)
    {
        return;
    }

    public void Tick()
    {
        return;
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
