using UnityEngine;

public interface IItemReceiver
{
    string Name { get; }
    bool TryReceiveItem(PlayerInventory inventory, InventoryItem item);
}
