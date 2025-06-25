using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField] private GameObject _noteContents;
    [SerializeField] private TMP_Text _noteContentsText;
    private readonly List<GameObject> _inventorySlots = new();
    private int _selectedInventoryIndex;

    [Header("Keycode UI Elements")] 
    [SerializeField] private GameObject _keycodePanel;
    [SerializeField] private TMP_Text _keycodePrompt;
    [SerializeField] private List<TMP_Text> _digitDisplays;
    private KeycodeReceiver _activeKeycodeReceiver;
    private int[] _currentDigits;
    private int _activeDigitIndex;
    
    [Header("Popup Window")] 
    [SerializeField] private GameObject _popUpBox;
    [SerializeField] private TMP_Text _popUpText;
    
    [Header("Main UI Panels")] // DOES NOT INCLUDE PUZZLE PANEL AND KEYCODE PANEL
    [SerializeField] private GameObject[] _subPanels; // INCLUDES INVENTORY AND SETTINGS SUB-PANELS
    private int _currentPanelIndex;

    public bool IsOnInventoryPanel => _currentPanelIndex == 0;
    public bool IsOnSettingsPanel => _currentPanelIndex == 1;
    
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

    #region General UI

    public void NavigatePanel(int direction)
    {
        _subPanels[_currentPanelIndex].SetActive(false);
        _currentPanelIndex = (_currentPanelIndex + direction + _subPanels.Length) % _subPanels.Length;
        _subPanels[_currentPanelIndex].SetActive(true);

        if (IsOnSettingsPanel && _noteContents.activeSelf)
        {
            ToggleNoteContents(false);
        }
    }

    #endregion
    
    #region HUD Methods

    public void UpdateHealthText(float health)
    {
        if (_healthText)
        {
            _healthText.text = $"Player Health: {health}";
        }
    }

    public void ToggleHealthText(bool isActive)
    {
        _healthText.gameObject.SetActive(isActive);
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

    public void ToggleNoteContents(bool isActive, string contents = "")
    {
        _noteContents.SetActive(isActive);
        _noteContentsText.text = contents;
    }

    #endregion

    #region PopUp Methods

    public void StartPopUpText(string message)
    {
        StartCoroutine(TypePopUpText(message));
    }

    private IEnumerator TypePopUpText(string message)
    {
        _popUpBox.SetActive(true);
        _popUpText.text = message;

        // var typingSpeed = 0.025f;
        //
        // foreach (var c in message)
        // {
        //     _popUpText.text += c;
        //     yield return new WaitForSeconds(typingSpeed);
        // }
        
        yield return new WaitForSeconds(3f);

        _popUpBox.SetActive(false);
    }

    #endregion

    #region Keycode Methods

    public void OpenKeycodePanel(KeycodeReceiver receiver, string prompt = "Enter Keycode: ")
    {
        _activeKeycodeReceiver = receiver;
        _keycodePrompt.text = prompt;

        _keycodePanel.SetActive(true);
        _activeDigitIndex = 0;

        _currentDigits = new int[_digitDisplays.Count];
        for (var i = 0; i < _digitDisplays.Count; i++)
        {
            _currentDigits[i] = 0;
            _digitDisplays[i].text = "0";
        }

        HighlightActiveDigit();
        InputManager.Instance.SwitchToKeycodeInput();
    }

    public void CloseKeycodePanel()
    {
        if (!InputManager.Instance.IsInKeycode) return;
        _keycodePanel.SetActive(false);
        _activeKeycodeReceiver = null;
        InputManager.Instance.SwitchToDefaultInput();
    }

    public void SubmitKeycode()
    {
        if (_activeKeycodeReceiver == null) return;

        var enteredCode = string.Join("", _currentDigits);
        _activeKeycodeReceiver.SubmitCode(enteredCode);
    }

    public void NavigateKeycodeDigits(Vector2 direction)
    {
        if (!_keycodePanel.activeSelf) return;

        if (direction.x > 0.1f)
        {
            _activeDigitIndex = (_activeDigitIndex + 1) % _digitDisplays.Count;
            HighlightActiveDigit();
        }
        else if (direction.x < -0.1f)
        {
            _activeDigitIndex = (_activeDigitIndex - 1 + _digitDisplays.Count) % _digitDisplays.Count;
            HighlightActiveDigit();
        }
        else if (direction.y > 0.1f)
        {
            _currentDigits[_activeDigitIndex] = (_currentDigits[_activeDigitIndex] + 1) % 10;
            _digitDisplays[_activeDigitIndex].text = _currentDigits[_activeDigitIndex].ToString();
        }
        else if (direction.y < -0.1f)
        {
            _currentDigits[_activeDigitIndex] = (_currentDigits[_activeDigitIndex] - 1 + 10) % 10;
            _digitDisplays[_activeDigitIndex].text = _currentDigits[_activeDigitIndex].ToString();
        }
    }

    public void ShowInvalidKeycodeFeedback()
    {
        // TODO
    }

    private void HighlightActiveDigit()
    {
        for (var i = 0; i < _digitDisplays.Count; i++)
        {
            _digitDisplays[i].color = (i == _activeDigitIndex) ? Color.yellow : Color.white;
        }
    }

    #endregion
}

