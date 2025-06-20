using UnityEngine;

public interface IItemReceiver
{
    bool TryReceiveItem(PlayerInventory inventory, InventoryItem item);
}
