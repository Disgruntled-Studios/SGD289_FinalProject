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

    private PlayerInventory _playerInventory;
    private bool _hasPopUpTriggered;

    [SerializeField] private string _name;
    public string Name => _name;

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
        _meshRenderer.material = _glitchedMaterial;
    }

    private void Update()
    {
        if (!_playerInventory || !_particles) return;

        var hasItem = false;

        foreach (var item in _playerInventory.Items)
        {
            if (item.itemName == _requiredItemName)
            {
                hasItem = true;
                break;
            }
        }

        if (hasItem)
        {
            if (!_particles.isPlaying)
            {
                _particles.Play();
            }
        }
        else
        {
            if (_particles.isPlaying)
            {
                _particles.Stop();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hasPopUpTriggered) return;
        UIManager.Instance.StartPopUpText(_popUpMessage);
        _hasPopUpTriggered = true;
    }

    public bool TryReceiveItem(PlayerInventory inventory, InventoryItem item)
    {
        if (item == null) return false;

        if (item.itemName != _requiredItemName)
        {
            return false;
        }

        if (_consumeItem)
        {
            inventory.RemoveItem(item);
        }

        _onItemReceivedExternal?.Invoke();
        OnItemReceivedInternal();
        return true;
    }

    // Internal events
    private void OnItemReceivedInternal()
    {
        _meshRenderer.material = _defaultMaterial;
    }
}
