using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PowerPuzzleManager : MonoBehaviour, IInteractable
{
    [SerializeField] private PowerPuzzleTile _powerNode;
    [SerializeField] private PowerPuzzleTile _receiverNode;
    [SerializeField] private TileSelection _tileSelection;
    [SerializeField] private GameCamera _sceneCamera;
    [SerializeField] private GameCamera _puzzleCamera;
    [SerializeField] private UnityEvent _onPuzzleComplete;
    [SerializeField, TextArea] private string puzzleCompletionDialogue;
    [TextArea] public string puzzleOnEnterDialogue;
    public bool hasEnterPopUpTriggered;

    private readonly List<PowerPuzzleTile> _tiles = new();
    
    private bool _isPuzzleDone;

    private void Awake()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var tile = transform.GetChild(i).GetComponent<PowerPuzzleTile>();
            if (tile.IsPowerNode)
            {
                _powerNode = tile;
            }
            else if (tile.IsReceiverNode)
            {
                _receiverNode = tile;
            }

            _tiles.Add(tile);

            tile.OnTileStateChanged += HandleTileStateChanged;
        }

        _isPuzzleDone = false;
        hasEnterPopUpTriggered = false;
    }

    private void HandleTileStateChanged()
    {
        if (_receiverNode.IsPowered && _receiverNode.IsConnected && !_isPuzzleDone)
        {
            //When the puzzle is solved do these functions.
            _isPuzzleDone = true;
            _onPuzzleComplete.Invoke();
            CameraManager.Instance.TrySwitchToCamera(_sceneCamera.CameraID);
            UIManager.Instance.StartPopUpText(puzzleCompletionDialogue);
            ExitPuzzle();
        }

        if (!_powerNode.IsConnected)
        {
            CheckTilesConnection();
        }
    }

    public void ExitPuzzle()
    {
        CameraManager.Instance.TrySwitchToCamera(_sceneCamera.CameraID);
        InputManager.Instance.SwitchToDefaultInput();
        UIManager.Instance.SetPuzzlePanelActive(false);
    }

    private void CheckTilesConnection()
    {
        foreach (var tile in _tiles)
        {
            if (tile != _powerNode && tile.IsPowered)
            {
                tile.IsPowered = false;
            }
        }
    }

    public void MoveSelection(int direction)
    {
        if (!InputManager.Instance.IsInPuzzle) return;

        var pos = _tileSelection.transform.localPosition;

        switch (direction)
        {
            case 1: // North
                if (pos.y < _tileSelection.yLimit)
                {
                    pos.y += 2;
                }

                break;
            case 2: // South
                if (pos.y > -_tileSelection.yLimit)
                {
                    pos.y -= 2;
                }

                break;
            case 3:
                if (pos.x > -_tileSelection.xLimit)
                {
                    pos.x -= 2;
                }

                break;
            case 4:
                if (pos.x < _tileSelection.xLimit)
                {
                    pos.x += 2;
                }

                break;
        }

        _tileSelection.transform.localPosition = new Vector3(pos.x, pos.y, 0);
    }

    public void RotateTile(bool rotateRight)
    {
        if (rotateRight)
        {
            _tileSelection.selectedOBJ.transform.Rotate(0, 0, 90f);
        }
        else
        {
            _tileSelection.selectedOBJ.transform.Rotate(0, 0, -90f);
        }
        
        CheckTilesConnection();
    }

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (!_isPuzzleDone)
        {
            InputManager.Instance.SwitchToPuzzleInput();
            CameraManager.Instance.TrySwitchToCamera(_puzzleCamera.CameraID);
            UIManager.Instance.SetPuzzlePanelActive(true);
        }
        else
        {
            UIManager.Instance.StartPopUpText("I already fixed this circuit.");
        }
    }

    public void OnEnter()
    {
        return;
    }

    public void OnExit()
    {
        return;
    }
}
