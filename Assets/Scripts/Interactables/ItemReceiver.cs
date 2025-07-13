using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ItemReceiver : MonoBehaviour, IItemReceiver
{
    [SerializeField] private string _requiredItemName;
    [SerializeField, TextArea] private string _popUpMessage;
    [SerializeField] private bool _consumeItem = true;

    [SerializeField] private UnityEvent _onItemReceivedExternal; // External events
    [SerializeField] private ParticleSystem _particles;
    
    [Header("Materials")] 
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _glitchedMaterial;

    [SerializeField] private AudioSource _itemReceivedAudio;

    private PlayerInventory _playerInventory;

    [SerializeField] private string _name;
    public string Name => _name;
    public string RequiredItemName => _requiredItemName;

    public bool ItemHasBeenReceived { get; set; }
    
    public bool PlayerHasItemInInventory { get; set; }

    [SerializeField] private GameObject _interactionPrompt;

    private void Awake()
    {
        if (!_meshRenderer)
        {
            _meshRenderer = GetComponentInParent<MeshRenderer>(); // Assuming script is attached to trigger box 
        }

        if (string.IsNullOrEmpty(_name))
        {
            _name = gameObject.name;
        }
    }

    private void Start()
    {
        _playerInventory = GameManager.Instance.Player.GetComponent<PlayerInventory>();
        //_meshRenderer.material = _glitchedMaterial;
    }

    private void Update()
    {
        if (!_playerInventory || !_particles || ItemHasBeenReceived)
        {
            if (_particles)
            {
                _particles?.Stop();
            }
        }

        foreach (var item in _playerInventory.Items)
        {
            if (item.itemName == _requiredItemName)
            {
                PlayerHasItemInInventory = true;
                break;
            }
        }

        if (PlayerHasItemInInventory)
        {
            if (_particles && !_particles.isPlaying)
            {
                _particles.Play();
            }
        }
        else
        {
            if (_particles && _particles.isPlaying)
            {
                _particles.Stop();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (string.IsNullOrEmpty(_popUpMessage)) return;

        if (!other.CompareTag("Player")) return;

        if (!_playerInventory || ItemHasBeenReceived) return;

        if (PlayerHasItemInInventory)
        {
            UIManager.Instance.StartPopUpText(_popUpMessage, 0f);
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
        
        if (_particles) _particles?.Stop();

        if (_itemReceivedAudio)
        {
            if (_itemReceivedAudio.clip)
            {
                _itemReceivedAudio.PlayOneShot(_itemReceivedAudio.clip);
            }
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
        //_meshRenderer.material = _defaultMaterial;
    }
}
