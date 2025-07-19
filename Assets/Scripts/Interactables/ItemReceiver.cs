using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ItemReceiver : MonoBehaviour, IItemReceiver
{
    [SerializeField] private string _requiredItemName;
    [SerializeField, TextArea] private string _popUpMessage;
    [SerializeField] private bool _consumeItem = true;

    [SerializeField] private UnityEvent _onItemReceivedExternal; // External events

    private PlayerInventory _playerInventory;

    [SerializeField] private string _name;
    public string Name => _name;
    public string RequiredItemName => _requiredItemName;

    public bool ItemHasBeenReceived { get; set; }
    
    public bool PlayerHasItemInInventory { get; set; }

    [SerializeField] private GameObject _interactionPrompt;
    
    [SerializeField] private AudioClip _interactionClip;
    [SerializeField] private AudioMixerGroup _outputGroup;
    [SerializeField] private float _volume = 1f;
    [SerializeField] private float _pitch = 1f;
    [SerializeField] private float _spatialBlend = 0f;

    private void Awake()
    {
        if (string.IsNullOrEmpty(_name))
        {
            _name = gameObject.name;
        }
    }

    private void Start()
    {
        _playerInventory = GameManager.Instance.Player.GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        foreach (var item in _playerInventory.Items)
        {
            if (item.itemName == _requiredItemName)
            {
                PlayerHasItemInInventory = true;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!_playerInventory || ItemHasBeenReceived) return;

        if (PlayerHasItemInInventory)
        {
            UIManager.Instance.StartPopUpText($"Use {_requiredItemName}?", 0f, false);
            _interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        UIManager.Instance.ClearPopUpText();
        
        if (_interactionPrompt && _interactionPrompt.activeSelf)
        {
            _interactionPrompt.SetActive(false);
        }
    }

    public bool TryReceiveItem(PlayerInventory inventory, InventoryItem item)
    {
        if (item == null || item.isGun || item.isNote) return false;
        
        if (item.itemName != _requiredItemName || ItemHasBeenReceived)
        {
            return false;
        }

        if (_consumeItem)
        {
            inventory.RemoveItem(item);
        }

        ItemHasBeenReceived = true;

        if (_interactionClip)
        {
            SoundUtility.PlayClipAtPoint(_interactionClip, transform.position, _volume, _pitch, _outputGroup,
                _spatialBlend);
        }

        Destroy(_interactionPrompt);
        
        UIManager.Instance.ClearPopUpText();
        
        _onItemReceivedExternal?.Invoke();
        OnItemReceivedInternal();
        
        return true;
    }

    // Internal events
    private void OnItemReceivedInternal()
    {
        
    }
}
