using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("HUD Elements")] 
    [SerializeField] private GameObject _hudPanel;

    [Header("Puzzle UI Elements")] 
    [SerializeField] private TMP_Text _tileMoveInstructions;
    [SerializeField] private TMP_Text _tileRotateInstructions;
    [SerializeField] private TMP_Text _puzzleInstructions;
    [SerializeField] private GameObject _puzzlePanel;
    
    [Header("Inventory UI Elements")] 
    [SerializeField] private List<InventorySlotController> _inventorySlots;
    [SerializeField] private GameObject _noteContents;
    public GameObject NoteContents => _noteContents;
    [SerializeField] private TMP_Text _noteContentsText;
    [SerializeField] private TMP_Text _itemDescriptionText;
    [SerializeField] private TMP_Text _promptInstructionsText;
    private PlayerInventory PlayerInventory => GameManager.Instance.PlayerInventory;

    [Header("Keycode UI Elements")] 
    [SerializeField] private GameObject _keycodePanel;
    [SerializeField] private TMP_Text _keycodePrompt;
    [SerializeField] private List<TMP_Text> _digitDisplays;
    
    [Header("Popup Window")] 
    [SerializeField] private GameObject _popUpBox;
    [SerializeField] private TMP_Text _popUpText;

    [Header("Pause Panel")] 
    [SerializeField] private GameObject _pausePanel;
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

    [Header("Graphics Menu Elements")] 
    [SerializeField] private List<Selectable> _graphicsElements;

    [Header("Help Menu Elements")] // None right now
    [SerializeField] private List<Selectable> _helpElements;

    [Header("Sound Menu Elements")] 
    [SerializeField] private List<Selectable> _soundElements;

    [Header("Audio")] 
    [SerializeField] private UIAudioController _uiAudio;
    public UIAudioController UIAudioController => _uiAudio;
    
    public bool IsOnInventoryPanel => _currentPanelIndex == 0;
    public bool IsOnSettingsPanel => _currentPanelIndex == 1;
    
    public bool IsGamePaused { get; private set; }

    private SettingsUIController _settingsUIController;
    public SettingsUIController SettingsUIController => _settingsUIController;

    private InventoryUIController _inventoryUIController;
    public InventoryUIController InventoryUIController => _inventoryUIController;

    private KeycodeUIController _keycodeUIController;
    public KeycodeUIController KeycodeUIController => _keycodeUIController;

    private Coroutine _popUpCoroutine;
    
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

        _settingsUIController = new SettingsUIController(_gameEventSystem, _settingsSubButtons,
            _helpButton.GetComponent<Button>(), _graphicsButton.GetComponent<Button>(),
            _soundButton.GetComponent<Button>(), _helpPanel, _graphicsPanel, _soundPanel, _settingsButton,
            _graphicsElements, _soundElements);

        _inventoryUIController = new InventoryUIController(_gameEventSystem, _inventorySlots,
            _itemDescriptionText, _promptInstructionsText);

        _keycodeUIController = new KeycodeUIController(_keycodePanel, _keycodePrompt, _digitDisplays);
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

        _inventoryUIController.Refresh(PlayerInventory.Items);

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
        if (_noteContents.activeSelf)
        {
            _noteContents.SetActive(false);
        }
        
        _subPanels[_currentPanelIndex].SetActive(false);
        _currentPanelIndex = (_currentPanelIndex + direction + _subPanels.Length) % _subPanels.Length;
        _subPanels[_currentPanelIndex].SetActive(true);

        _gameEventSystem.SetSelectedGameObject(null);

        if (IsOnInventoryPanel)
        {
            _inventoryPanel.SetActive(true);
            _settingsPanel.SetActive(false);

            HighlightTab(_inventoryButton);

            _inventoryUIController.Refresh(PlayerInventory.Items);
        }
        else if (IsOnSettingsPanel)
        {
            _settingsPanel.SetActive(true);
            _inventoryPanel.SetActive(false);

            HighlightTab(_settingsButton);
            _settingsUIController.Reset();
        }
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
        _inventoryUIController.Refresh(PlayerInventory.Items);
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
        if (_popUpCoroutine != null)
        {
            StopCoroutine(_popUpCoroutine);
        }
        
        _popUpCoroutine = StartCoroutine(TypePopUpText(message));
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
        _popUpCoroutine = null;
    }

    #endregion

    #region Keycode Methods

    public void OpenKeycodePanel(KeycodeReceiver receiver, string prompt = "Enter Keycode: ")
    {
        _keycodeUIController.Open(receiver, prompt);
    }

    public void CloseKeycodePanel()
    {
        _keycodeUIController.Close();
    }

    public void SubmitKeycode()
    {
        _keycodeUIController.Submit();
    }

    public void NavigateKeycodeDigits(Vector2 input)
    {
        _keycodeUIController.Navigate(input);
    }

    public void ShowInvalidCodeFeedback()
    {
        _keycodeUIController.ShowInvalidFeedback();
    }

    #endregion
}

