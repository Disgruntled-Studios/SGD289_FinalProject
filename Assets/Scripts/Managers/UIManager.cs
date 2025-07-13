using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Main Panels")] // HUD, Pause, Puzzle, Keycode
    [SerializeField] private GameObject _hudPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _puzzlePanel;
    [SerializeField] private GameObject _keycodePanel;

    [Header("Pause Panel Buttons")] 
    [SerializeField] private List<Button> _topButtons;

    [Header("Pause Panels")] // Inventory, Controls, Audio, Visual, More
    [SerializeField] private List<GameObject> _subPanels;

    [Header("Panel Controllers")] 
    [SerializeField] private InventoryUIController _inventoryController;
    [SerializeField] private ControlsUIController _controlsController;
    [SerializeField] private AudioUIController _audioController;
    [SerializeField] private VisualUIController _visualController;
    [SerializeField] private MoreUIController _moreController;
    private List<IUIPanelController> _panelControllers;
    private int _currentPanelIndex;

    [Header("Puzzle UI Elements")] 
    [SerializeField] private TMP_Text _tileMoveInstructions;
    [SerializeField] private TMP_Text _tileRotateInstructions;
    [SerializeField] private TMP_Text _puzzleInstructions;
    
    [Header("Inventory UI Elements")] 
    [SerializeField] private List<InventorySlotController> _inventorySlots;
    public List<InventorySlotController> InventorySlots => _inventorySlots;
    private PlayerInventory PlayerInventory => GameManager.Instance.PlayerInventory;
    [SerializeField] private GameObject _gunImage;

    [Header("Keycode UI Elements")] 
    [SerializeField] private TMP_Text _keycodePrompt;
    [SerializeField] private List<TMP_Text> _digitDisplays;
    [SerializeField] private List<GameObject> _digitIcons;
    
    [Header("Popup Window")] 
    [SerializeField] private GameObject _popUpBox;
    [SerializeField] private TMP_Text _popUpText;

    [Header("Event System")] 
    [SerializeField] private EventSystem _gameEventSystem;
    
    [Header("Audio")] 
    [SerializeField] private UIAudioController _uiAudio;
    public UIAudioController UIAudioController => _uiAudio;

    [Header("Game Over")] 
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private List<Button> _deathPanelButtons;
    [SerializeField] private DeathMenuController _deathMenuController;
    
    public bool IsGamePaused { get; private set; }

    private KeycodeUIController _keycodeUIController;
    public KeycodeUIController KeycodeUIController => _keycodeUIController;

    private Coroutine _popUpCoroutine;
    
    public bool PopUpTypingEnabled { get; set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _panelControllers = new List<IUIPanelController>
        {
            _inventoryController,
            _controlsController,
            _audioController,
            _visualController,
            _moreController
        };
    }

    private void Start()
    {
        if (PlayerInventory)
        {
            PlayerInventory.OnInventoryChanged += HandleInventoryChanged;
        }

        _keycodeUIController = new KeycodeUIController(_keycodePanel, _keycodePrompt, _digitDisplays);

        ToggleGunImage(false);
    }

    #region UI Navigation

    public void OpenPauseMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _hudPanel.SetActive(false);
        _pausePanel.SetActive(true);
        IsGamePaused = true;

        _currentPanelIndex = 0;

        for (var i = 0; i < _subPanels.Count; i++)
        {
            var isActive = i == _currentPanelIndex;
            _subPanels[i].SetActive(isActive);

            if (isActive)
            {
                _panelControllers[i]?.OnPanelActivated();
            }
            else
            {
                _panelControllers[i]?.OnPanelDeactivated();
            }
        }

        HighlightActiveButton(_currentPanelIndex);

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

    public void OpenGameOverScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _hudPanel.SetActive(false);
        _gameOverPanel.SetActive(true);
        
        InputManager.Instance.SwitchToUIInput();

        if (_deathPanelButtons.Count > 0)
        {
            SetEventSystemObject(_deathPanelButtons[0].gameObject);
        }
    }

    public void CloseGameOverScreen()
    {
        _gameOverPanel.SetActive(false);
    }

    private void SetActivePanel(int index)
    {
        if (index == _currentPanelIndex) return;
        
        _panelControllers[_currentPanelIndex]?.OnPanelDeactivated();
        _subPanels[_currentPanelIndex].SetActive(false);

        _currentPanelIndex = index;
        _subPanels[_currentPanelIndex].SetActive(true);
        _panelControllers[_currentPanelIndex]?.OnPanelActivated();

        HighlightActiveButton(index);

        var defaultSelectable = _panelControllers[_currentPanelIndex]?.GetDefaultSelectable();
        if (defaultSelectable)
        {
            SetEventSystemObject(defaultSelectable);
        }
    }

    public void NavigatePanel(int direction)
    {
        var newIndex = (_currentPanelIndex + direction + _subPanels.Count) % _subPanels.Count;
        SetActivePanel(newIndex);
    }

    private void HighlightActiveButton(int index)
    {
        for (var i = 0; i < _topButtons.Count; i++)
        {
            var buttonText = _topButtons[i].gameObject.GetComponentInChildren<TMP_Text>();
            buttonText.rectTransform.localScale = (i == index) ? new Vector3(1.25f, 1.25f, 1.25f) : Vector3.one;
            buttonText.color = i == index ? Color.red : Color.white;
        }
    }

    public IUIPanelController GetActivePanelController()
    {
        if (_pausePanel.activeSelf)
        {
            return _panelControllers[_currentPanelIndex];
        }

        if (_gameOverPanel.activeSelf)
        {
            return _deathMenuController;
        }

        return null;
    }

    public void DestroyUI()
    {
        Destroy(gameObject);
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
        _inventoryController?.RefreshInventory();
    }

    public void ToggleGunImage(bool isActive)
    {
        _gunImage.SetActive(isActive);
    }

    #endregion

    #region PopUp Methods

    // CALL WITH DURATION OF 0 TO HAVE PERMANENT POPUP TEXT IN AREA
    // CLEAR WITH ClearPopUpText()
    public void StartPopUpText(string message, float duration = 3f)
    {
        if (_popUpCoroutine != null)
        {
            StopCoroutine(_popUpCoroutine);
            _popUpCoroutine = null;
        }

        if (duration > 0f)
        {
            _popUpCoroutine = StartCoroutine(TypePopUpText(message, duration));
        }
        else
        {
            ShowPermanentPopUpText(message);
        }
    }

    public void ClearPopUpText()
    {
        if (_popUpCoroutine != null)
        {
            StopCoroutine(_popUpCoroutine);
            _popUpCoroutine = null;
        }

        _popUpBox.SetActive(false);
        _popUpText.text = "";
    }

    private IEnumerator TypePopUpText(string message, float duration)
    {
        _popUpBox.SetActive(true);
        _popUpText.text = "";

        if (PopUpTypingEnabled)
        {
            var typingSpeed = 0.025f;

            foreach (var c in message)
            {
                _popUpText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        else
        {
            _popUpText.text = message;
        }

        yield return new WaitForSeconds(duration);

        _popUpBox.SetActive(false);
        _popUpCoroutine = null;
    }

    private void ShowPermanentPopUpText(string message)
    {
        _popUpBox.SetActive(true);
        _popUpText.text = PopUpTypingEnabled ? "" : message;

        if (PopUpTypingEnabled)
        {
            StartCoroutine(TypeTextWithoutAutoClear(message));
        }
    }

    private IEnumerator TypeTextWithoutAutoClear(string message)
    {
        foreach (var c in message)
        {
            _popUpText.text += c;
            yield return new WaitForSeconds(0.025f);
        }
    }

    #endregion

    #region Keycode Methods

    public void OpenKeycodePanel(KeycodeReceiver receiver, string prompt = "Enter Keycode: ")
    {
        ClearPopUpText();
        
        _keycodeUIController.Open(receiver, prompt);

        foreach (var icon in _digitIcons)
        {
            icon.SetActive(receiver.ShouldShowPipeIcons);
        }
    }

    public void CloseKeycodePanel()
    {
        _keycodeUIController.Close();

        foreach (var icon in _digitIcons)
        {
            icon.SetActive(false);
        }
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

    public void ShakeKeycodePanel(float duration = 0.25f, float magnitude = 10f)
    {
        if (!_keycodePanel) return;

        var rectTransform = _keycodePanel.GetComponent<RectTransform>();
        if (!rectTransform) return;

        StartCoroutine(ShakeRectTransform(rectTransform, duration, magnitude));
    }

    private IEnumerator ShakeRectTransform(RectTransform rectTransform, float duration, float magnitude)
    {
        var elapsed = 0f;
        var originalPosition = rectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            var offset = Random.insideUnitCircle * magnitude;
            rectTransform.anchoredPosition = originalPosition + offset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
    }

    #endregion

    #region Helpers

    public void SetEventSystemObject(GameObject obj)
    {
        if (!obj) return; 
        
        _gameEventSystem.SetSelectedGameObject(null);
        _gameEventSystem.SetSelectedGameObject(obj);
    }

    public GameObject GetCurrentSelectedObject()
    {
        return _gameEventSystem.currentSelectedGameObject;
    }

    #endregion
}

