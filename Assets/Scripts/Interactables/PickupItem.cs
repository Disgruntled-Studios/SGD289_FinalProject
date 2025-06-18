using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _dropPrefab;
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        var item = new InventoryItem("Test Item", _icon, _dropPrefab);
        inventory.AddItem(item);
        Destroy(transform.root.gameObject);
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
