using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PowerPuzzleManager : MonoBehaviour, IInteractable
{

    [HideInInspector] public int currentTab;

#region Setup Variables

    [Header("Puzzle Nodes")]
    public PowerPuzzleTile _powerNode;
    public PowerPuzzleTile _receiverNode;
    
    [Header("Puzzle Components")]
    public TileSelection _tileSelection;
    public Light selectionLight;
    
    [Header("Cameras")]
    public GameCamera _sceneCamera;
    public GameCamera _puzzleCamera;
    
    [Header("Camera Cut")]
    public bool hasCameraCut;
    public GameCamera cutCam;
    public float camCutLength;
    
    [Header("Puzzle Events")]
    public UnityEvent _onPuzzleComplete;
    
    [Header("Dialog")]
    [TextArea] public string puzzleCompletionDialogue;
    [TextArea] public string puzzleOnEnterDialogue;
    public bool hasEnterPopUpTriggered;

    [Header("Audio")] 
    [SerializeField] private AudioSource _enterAudio;

#endregion Setup Variables

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
        GameManager.Instance.PlayerController.UnfreezeMovement();
        
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
        if (_isPuzzleDone) return;
        
        GameManager.Instance.PlayerController.FreezeMovement();
        
        InputManager.Instance.SwitchToPuzzleInput();
        CameraManager.Instance.TrySwitchToCamera(_puzzleCamera.CameraID, "EaseInOut", 0.75f);
        UIManager.Instance.SetPuzzlePanelActive(true);
        if (selectionLight != null) selectionLight.enabled = true;

        if (_enterAudio)
        {
            _enterAudio.Play();
        }
            
        RumbleController.Instance.TriggerPresetRumble(RumblePreset.ConnectionPuzzleEntry);
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


#if UNITY_EDITOR

[CustomEditor(typeof(PowerPuzzleManager))]
public class PowerPuzzleManager_Editor : Editor
{
    PowerPuzzleManager targetScript;
    private SerializedObject soTarget;

#region Camera Vars
    private SerializedProperty cutCam;
    private SerializedProperty camCutLength;
    private SerializedProperty hasCameraCut;
    private SerializedProperty _sceneCamera;
    private SerializedProperty _puzzleCamera;
#endregion

#region  PuzzleComponents

    private SerializedProperty _powerNode;
    private SerializedProperty _receieverNode;
    private SerializedProperty _tileSelection;
    
#endregion

#region OutsideReferences

    private SerializedProperty puzzleCompletionDialogue;
    private SerializedProperty puzzleOnEnterDialogue;
    private SerializedProperty _onPuzzleComplete;
    private SerializedProperty _enterAudio;
    private SerializedProperty highlightableObj;
    private SerializedProperty _doorAnimator;

#endregion

    void OnEnable()
    {
        targetScript = (PowerPuzzleManager)target;
        soTarget = new SerializedObject(target);

        cutCam = soTarget.FindProperty("cutCam");
        camCutLength = soTarget.FindProperty("camCutLength");
        _sceneCamera = soTarget.FindProperty("_sceneCamera");
        _puzzleCamera = soTarget.FindProperty("_puzzleCamera");
        hasCameraCut = soTarget.FindProperty("hasCameraCut");

        _powerNode = soTarget.FindProperty("_powerNode");
        _receieverNode = soTarget.FindProperty("_receiverNode");
        _tileSelection = soTarget.FindProperty("_tileSelection");

        puzzleCompletionDialogue = soTarget.FindProperty("puzzleCompletionDialogue");
        puzzleOnEnterDialogue = soTarget.FindProperty("puzzleOnEnterDialogue");
        _onPuzzleComplete = serializedObject.FindProperty("_onPuzzleComplete");
        _enterAudio = soTarget.FindProperty("_enterAudio");
        highlightableObj = soTarget.FindProperty("HighlightableObj");
        _doorAnimator = soTarget.FindProperty("_doorAnimator");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUI.BeginChangeCheck();
        
        targetScript.currentTab = GUILayout.Toolbar(targetScript.currentTab, new string[] {"Cameras","PuzzleComponents","OutsideReferences"});

        switch (targetScript.currentTab)
        {
            case 0:

                EditorGUILayout.PropertyField(_sceneCamera);
                EditorGUILayout.PropertyField(_puzzleCamera);
                EditorGUILayout.PropertyField(hasCameraCut);

                if (targetScript.hasCameraCut)
                {
                    EditorGUILayout.PropertyField(cutCam);
                    EditorGUILayout.PropertyField(camCutLength);
                }
                break;
            case 1:

                EditorGUILayout.PropertyField(_powerNode);
                EditorGUILayout.PropertyField(_receieverNode);
                EditorGUILayout.PropertyField(_tileSelection);
            
                break;
            case 2:

                EditorGUILayout.PropertyField(puzzleCompletionDialogue);
                EditorGUILayout.PropertyField(puzzleOnEnterDialogue);
                EditorGUILayout.PropertyField(_onPuzzleComplete);
                EditorGUILayout.PropertyField(_enterAudio);
                EditorGUILayout.PropertyField(highlightableObj);
                EditorGUILayout.PropertyField(_doorAnimator);
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }
        



        
    }
}

#endif