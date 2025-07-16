using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string _itemName;
    [SerializeField, TextArea] private string _additionalText;
    public string AdditionalText => _additionalText;
    [SerializeField] private Sprite _icon;

    private bool _isGun; // PlayerGun script sets this automatically
    public bool IsGun => _isGun;
    private bool _isNote; // ReadableNote script sets this automatically

    private bool _isDevNote;

    public UnityEvent onGunPickup; // DONT USE FOR ANYTHING EXCEPT THE GUN

    [SerializeField] private GameObject _interactionPrompt;
    
    [SerializeField] private AudioClip _interactionClip;
    [SerializeField] private AudioMixerGroup _outputGroup;
    [SerializeField] private float _volume = 1f;
    [SerializeField] private float _pitch = 1f;
    
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
            UIManager.Instance.ActivateInventoryIndicator();
        }

        if (_isDevNote)
        {
            UIManager.Instance.StartPopUpText(_additionalText);
        }

        if (_interactionClip != null)
        {
            SoundUtility.PlayClipAtPoint(_interactionClip, transform.position, _volume, _pitch, _outputGroup, 0f);
        }
        
        RumbleController.Instance.TriggerPresetRumble(RumblePreset.ItemPickup);

        GameManager.Instance.PlayerController.ClearCurrentInteractable(this);

        Destroy(transform.root.gameObject);
        
        if (_interactionPrompt)
        {
            Destroy(_interactionPrompt);
        }
    }

    public void OnEnter()
    {
        if (_interactionPrompt)
        {
            _interactionPrompt.SetActive(true);
        }
    }

    public void OnExit()
    {
        if (_interactionPrompt)
        {
            _interactionPrompt.SetActive(false);
        }
    }
}