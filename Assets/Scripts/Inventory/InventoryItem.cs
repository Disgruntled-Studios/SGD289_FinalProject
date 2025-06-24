using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string additionalText;
    public string noteContents;
    public Sprite icon;
    public GameObject prefab;
    public bool isUsable;

    public InventoryItem(string name, Sprite icon = null, GameObject prefab = null, bool isUsable = true, string additionalText = "", string noteContents = "")
    {
        itemName = name;
        this.icon = icon;
        this.prefab = prefab;
        this.isUsable = isUsable;
        this.additionalText = additionalText;
        this.noteContents = noteContents;
    }
}
