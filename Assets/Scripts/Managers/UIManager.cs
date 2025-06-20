using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD Elements")] 
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _cameraText;
    [SerializeField] private TMP_Text _ammoCountText;

    [Header("Puzzle UI Elements")] 
    [SerializeField] private TMP_Text _tileMoveInstructions;
    [SerializeField] private TMP_Text _tileRotateInstructions;
    [SerializeField] private TMP_Text _puzzleInstructions;
    [SerializeField] private GameObject _puzzlePanel;

    [Header("Inventory UI Elements")] 
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private Transform _inventorySlotParent;
    private readonly List<GameObject> _inventorySlots = new();
    private int _selectedInventoryIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region HUD Methods

    public void UpdateHealthText(float health)
    {
        if (_healthText)
        {
            _healthText.text = $"Health: {health}";
        }
    }

    public void UpdateCameraText(string camName)
    {
        if (_cameraText)
        {
            _cameraText.text = camName;
        }
    }

    public void UpdateAmmoText(int current, int max)
    {
        if (_ammoCountText)
        {
            _ammoCountText.text = $"Ammo: {current}/{max}";
        }
    }

    public void ShowReloading()
    {
        if (_ammoCountText)
        {
            _ammoCountText.text = "Reloading";
        }
    }

    #endregion

    #region Puzzle Methods

    public void SetPuzzlePanelActive(bool isActive)
    {
        if (_puzzlePanel)
        {
            _puzzlePanel.SetActive(isActive);
        }
    }

    public void UpdatePuzzleInstructions(string instructions)
    {
        if (_puzzleInstructions)
        {
            _puzzleInstructions.text = instructions;
        }
    }

    public void UpdateTileControls(string moveText, string rotateText)
    {
        if (_tileMoveInstructions)
        {
            _tileMoveInstructions.text = moveText;
        }

        if (_tileRotateInstructions)
        {
            _tileRotateInstructions.text = rotateText;
        }
    }

    #endregion

    #region Inventory Methods

    public void RefreshInventoryUI(IReadOnlyList<InventoryItem> items)
    {
        foreach (var obj in _inventorySlots)
        {
            Destroy(obj);
        }
        
        _inventorySlots.Clear();

        foreach (var item in items)
        {
            var obj = Instantiate(_inventorySlotPrefab, _inventorySlotParent);
            var controller = obj.GetComponent<InventorySlotController>();
            if (controller)
            {
                controller.SetSlot(item);
            }

            _inventorySlots.Add(obj);
        }

        if (_inventorySlots.Count > 0)
        {
            _selectedInventoryIndex = 0;
            HighlightInventorySlot(_selectedInventoryIndex);
        }
    }

    public void NavigateInventory(int direction)
    {
        if (_inventorySlots.Count == 0) return;

        _selectedInventoryIndex = (_selectedInventoryIndex + direction + _inventorySlots.Count) % _inventorySlots.Count;
        HighlightInventorySlot(_selectedInventoryIndex);
    }

    public InventoryItem GetSelectedInventoryItem(IReadOnlyList<InventoryItem> items)
    {
        if (items.Count == 0) return null;
        return items[_selectedInventoryIndex];
    }

    private void HighlightInventorySlot(int index)
    {
        for (var i = 0; i < _inventorySlots.Count; i++)
        {
            var controller = _inventorySlots[i].GetComponent<InventorySlotController>();
            if (controller)
            {
                controller.SetHighlighted(i == index);
            }
        }
    }

    #endregion
}

