using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string additionalText;
    public Sprite icon;
    public readonly bool isGun;
    public readonly bool isNote;
    public bool hasBeenRead;

    public InventoryItem(string name, bool isGun, bool isNote, Sprite icon = null, string additionalText = "")
    {
        itemName = name;
        this.icon = icon;
        this.additionalText = additionalText;
        this.isGun = isGun;
        this.isNote = isNote;
        hasBeenRead = false;
    }
}
