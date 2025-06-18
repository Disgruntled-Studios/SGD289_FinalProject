using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _itemName = "Test Item";
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _dropPrefab;
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        var item = new InventoryItem(_itemName, _icon, _dropPrefab);
        inventory.AddItem(item);
        Destroy(gameObject);
    }

    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
