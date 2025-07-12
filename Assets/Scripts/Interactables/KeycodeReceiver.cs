using UnityEngine;
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
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (_playerIsNearby)
        {
            UIManager.Instance.OpenKeycodePanel(this);
        }
    }

    public void SubmitCode(string input)
    {
        if (string.Equals(input, _correctCode))
        {
            _onCorrectCodeEntered?.Invoke();
            CodeHasBeenAccepted = true;
            UIManager.Instance.CloseKeycodePanel();
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
        UIManager.Instance.StartPopUpText("Enter code?", 0f);
    }

    public void OnExit()
    {
        _playerIsNearby = false;
        UIManager.Instance.ClearPopUpText();
    }
}
