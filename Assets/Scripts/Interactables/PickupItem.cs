using UnityEngine;
using UnityEngine.Serialization;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _itemName;
    [SerializeField, TextArea] private string _additionalText;
    public string AdditionalText => _additionalText;
    [SerializeField] private Sprite _icon;

    private bool _isGun; // PlayerGun script sets this automatically
    private bool _isNote; // ReadableNote script sets this automatically
    
    private void Start()
    {
        _isGun = GetComponent<PlayerGun>();
        _isNote = GetComponent<ReadableNote>();
    }
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (_isGun)
        {
            player.gameObject.GetComponent<PlayerController>().GunController.HasGun = true;
            UIManager.Instance.ToggleGunImage(true);
            UIManager.Instance.StartPopUpText("A pistol with a full mag, this could come in handy.");
        }
        else
        {
            var item = new InventoryItem(_itemName, _isGun, _isNote, _icon, _additionalText);
            inventory.AddItem(item);
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
