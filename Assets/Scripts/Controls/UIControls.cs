using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class UIControls : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;

    private UIManager _ui;
    
    private bool _noteIsActivated;

    private PlayerInput Input => InputManager.Instance.PlayerInput;

    private void Start()
    {
        _ui = UIManager.Instance;
    }
    
    private void OnEnable()
    {
        var uiMap = Input.UIMap;
        
        uiMap.InventoryNavigate.performed += OnNavigate;
        uiMap.InventorySubmit.performed += OnInventorySubmit;
        uiMap.NextPanel.performed += OnNextPanel;
        uiMap.PreviousPanel.performed += OnPreviousPanel;
        uiMap.InventoryCancel.performed += OnInventoryCancel;
    }

    private void OnDisable()
    { 
        var uiMap = Input.UIMap;
        
        uiMap.InventoryNavigate.performed -= OnNavigate;
        uiMap.InventorySubmit.performed -= OnInventorySubmit;
        uiMap.NextPanel.performed -= OnNextPanel;
        uiMap.PreviousPanel.performed -= OnPreviousPanel;
        uiMap.InventoryCancel.performed -= OnInventoryCancel;
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;

        if (_ui.NoteContents.activeInHierarchy) return;
        
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        var input = context.ReadValue<Vector2>();

        // Settings
        if (_ui.IsOnSettingsPanel)
        {
            if (_ui.CurrentSettingsFocusState == SettingsFocusState.MainButtons)
            {
                if (input.y > 0.1f)
                {
                    _ui.NavigateSettingsSubPanels(-1);
                }
                else if (input.y < -0.1f)
                {
                    _ui.NavigateSettingsSubPanels(1);
                }
                else if (input.x > 0.5f)
                {
                    _ui.EnterActiveSubPanel();
                }
            }
            else if (_ui.CurrentSettingsFocusState == SettingsFocusState.SubPanel)
            {
                switch (_ui.CurrentSettingsPanelType)
                {
                    case SettingsPanelType.Graphics:
                        if (input.y > 0.1f)
                        {
                            _ui.NavigateGraphicsPanel(-1);
                        }
                        else if (input.y < -0.1f)
                        {
                            _ui.NavigateGraphicsPanel(1);
                        }
                        else if (input.x > 0.5f)
                        {
                            _ui.AdjustGraphicsElement(1);
                        }
                        else if (input.x < -0.1f)
                        {
                            _ui.ExitSubPanelToSidebar();
                        }

                        break;
                    case SettingsPanelType.Sound:
                        if (input.y > 0.1f)
                        {
                            _ui.NavigateSoundPanel(-1);
                        }
                        else if (input.y < -0.1f)
                        {
                            _ui.NavigateSoundPanel(1);
                        }
                        else if (input.x > 0.5f)
                        {
                            _ui.AdjustSoundElement(1);
                        }
                        else if (input.x < -0.5f)
                        {
                            _ui.AdjustSoundElement(-1);
                        }
                        else if (input.x < -0.1f)
                        {
                            _ui.ExitSubPanelToSidebar();
                        }

                        break;
                    case SettingsPanelType.Help:
                        break;
                }
            }
        }
        else if (_ui.IsOnInventoryPanel)
        {
            _ui.NavigateInventory(input);
        }
    }

    // AKA USE ITEM
    public void OnInventorySubmit(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        if (_ui.IsOnSettingsPanel)
        {
            var selected = UIManager.Instance.GameEventSystem.currentSelectedGameObject;
            if (selected)
            {
                var button = selected.GetComponent<Button>();
                button.onClick.Invoke();
            }

            return;
        }
        
        if (_noteIsActivated && UIManager.Instance.IsOnInventoryPanel)
        {
            _noteIsActivated = false;
            UIManager.Instance.ToggleNoteContents(_noteIsActivated);
            return;
        }
        
        var selectedItem = _ui.GetSelectedInventoryItem(_inventory.Items);
        if (selectedItem == null) return;

        if (GameManager.Instance.PlayerController.CurrentItemReceiver != null && !selectedItem.isReadable)
        {
            if (GameManager.Instance.PlayerController.CurrentItemReceiver.TryReceiveItem(_inventory, selectedItem))
            {
                UIManager.Instance.StartPopUpText($"{selectedItem.itemName} used on: {GameManager.Instance.PlayerController.CurrentItemReceiver.Name}.");
            }
            else
            {
                // Don't unpause? 
                Debug.Log($"Receiver did not accept item {selectedItem.itemName}");
            }
            
            _ui.RefreshInventoryUI(_inventory.Items);
            UIManager.Instance.ClosePauseMenu();
        }
        else if (selectedItem.isReadable)
        {
            if (UIManager.Instance.IsOnSettingsPanel) return;

            if (_inventory.TryReadItem(selectedItem))
            {
                _noteIsActivated = true;
            }
        }
        else if (selectedItem.isDroppable)
        {
            _inventory.DropItem(selectedItem);
            _ui.RefreshInventoryUI(_inventory.Items);
            UIManager.Instance.ClosePauseMenu();
        }
        else
        {
            Debug.Log("What happened here?");
        }
    }
    
    public void OnInventoryCancel(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        if (_noteIsActivated)
        {
            _noteIsActivated = false;
            _ui.ToggleNoteContents(_noteIsActivated);
        }
        
        _ui.ClosePauseMenu();
        InputManager.Instance.SwitchToDefaultInput();
    }

    public void OnNextPanel(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        _ui.NavigatePanel(1);
    }
    
    public void OnPreviousPanel(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        _ui.NavigatePanel(-1);
    }
}
