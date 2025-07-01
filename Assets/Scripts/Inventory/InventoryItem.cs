using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string additionalText;
    public string noteContents;
    public string itemDescription;
    public Sprite icon;
    public bool isReadable;
    public readonly bool isGun;

    public InventoryItem(string name, bool isReadable, bool isGun, Sprite icon = null, string additionalText = "", string noteContents = "", string itemDescription = "")
    {
        itemName = name;
        this.icon = icon;
        this.isReadable = isReadable;
        this.additionalText = additionalText;
        this.noteContents = noteContents;
        this.isGun = isGun;
        this.itemDescription = itemDescription;
    }
}
