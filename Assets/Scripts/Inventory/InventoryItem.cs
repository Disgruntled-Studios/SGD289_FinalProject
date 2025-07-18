using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string additionalText;
    public Sprite readIcon;
    public Sprite unreadIcon;
    public readonly bool isGun;
    public readonly bool isNote;
    public bool hasBeenRead;

    public InventoryItem(string name, bool isGun, bool isNote, Sprite readIcon, Sprite unreadIcon, string additionalText = "")
    {
        itemName = name;
        this.readIcon = readIcon;
        this.unreadIcon = unreadIcon;
        this.additionalText = additionalText;
        this.isGun = isGun;
        this.isNote = isNote;
        hasBeenRead = false;
    }
}
