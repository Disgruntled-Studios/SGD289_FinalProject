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

    [SerializeField] private bool _isGun; // ONLY MARK THIS ON GUN OBJECT
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        var item = new InventoryItem(_itemName, _isReadable, _isDroppable, _icon, _dropPrefab, _additionalText, _noteContents);
        inventory.AddItem(item);

        if (_isGun)
        {
            player.gameObject.GetComponent<PlayerController>().GunController.HasGun = true;
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
