using UnityEngine;

public interface IItemReceiver
{
    string Name { get; }
    bool ItemHasBeenReceived { get; set; }
    bool TryReceiveItem(PlayerInventory inventory, InventoryItem item);
}
