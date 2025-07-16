using UnityEngine;

public class ExitDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _interactionPrompt;
    [SerializeField] private string _targetSceneName;
    [SerializeField] private string _targetCameraId;

    private bool _isItem = false;
    public bool IsItem => _isItem;

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (!string.IsNullOrEmpty(_targetSceneName))
        {
            TransitionManager.Instance.GetComponent<Animator>().SetTrigger("StartFadeOut");
            GameManager.Instance.PlayerController.enabled = false;
        }
    }

    public void OnEnter()
    {
        _interactionPrompt.SetActive(true);
    }

    public void OnExit()
    {
        _interactionPrompt.SetActive(false);
    }
}
