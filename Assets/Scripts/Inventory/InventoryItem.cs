using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;

    public InventoryItem(string name, Sprite icon = null, GameObject prefab = null)
    {
        itemName = null;
        this.icon = icon;
        this.prefab = prefab;
    }
}
