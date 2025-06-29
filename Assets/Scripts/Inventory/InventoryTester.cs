using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private Transform _playerTransform;

    [Header("Test Pickups")] 
    [SerializeField] private GameObject[] _testItems;

    [ContextMenu("Add First Test Item")]
    public void AddFirstTestItem()
    {
        if (_testItems.Length == 0) return;
        SpawnAndPickup(_testItems[0]);
    }

    public void SpawnAndPickup(GameObject pickupItem)
    {
        if (!pickupItem) return;

        var instance = Instantiate(pickupItem, Vector3.zero, Quaternion.identity);

        var pickup = instance.GetComponentInChildren<PickupItem>();

        if (pickup)
        {
            pickup.Interact(_playerTransform, _playerInventory);
        }
    }

    [ContextMenu("Add All Test Items")]
    public void AddAllTestItems()
    {
        foreach (var item in _testItems)
        {
            SpawnAndPickup(item);
        }
    }
}
