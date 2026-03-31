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

#region Custom Editor Vars

    [HideInInspector] public int toolbarTab;
    [HideInInspector] public string currentTab;

#endregion Custom Editor Vars

#region Setup Variables

    [Header("Puzzle Nodes")]
    [SerializeField, Tooltip("This is the node that has power and will need to connect to the RecieverNode to complete the puzzle")]
    private PowerPuzzleTile _powerNode;
    [SerializeField, Tooltip("This is the node that has no power and will need to connect to the PowerNode to complete the puzzle")]
    private PowerPuzzleTile _receiverNode;
    
    [Header("Puzzle Components")]
    [SerializeField,Tooltip("This is the tile selection gameobject reference it'll act as a cursor for the puzzle.")]
    private TileSelection _tileSelection;
    [SerializeField,Tooltip("This is the light that may turn on while the player is solving the puzzle.")] 
    Light selectionLight;
    
    [Header("Cameras")]
    [Tooltip("This is what camera in the scene the game should switch from when the puzzle starts and switch back to once the puzzle ends.")]
    public GameCamera _sceneCamera;
    [Tooltip("This is what camera in the scene the game should switch to when the puzzle starts.")]
    public GameCamera _puzzleCamera;
    
    [Header("Camera Cut")]
    [Tooltip("Boolean check to see wether or not the puzzle cuts to another camera to showcase an animation after the puzzle ends.")]
    public bool hasCameraCut;
    [Tooltip("The camera that the puzzle will cut to after the puzzle before returning to the scene camera.")]
    public GameCamera cutCam;
    [Tooltip("The length that the camera cut will last for after the puzzle is completed.")]
    public float camCutLength;
    
    [Header("Puzzle Events")]
    [SerializeField, Tooltip("The UnityEvent that will be called after the puzzle is completed. You can use this to reference outside scripts to trigger other functions that you want to use as the event is called")]
    private UnityEvent _onPuzzleComplete;
    
    [Header("Dialog")]
    [TextArea, Tooltip("This is the text that can appear after the puzzle is completed.")]
     public string puzzleCompletionDialogue;
    [TextArea, Tooltip("What text can appear once the player gets close to the puzzle.")] 
    public string puzzleOnEnterDialogue;
    public bool hasEnterPopUpTriggered;

    [Header("Audio")] 
    [Tooltip("The AudioSource that will play once the player enters the puzzle.")]
    [SerializeField] private AudioSource _enterAudio;

    public Transform HighlightableObj;

#endregion Setup Variables

    
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
    private SerializedProperty _receiverNode;
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
        _receiverNode = soTarget.FindProperty("_receiverNode");
        _tileSelection = soTarget.FindProperty("_tileSelection");

        puzzleCompletionDialogue = soTarget.FindProperty("puzzleCompletionDialogue");
        puzzleOnEnterDialogue = soTarget.FindProperty("puzzleOnEnterDialogue");
        _onPuzzleComplete = soTarget.FindProperty("_onPuzzleComplete");
        _enterAudio = soTarget.FindProperty("_enterAudio");
        highlightableObj = soTarget.FindProperty("HighlightableObj");
        _doorAnimator = soTarget.FindProperty("_doorAnimator");

    }

    public override void OnInspectorGUI()
    {
        soTarget.Update();
        EditorGUI.BeginChangeCheck();
        
        targetScript.toolbarTab = GUILayout.Toolbar(targetScript.toolbarTab, new string[] {"Cameras","Puzzle Vars","Outside Vars"});

        switch (targetScript.toolbarTab)
        {
            case 0:
                targetScript.currentTab = "Cameras";
                
                break;
            case 1:
                targetScript.currentTab = "Puzzle Vars";

            
                break;
            case 2:
                targetScript.currentTab = "Outside Vars";

                
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        EditorGUI.BeginChangeCheck();


        switch (targetScript.currentTab)
        {
            case "Cameras":

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_sceneCamera);
                EditorGUILayout.PropertyField(_puzzleCamera);
                EditorGUI.indentLevel--;

                
                EditorGUILayout.PropertyField(hasCameraCut);

                if (targetScript.hasCameraCut)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(cutCam);
                    EditorGUILayout.PropertyField(camCutLength);
                    EditorGUI.indentLevel--;
                }

                break;
            case "Puzzle Vars":
                EditorGUILayout.PropertyField(_powerNode);
                EditorGUILayout.PropertyField(_receiverNode);
                EditorGUILayout.PropertyField(_tileSelection);
            
                break;
            case "Outside Vars":
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
            soTarget.ApplyModifiedProperties();
        }
        
    }
}

#endif