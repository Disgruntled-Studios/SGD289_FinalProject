using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum SettingsPanelType
{
    Help,
    Graphics,
    Sound
}

public enum SettingsFocusState
{
    MainButtons,
    SubPanel
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("HUD Elements")] 
    [SerializeField] private GameObject _hudPanel;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _gameOverText;

    [Header("Puzzle UI Elements")] 
    [SerializeField] private TMP_Text _tileMoveInstructions;
    [SerializeField] private TMP_Text _tileRotateInstructions;
    [SerializeField] private TMP_Text _puzzleInstructions;
    [SerializeField] private GameObject _puzzlePanel;

    [Header("Inventory UI Elements")] 
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private Transform _inventorySlotParent;
    [SerializeField] private GameObject _noteContents;
    public GameObject NoteContents => _noteContents;
    [SerializeField] private TMP_Text _noteContentsText;
    [SerializeField] private TMP_Text _itemDescriptionText;
    private readonly List<GameObject> _inventorySlots = new();
    private int _selectedInventoryIndex;
    private PlayerInventory PlayerInventory => GameManager.Instance.PlayerInventory;

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

    [Header("Pause Panel")] 
    [SerializeField] private GameObject _pausePanel;
    public GameObject PausePanel => _pausePanel;
    [SerializeField] private EventSystem _gameEventSystem;
    public EventSystem GameEventSystem => _gameEventSystem;

    [Header("Pause Sub-Panels")] // DOES NOT INCLUDE PUZZLE PANEL AND KEYCODE PANEL
    [SerializeField] private GameObject[] _subPanels; // INCLUDES INVENTORY AND SETTINGS SUB-PANELS
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _inventoryButton;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _settingsButton;
    private int _currentPanelIndex;
    private GameObject _currentHighlightedTab;

    [Header("Settings Sub-Panels")] // ADDITIONAL PANELS ON SETTINGS SCREEN (Graphics, Sounds, etc)
    [SerializeField] private GameObject _helpPanel;
    [SerializeField] private GameObject _graphicsPanel;
    [SerializeField] private GameObject _soundPanel;

    [Header("Settings Sub-Buttons")] 
    [SerializeField] private GameObject _resumeGameButton;
    [SerializeField] private GameObject _helpButton;
    [SerializeField] private GameObject _graphicsButton;
    [SerializeField] private GameObject _soundButton;
    [SerializeField] private GameObject _exitMainMenuButton;
    [SerializeField] private GameObject _exitDesktopButton;
    [SerializeField] private List<Button> _settingsSubButtons;
    private int _selectedSettingsButtonIndex;

    [Header("Graphics Menu Elements")] 
    [SerializeField] private List<Selectable> _graphicsElements;
    private int _currentGraphicsElementIndex;

    [Header("Help Menu Elements")] // None right now
    [SerializeField] private List<Selectable> _helpElements;
    private int _currentHelpElementIndex;

    [Header("Sound Menu Elements")] 
    [SerializeField] private List<Selectable> _soundElements;
    private int _currentSoundElementIndex;
    
    private SettingsFocusState _currentSettingsFocusState = SettingsFocusState.MainButtons;
    public SettingsFocusState CurrentSettingsFocusState => _currentSettingsFocusState;
    private SettingsPanelType _currentSettingsPanelType;
    public SettingsPanelType CurrentSettingsPanelType => _currentSettingsPanelType;
    
    [Header("Graphics")] 
    [SerializeField] private Toggle _fullScreenToggle;

    public bool IsOnInventoryPanel => _currentPanelIndex == 0;
    public bool IsOnSettingsPanel => _currentPanelIndex == 1;
    
    public bool IsGamePaused { get; private set; }
    
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

    private void Start()
    {
        if (PlayerInventory)
        {
            PlayerInventory.OnInventoryChanged += HandleInventoryChanged;
        }

        _fullScreenToggle.isOn = Screen.fullScreen;
    }

    #region UI Navigation

    public void OpenPauseMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _hudPanel.SetActive(false);
        _pausePanel.SetActive(true);
        
        IsGamePaused = true;

        _inventoryPanel.SetActive(true);
        _settingsPanel.SetActive(false);

        HighlightTab(_inventoryButton);

        RefreshInventoryUI(PlayerInventory.Items);

        _gameEventSystem.SetSelectedGameObject(null);

        if (_inventorySlots.Count > 0)
        {
            _selectedInventoryIndex = 0;
            HighlightInventorySlot(_selectedInventoryIndex);
            _gameEventSystem.SetSelectedGameObject(_inventorySlots[0]);
        }
        else
        {
            _itemDescriptionText.gameObject.SetActive(false);
        }

        InputManager.Instance.SwitchToUIInput();
    }

    public void ClosePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _hudPanel.SetActive(true);
        _pausePanel.SetActive(false);
        IsGamePaused = false;
        
        InputManager.Instance.SwitchToDefaultInput();
    }
    
    public void NavigatePanel(int direction)
    {
        _subPanels[_currentPanelIndex].SetActive(false);
        _currentPanelIndex = (_currentPanelIndex + direction + _subPanels.Length) % _subPanels.Length;
        _subPanels[_currentPanelIndex].SetActive(true);

        _gameEventSystem.SetSelectedGameObject(null);

        if (IsOnInventoryPanel)
        {
            _inventoryPanel.SetActive(true);
            _settingsPanel.SetActive(false);

            HighlightTab(_inventoryButton);

            if (_inventorySlots.Count > 0)
            {
                _selectedInventoryIndex = 0;
                HighlightInventorySlot(_selectedInventoryIndex);
                _gameEventSystem.SetSelectedGameObject(_inventorySlots[0]);
            }
        }
        else if (IsOnSettingsPanel)
        {
            _selectedSettingsButtonIndex = 0;

            _currentSettingsPanelType = SettingsPanelType.Help;

            ShowSettingsSubPanel(0);
            
            _settingsPanel.SetActive(true);
            _inventoryPanel.SetActive(false);

            HighlightTab(_settingsButton);
            _gameEventSystem.SetSelectedGameObject(null);
            _gameEventSystem.SetSelectedGameObject(_settingsSubButtons[_selectedSettingsButtonIndex].gameObject);
        }
    }

    public void NavigateSettingsSubPanels(int direction)
    {
        if (_settingsSubButtons.Count == 0) return;

        _selectedSettingsButtonIndex = (_selectedSettingsButtonIndex + direction + _settingsSubButtons.Count) %
                                       _settingsSubButtons.Count;

        var selected = _settingsSubButtons[_selectedSettingsButtonIndex];
        _gameEventSystem.SetSelectedGameObject(null);
        _gameEventSystem.SetSelectedGameObject(selected.gameObject);

        if (selected.gameObject == _helpButton.gameObject)
        {
            _currentSettingsPanelType = SettingsPanelType.Help;
            ShowSettingsSubPanel(0);
        }
        else if (selected.gameObject == _graphicsButton.gameObject)
        {
            _currentSettingsPanelType = SettingsPanelType.Graphics;
            ShowSettingsSubPanel(1);
        }
        else if (selected.gameObject == _soundButton.gameObject)
        {
            _currentSettingsPanelType = SettingsPanelType.Sound;
            ShowSettingsSubPanel(2);
        }
    }

    private void ShowSettingsSubPanel(int index)
    {
        _helpPanel.SetActive(false);
        _graphicsPanel.SetActive(false);
        _soundPanel.SetActive(false);

        switch (index)
        {
            case 0:
                _helpPanel.SetActive(true);
                break;
            case 1:
                _graphicsPanel.SetActive(true);
                break;
            case 2:
                _soundPanel.SetActive(true);
                break;
        }
    }

    public void EnterActiveSubPanel()
    {
        _currentSettingsFocusState = SettingsFocusState.SubPanel;

        switch (_currentSettingsPanelType)
        {
            case SettingsPanelType.Graphics:
                _currentGraphicsElementIndex = 0;
                _gameEventSystem.SetSelectedGameObject(null);
                _gameEventSystem.SetSelectedGameObject(_graphicsElements[_currentGraphicsElementIndex].gameObject);
                break;
            case SettingsPanelType.Sound:
                _currentSoundElementIndex = 0;
                _gameEventSystem.SetSelectedGameObject(null);
                _gameEventSystem.SetSelectedGameObject(_soundElements[_currentSoundElementIndex].gameObject);
                break;
            case SettingsPanelType.Help:
                _currentHelpElementIndex = 0;
                _gameEventSystem.SetSelectedGameObject(null);
                _gameEventSystem.SetSelectedGameObject(_helpElements[_currentHelpElementIndex].gameObject);
                break;
        }
    }

    public void NavigateGraphicsPanel(int direction)
    {
        if (_graphicsElements.Count == 0) return;

        _currentGraphicsElementIndex = (_currentGraphicsElementIndex + direction + _graphicsElements.Count) %
                                       _graphicsElements.Count;

        var selected = _graphicsElements[_currentGraphicsElementIndex];
        _gameEventSystem.SetSelectedGameObject(null);
        _gameEventSystem.SetSelectedGameObject(selected.gameObject);
    }

    public void NavigateSoundPanel(int direction)
    {
        if (_soundElements.Count == 0) return;

        _currentSoundElementIndex =
            (_currentSoundElementIndex + direction + _soundElements.Count) % _graphicsElements.Count;

        var selected = _soundElements[_currentSoundElementIndex];
        _gameEventSystem.SetSelectedGameObject(null);
        _gameEventSystem.SetSelectedGameObject(selected.gameObject);
    }

    private void AdjustSelectable(Selectable selectable, int direction)
    {
        if (!selectable) return;

        if (selectable.TryGetComponent<Slider>(out var slider))
        {
            var step = slider.wholeNumbers ? 1f : slider.maxValue / 20f;
            slider.value = Mathf.Clamp(slider.value + (step * direction), slider.minValue, slider.maxValue);
        }
        else if (selectable.TryGetComponent<Toggle>(out var toggle))
        {
            toggle.isOn = !toggle.isOn;
        }
        else if (selectable.TryGetComponent<Button>(out var button))
        {
            button.onClick.Invoke();
        }
    }

    public void AdjustGraphicsElement(int direction)
    {
        var selected = _graphicsElements[_currentGraphicsElementIndex];
        AdjustSelectable(selected, direction);
    }

    public void AdjustSoundElement(int direction)
    {
        var selected = _soundElements[_currentSoundElementIndex];
        AdjustSelectable(selected, direction);
    }

    public void ExitSubPanelToSidebar()
    {
        _currentSettingsFocusState = SettingsFocusState.MainButtons;

        _gameEventSystem.SetSelectedGameObject(null);
        _gameEventSystem.SetSelectedGameObject(_settingsSubButtons[_selectedSettingsButtonIndex].gameObject);
    }

    private void HighlightTab(GameObject newTab)
    {
        if (_currentHighlightedTab == newTab) return;

        if (_currentHighlightedTab)
        {
            var oldImage = _currentHighlightedTab.GetComponent<Image>();
            if (oldImage)
            {
                oldImage.color = Color.white;
            }
        }

        var newImage = newTab.GetComponent<Image>();
        if (newImage)
        {
            newImage.color = Color.yellow;
        }

        _currentHighlightedTab = newTab;
    }

    public void ApplyGraphics()
    {
        Screen.fullScreen = _fullScreenToggle.isOn;
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

    private void HandleInventoryChanged()
    {
        RefreshInventoryUI(PlayerInventory.Items);
    }

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

            _gameEventSystem.SetSelectedGameObject(null);
            _gameEventSystem.SetSelectedGameObject(_inventorySlots[0]);
            _itemDescriptionText.gameObject.SetActive(true);
            _itemDescriptionText.text = _inventorySlots[0].GetComponent<InventorySlotController>().ItemName;
        }
        else
        {
            _gameEventSystem.SetSelectedGameObject(null);
        }
    }

    public void NavigateInventory(Vector2 input)
    {
        if (_inventorySlots.Count == 0) return;

        const int columns = 3;
        var total = _inventorySlots.Count;
        var rows = Mathf.CeilToInt((float)total / columns);

        var row = _selectedInventoryIndex / columns;
        var col = _selectedInventoryIndex % columns;
        
        if (input.x > 0.5f)
        {
            col += 1;
            if (col >= columns)
            {
                col = 0;
                row = (row + 1) % rows;
            }
        }
        else if (input.x < -0.5f)
        {
            col -= 1;
            if (col < 0)
            {
                col = columns - 1;
                row = (row - 1 + rows) % rows;
            }
        }
        
        if (input.y > 0.5f)
        {
            row -= 1;
            if (row < 0) row = rows - 1;
        }
        else if (input.y < -0.5f)
        {
            row = (row + 1) % rows;
        }

        var newIndex = row * columns + col;
        
        if (newIndex >= total)
        {
            while (col > 0)
            {
                col--;
                newIndex = row * columns + col;
                if (newIndex < total) break;
            }

            if (newIndex >= total) return;
        }

        _selectedInventoryIndex = newIndex;
        HighlightInventorySlot(_selectedInventoryIndex);

        _gameEventSystem.SetSelectedGameObject(null);
        _gameEventSystem.SetSelectedGameObject(_inventorySlots[_selectedInventoryIndex]);
    }

    public InventoryItem GetSelectedInventoryItem(IReadOnlyList<InventoryItem> items)
    {
        return items.Count == 0 ? null : items[_selectedInventoryIndex];
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

        var selectedController = _inventorySlots[index].GetComponent<InventorySlotController>();
        if (selectedController)
        {
            _itemDescriptionText.text = selectedController.ItemName;
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

