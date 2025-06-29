using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIController
{
    private readonly EventSystem _eventSystem;
    private readonly GameObject _slotPrefab;
    private readonly Transform _slotParent;
    private readonly TMP_Text _descriptionText;
    private readonly int _columns;

    private readonly List<GameObject> _slots = new();
    private int _selectedIndex;
}
