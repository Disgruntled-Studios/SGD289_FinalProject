using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _itemName;
    [SerializeField, TextArea] private string _additionalDialogue;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _dropPrefab;
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        var item = new InventoryItem(_itemName, _icon, _dropPrefab);
        inventory.AddItem(item);

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.InitiateDialogue("Picked up " + _itemName + ". " + _additionalDialogue);
        }
        else
        {
            Debug.LogWarning("DialogueManager could not be found");
        }

        GameManager.Instance.PlayerController.ClearCurrentInteractable(this);
        
        Destroy(transform.root.gameObject);
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
