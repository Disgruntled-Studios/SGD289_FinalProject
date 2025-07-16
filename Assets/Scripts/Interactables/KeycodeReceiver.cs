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
    [SerializeField] private float _volume = 1f;
    [SerializeField] private float _pitch = 1f;
    [SerializeField] private float _spatialBlend = 0f;
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (_playerIsNearby)
        {
            if (_interactionClip)
            {
                SoundUtility.PlayClipAtPoint(_interactionClip, transform.position, _volume, _pitch, _outputGroup,
                    _spatialBlend);
            }
            
            UIManager.Instance.OpenKeycodePanel(this);
        }
    }

    public void SubmitCode(string input)
    {
        if (string.Equals(input, _correctCode))
        {
            _onCorrectCodeEntered?.Invoke();
            CodeHasBeenAccepted = true;
            UIManager.Instance.CloseKeycodePanel(true);
            //UIManager.Instance.StartPopUpText(_onCompletionText);
            GameManager.Instance.PlayerController.currentHighlightedObj = null;
        }
        else
        {
            UIManager.Instance.ShowInvalidCodeFeedback();
        }
    }

    public void OnEnter()
    {
        _playerIsNearby = true;
        // UIManager.Instance.StartPopUpText("Enter code?", 0f);
    }

    public void OnExit()
    {
        _playerIsNearby = false;
        // UIManager.Instance.ClearPopUpText();
    }
}
