using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string additionalText;
    public string noteContents;
    public Sprite icon;
    public GameObject prefab;
    public bool isReadable;
    public bool isDroppable;

    public InventoryItem(string name, bool isReadable, bool isDroppable, Sprite icon = null, GameObject prefab = null, string additionalText = "", string noteContents = "")
    {
        itemName = name;
        this.icon = icon;
        this.prefab = prefab;
        this.isReadable = isReadable;
        this.isDroppable = isDroppable;
        this.additionalText = additionalText;
        this.noteContents = noteContents;
    }
}
