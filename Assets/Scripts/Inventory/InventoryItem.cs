using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    [FormerlySerializedAs("additionalText")] public string additionalInteractText;
    [FormerlySerializedAs("itemDescription")] public string inventoryItemDescription;
    public Sprite icon;
    public readonly bool isGun;

    public InventoryItem(string name, bool isGun, Sprite icon = null, string additionalInteractText = "", string inventoryItemDescription = "")
    {
        itemName = name;
        this.icon = icon;
        this.additionalInteractText = additionalInteractText;
        this.isGun = isGun;
        this.inventoryItemDescription = inventoryItemDescription;
    }
}
