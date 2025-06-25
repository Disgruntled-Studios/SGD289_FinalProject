using UnityEngine;
using UnityEngine.Events;

public class KeycodeReceiver : MonoBehaviour, IInteractable
{
    [SerializeField] private string _correctCode;
    [SerializeField] private UnityEvent _onCorrectCodeEntered;

    private const string PromptText = "Enter Keycode: ";

    private bool _playerIsNearby;
    
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
            UIManager.Instance.CloseKeycodePanel();
        }
        else
        {
            UIManager.Instance.ShowInvalidKeycodeFeedback();
        }
    }

    public void OnEnter()
    {
        _playerIsNearby = true;
        Debug.Log("Player is near");
    }

    public void OnExit()
    {
        _playerIsNearby = false;
        Debug.Log("player is far");
    }
}
