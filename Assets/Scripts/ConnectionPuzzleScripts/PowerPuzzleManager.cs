using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PowerPuzzleManager : MonoBehaviour, IInteractable
{
    [Header("Puzzle Nodes")]
    [SerializeField] private PowerPuzzleTile _powerNode;
    [SerializeField] private PowerPuzzleTile _receiverNode;
    
    [Header("Puzzle Components")]
    [SerializeField] private TileSelection _tileSelection;
    [SerializeField] Light selectionLight;
    
    [Header("Cameras")]
    [SerializeField] private GameCamera _sceneCamera;
    [SerializeField] private GameCamera _puzzleCamera;
    public bool hasCameraCut;
    public GameCamera cutCam;
    public float camCutLength;
    
    [Header("Puzzle Events")]
    [SerializeField] private UnityEvent _onPuzzleComplete;
    
    [Header("Dialog")]
    [SerializeField, TextArea] private string puzzleCompletionDialogue;
    [TextArea] public string puzzleOnEnterDialogue;
    public bool hasEnterPopUpTriggered;

    public Transform HighlightableObj;
    
    private readonly List<PowerPuzzleTile> _tiles = new();
    private bool _isPuzzleDone;

    [SerializeField] private Animator _doorAnimator;
    private const string DoorTrigger = "OpenDoor";

    private const float PuzzleCompletionViewDuration = 0.5f;

    [SerializeField] private GameObject _interactionPrompt;

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
            _interactionPrompt.SetActive(false);
            _onPuzzleComplete?.Invoke();
            CompletePuzzleAndExit();
        }

        if (!_powerNode.IsConnected)
        {
            CheckTilesConnection();
        }
    }

    private void CompletePuzzleAndExit()
    {
        if (_isPuzzleDone) return;

        _isPuzzleDone = true;
        UIManager.Instance.UIAudioController.PlaySound(UISound.CircuitComplete);

        if (hasCameraCut)
        {
            StartCoroutine(HandleCamCut());
        }
        else
        {
            StartCoroutine(ExitPuzzleByCompletion());
        }
    }

    public void ExitPuzzleManually()
    {
        ReturnToGameplay();
    }

    private IEnumerator ExitPuzzleByCompletion()
    {
        SetAllCompleted();
        yield return new WaitForSeconds(PuzzleCompletionViewDuration);
        ReturnToGameplay();
    }
    
    public void SetAllCompleted()
    {
        foreach (var tile in _tiles)
        {
            tile.SetCompletedMaterial();
        }
    }

    private void ReturnToGameplay()
    {
        CameraManager.Instance.TrySwitchToCamera(_sceneCamera.CameraID, "EaseInOut", 1.25f);
        InputManager.Instance.SwitchToDefaultInput();
        UIManager.Instance.SetPuzzlePanelActive(false);
        if (selectionLight) selectionLight.enabled = false;
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
        if (!InputManager.Instance.IsInPuzzle || _isPuzzleDone) return;

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

        UIManager.Instance.UIAudioController.PlaySound(UISound.TileNav);
        _tileSelection.transform.localPosition = new Vector3(pos.x, pos.y, 0);
    }

    public void RotateTile(bool rotateRight)
    {
        if (_isPuzzleDone) return;
        
        if (rotateRight)
        {
            _tileSelection.selectedOBJ.transform.Rotate(0, 0, 90f);
        }
        else
        {
            _tileSelection.selectedOBJ.transform.Rotate(0, 0, -90f);
        }

        UIManager.Instance.UIAudioController.PlaySound(UISound.TileRotate);
        
        CheckTilesConnection();
    }

    private IEnumerator HandleCamCut()
    {
        SetAllCompleted();
        UIManager.Instance.SetPuzzlePanelActive(false);
        yield return new WaitForSeconds(PuzzleCompletionViewDuration);

        CameraManager.Instance.TrySwitchToCamera(cutCam.CameraID, "EaseInOut", 1.25f);

        _doorAnimator?.SetTrigger(DoorTrigger);

        var clipLength =
            _doorAnimator?.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == DoorTrigger)
                ?.length ?? camCutLength;

        if (RumbleController.Instance)
        {
            RumbleController.Instance.TriggerPatternedRumble(0.5f, clipLength, RumblePattern.Constant);
        }

        yield return new WaitForSeconds(clipLength);
        
        ReturnToGameplay();
    }

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (!_isPuzzleDone)
        {
            InputManager.Instance.SwitchToPuzzleInput();
            CameraManager.Instance.TrySwitchToCamera(_puzzleCamera.CameraID, "EaseInOut", 0.75f);
            UIManager.Instance.SetPuzzlePanelActive(true);
            if (selectionLight != null) selectionLight.enabled = true;
        }
    }

    public void OnEnter()
    {
        if (!_isPuzzleDone)
        {
            _interactionPrompt.SetActive(true);
        }
    }

    public void OnExit()
    {
        _interactionPrompt.SetActive(false);
    }
}
