using UnityEngine;

public class DoorTransition : MonoBehaviour, IInteractable
{
    [SerializeField] private string _sceneSwitchName;
    [SerializeField] private string _cameraSwitchId;
    [SerializeField] private bool useGameManagerExit;

    private bool _isItem = false;
    public bool IsItem => _isItem;

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (useGameManagerExit)
        {
            GameManager.Instance.ResetScene();
            return;
        }
        TransitionManager.Instance.TransitionToScene(_sceneSwitchName, _cameraSwitchId);
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
