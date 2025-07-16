using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class KeycodeReceiver : MonoBehaviour, IInteractable
{
    [SerializeField] private string _correctCode;
    [SerializeField] private UnityEvent _onCorrectCodeEntered;
    [SerializeField, TextArea] private string _onCompletionText;

    [SerializeField] private bool _shouldHighlight;
    public bool ShouldHighlight => _shouldHighlight;

    [SerializeField] private bool _shouldShowPipeIcons;
    public bool ShouldShowPipeIcons => _shouldShowPipeIcons;

    private const string PromptText = "Enter Keycode:";

    private bool _playerIsNearby;
    
    public bool CodeHasBeenAccepted { get; set; }

    [SerializeField] private GameObject _interactionPrompt;

    [SerializeField] private AudioClip _interactionClip;
    [SerializeField] private AudioMixerGroup _outputGroup;
    [SerializeField] private float _volume = .25f;
    [SerializeField] private float _pitch = 1f;
    [SerializeField] private float _spatialBlend = 0f;

    [SerializeField] private GameObject _door;
    [SerializeField] private Material _nearMaterial;
    [SerializeField] private Material _farMaterial;

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (!_playerIsNearby || CodeHasBeenAccepted) return;
        
        if (_interactionClip)
        {
            SoundUtility.PlayClipAtPoint(_interactionClip, transform.position, _volume, _pitch, _outputGroup,
                _spatialBlend);
        }
            
        UIManager.Instance.OpenKeycodePanel(this);
    }

    public void SubmitCode(string input)
    {
        if (string.Equals(input, _correctCode))
        {
            UIManager.Instance.KeycodeUIController.AnimateCorrectCodeSequence(() =>
            {
                _onCorrectCodeEntered?.Invoke();
                CodeHasBeenAccepted = true;

                if (_interactionPrompt && _interactionPrompt.activeSelf)
                {
                    _interactionPrompt.SetActive(false);
                }
                
                UIManager.Instance.CloseKeycodePanel(true);
                GameManager.Instance.PlayerController.currentHighlightedObj = null;
            });
        }
        else
        {
            UIManager.Instance.ShowInvalidCodeFeedback();
        }
    }

    public void OnEnter()
    {
        if (CodeHasBeenAccepted) return;
        
        _playerIsNearby = true;
        _interactionPrompt.SetActive(true);
        UIManager.Instance.StartPopUpText("Enter Keycode?", 0f, withPrompt: false);

        if (!_door) return;
        
        const int matSlot = 1;
        
        var doorMeshRend = _door.GetComponent<MeshRenderer>();
        var materials = doorMeshRend.materials;
        materials[matSlot] = _nearMaterial;
        doorMeshRend.materials = materials;
    }

    public void OnExit()
    {
        if (CodeHasBeenAccepted) return;
        
        _playerIsNearby = false;
        _interactionPrompt.SetActive(false);
        UIManager.Instance.ClearPopUpText();

        if (!_door) return;
        
        const int matSlot = 1;
        
        var doorMeshRend = _door.GetComponent<MeshRenderer>();
        var materials = doorMeshRend.materials;
        materials[matSlot] = _farMaterial;
        doorMeshRend.materials = materials;
    }
}
