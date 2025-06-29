using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
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

        if (_ui.IsOnSettingsPanel)
        {
            var settings = _ui.SettingsUIController;

            if (settings.FocusState == SettingsFocusState.MainButtons)
            {
                if (input.y > 0.1f)
                {
                    settings.NavigateSidebar(-1);
                }
                else if (input.y < -0.1f)
                {
                    settings.NavigateSidebar(1);
                }
                else if (input.x > 0.5f)
                {
                    settings.EnterSubPanel();
                }
            }
            else if (settings.FocusState == SettingsFocusState.SubPanel)
            {
                if (input.y > 0.1f)
                {
                    settings.NavigateSubPanel(-1);
                }
                else if (input.y < -0.1f)
                {
                    settings.NavigateSubPanel(1);
                }
                else if (input.x > 0.1f)
                {
                    settings.AdjustCurrentElement(1);
                }
                else if (input.x < -0.1f)
                {
                    settings.AdjustCurrentElement(-1);
                }
            }

            return;
        }

        if (_ui.IsOnInventoryPanel)
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

        var settings = _ui.SettingsUIController;

        if (_ui.IsOnSettingsPanel && settings.FocusState == SettingsFocusState.SubPanel)
        {
            settings.ExitSubPanel();
            return;
        }
        
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
