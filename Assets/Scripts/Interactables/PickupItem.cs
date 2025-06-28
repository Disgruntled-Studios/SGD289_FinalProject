using UnityEngine;
using UnityEngine.Serialization;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _itemName;
    [SerializeField, TextArea] private string _additionalText;
    public string AdditionalText => _additionalText;
    [SerializeField, TextArea] private string _noteContents;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _dropPrefab;
    [SerializeField] private bool _isReadable;
    [SerializeField] private bool _isDroppable;
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        var item = new InventoryItem(_itemName, _isReadable, _isDroppable, _icon, _dropPrefab, _additionalText, _noteContents);
        inventory.AddItem(item);

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
