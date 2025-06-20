using UnityEngine;
using UnityEngine.Serialization;

public class ItemReceiver : MonoBehaviour, IItemReceiver
{
    [SerializeField] private string _requiredItemName;
    [SerializeField] private bool _consumeItem = true;

    [Header("Action")] 
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private string _action;

    [SerializeField] private ParticleSystem _particles;

    private PlayerInventory _playerInventory;

    private void Start()
    {
        _playerInventory = GameManager.Instance.Player.GetComponent<PlayerInventory>();
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

        TriggerEffect();
        return true;
    }

    private void TriggerEffect()
    {
        switch (_action)
        {
            case "UnlockDoor":
                if (_targetObject)
                {
                    _targetObject.SetActive(false);
                }

                break;
        }
    }
}
