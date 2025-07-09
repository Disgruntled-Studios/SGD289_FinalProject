using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _itemName;
    [SerializeField, TextArea] private string _additionalText;
    public string AdditionalText => _additionalText;
    [SerializeField] private Sprite _icon;

    private bool _isGun; // PlayerGun script sets this automatically
    private bool _isNote; // ReadableNote script sets this automatically

    private bool _isDevNote;

    public UnityEvent onGunPickup; // DONT USE FOR ANYTHING EXCEPT THE GUN
    
    private void Start()
    {
        _isGun = GetComponent<PlayerGun>();
        _isNote = GetComponent<ReadableNote>();

        if (TryGetComponent<ReadableNote>(out var devNote))
        {
            _isDevNote = devNote.IsDevNote;
        }
    }
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (_isGun)
        {
            player.gameObject.GetComponent<PlayerController>().GunController.HasGun = true;
            UIManager.Instance.ToggleGunImage(true);
            onGunPickup?.Invoke();
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
        if (_isDevNote)
        {
            UIManager.Instance.StartPopUpText("Press Triangle to Collect", 0f);
        }
    }

    public void OnExit()
    {
        if (_isDevNote)
        {
            UIManager.Instance.ClearPopUpText();
        }
    }
}
