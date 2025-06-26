using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleControls : MonoBehaviour
{
    private PowerPuzzleManager _activePuzzleManager;
    private PlayerInput Input => InputManager.Instance.PlayerInput;

    private void OnEnable()
    {
        var puzzleMap = Input.PuzzleMap;
        
        puzzleMap.RotateTileRight.performed += OnRotateTileRight;
        puzzleMap.RotateTileLeft.performed += OnRotateTileLeft;
        puzzleMap.MoveNorth.performed += OnMoveNorth;
        puzzleMap.MoveSouth.performed += OnMoveSouth;
        puzzleMap.MoveWest.performed += OnMoveWest;
        puzzleMap.MoveEast.performed += OnMoveEast;
        puzzleMap.ExitPuzzle.performed += OnExitPuzzle;
    }

    private void OnDisable()
    {
        var puzzleMap = Input.PuzzleMap;
        
        puzzleMap.RotateTileRight.performed -= OnRotateTileRight;
        puzzleMap.RotateTileLeft.performed -= OnRotateTileLeft;
        puzzleMap.MoveNorth.performed -= OnMoveNorth;
        puzzleMap.MoveSouth.performed -= OnMoveSouth;
        puzzleMap.MoveWest.performed -= OnMoveWest;
        puzzleMap.MoveEast.performed -= OnMoveEast;
        puzzleMap.ExitPuzzle.performed -= OnExitPuzzle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PuzzleManager"))
        {
            _activePuzzleManager = other.GetComponentInParent<PowerPuzzleManager>();

            if (!_activePuzzleManager)
            {
                _activePuzzleManager = other.GetComponentInChildren<PowerPuzzleManager>();
            }

            if (!_activePuzzleManager.hasEnterPopUpTriggered && _activePuzzleManager.puzzleOnEnterDialogue != null)
            {

                UIManager.Instance.StartPopUpText(_activePuzzleManager.puzzleOnEnterDialogue);
                _activePuzzleManager.hasEnterPopUpTriggered = true;
            }   
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PuzzleManager"))
        {
            var exitingManager = other.GetComponentInParent<PowerPuzzleManager>() ??
                                 other.GetComponentInChildren<PowerPuzzleManager>();

            if (exitingManager && exitingManager == _activePuzzleManager)
            {
                _activePuzzleManager = null;
            }
        }
    }

    public void OnRotateTileRight(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (context.performed && _activePuzzleManager != null)
        {
            _activePuzzleManager?.RotateTile(true);
        }
    }
    public void OnRotateTileLeft(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (context.performed && _activePuzzleManager != null)
        {
            _activePuzzleManager?.RotateTile(false);
        }
    }
    public void OnMoveNorth(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (context.performed && _activePuzzleManager != null)
        {
            _activePuzzleManager?.MoveSelection(1);
        }
    }
    public void OnMoveSouth(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (context.performed && _activePuzzleManager != null)
        {
            _activePuzzleManager?.MoveSelection(2);
        }
    }
    public void OnMoveWest(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (context.performed && _activePuzzleManager != null)
        {
            _activePuzzleManager?.MoveSelection(3);
        }
    }
    public void OnMoveEast(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (context.performed && _activePuzzleManager != null)
        {
            _activePuzzleManager?.MoveSelection(4);
        }
    }
    public void OnExitPuzzle(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (context.performed && _activePuzzleManager != null && InputManager.Instance.IsInPuzzle)
        {
            _activePuzzleManager?.ExitPuzzle();
        }
    }
    
}
