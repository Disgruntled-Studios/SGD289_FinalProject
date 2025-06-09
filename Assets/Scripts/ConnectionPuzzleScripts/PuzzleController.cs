using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleController : MonoBehaviour
{
    public PowerPuzzleManager puzzleManager;

    public void SetVars(PowerPuzzleManager managerRef)
    {
        puzzleManager = managerRef;
    }

    public void WipeVars()
    {
        puzzleManager = null;
    }

    public void OnRotateTileRight(InputAction.CallbackContext context)
    {
        if (context.performed && puzzleManager != null)
        {
            puzzleManager.RotateTile(true);
        }
    }
    public void OnRotateTileLeft(InputAction.CallbackContext context)
    {
        if (context.performed && puzzleManager != null)
        {
            puzzleManager.RotateTile(false);
        }
    }
    public void OnMoveNorth(InputAction.CallbackContext context)
    {
        if (context.performed && puzzleManager != null)
        {
            puzzleManager.MoveSelection(1);
        }
    }
    public void OnMoveSouth(InputAction.CallbackContext context)
    {
        if (context.performed && puzzleManager != null)
        {
            puzzleManager.MoveSelection(2);
        }
    }
    public void OnMoveWest(InputAction.CallbackContext context)
    {
        if (context.performed && puzzleManager != null)
        {
            puzzleManager.MoveSelection(3);
        }
    }
    public void OnMoveEast(InputAction.CallbackContext context)
    {
        if (context.performed && puzzleManager != null)
        {
            puzzleManager.MoveSelection(4);
        }
    }
    public void OnExitPuzzle(InputAction.CallbackContext context)
    {
        if (context.performed && puzzleManager != null)
        {
            puzzleManager.ExitPuzzle();
        }
    }
    
}
