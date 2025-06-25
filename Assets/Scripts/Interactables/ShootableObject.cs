using UnityEngine;
using UnityEngine.Events;

public class ShootableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent onShotEvent;
    [SerializeField, TextArea] private string dialoguePopUpTxt;
    private bool isOpen = false;

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (!isOpen)
        {
            UIManager.Instance.StartPopUpText(dialoguePopUpTxt);
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

    public void OnShot()
    {
        onShotEvent?.Invoke();
        isOpen = true;
    }
}
