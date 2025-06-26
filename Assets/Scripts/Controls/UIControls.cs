using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        uiMap.InventoryNavigate.performed += OnInventoryNavigate;
        uiMap.InventorySubmit.performed += OnInventorySubmit;
        uiMap.NextPanel.performed += OnNextPanel;
        uiMap.PreviousPanel.performed += OnPreviousPanel;
        uiMap.InventoryCancel.performed += OnInventoryCancel;
    }

    private void OnDisable()
    { 
        var uiMap = Input.UIMap;
        
        uiMap.InventoryNavigate.performed -= OnInventoryNavigate;
        uiMap.InventorySubmit.performed -= OnInventorySubmit;
        uiMap.NextPanel.performed -= OnNextPanel;
        uiMap.PreviousPanel.performed -= OnPreviousPanel;
        uiMap.InventoryCancel.performed -= OnInventoryCancel;
    }

    public void OnInventoryNavigate(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;

        if (UIManager.Instance.NoteContents.activeInHierarchy) return;
        
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        var input = context.ReadValue<Vector2>();

        if (input.x > 0.1f)
        {
            _ui.NavigateInventory(-1);
        }
        else if (input.x < -0.1f)
        {
            _ui.NavigateInventory(1);
        }
    }

    // AKA USE ITEM
    public void OnInventorySubmit(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        if (_noteIsActivated && UIManager.Instance.IsOnInventoryPanel)
        {
            _noteIsActivated = false;
            UIManager.Instance.ToggleNoteContents(_noteIsActivated);
            return;
        }
        
        var selectedItem = _ui.GetSelectedInventoryItem(_inventory.Items);
        if (selectedItem == null) return;

        if (GameManager.Instance.PlayerController.CurrentItemReceiver != null && selectedItem.isUsable)
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
        else if (!selectedItem.isUsable)
        {
            if (UIManager.Instance.IsOnSettingsPanel) return;

            if (_inventory.TryReadItem(selectedItem))
            {
                _noteIsActivated = true;
            }
        }
        else
        {
            _inventory.DropItem(selectedItem);
            _ui.RefreshInventoryUI(_inventory.Items);
            UIManager.Instance.ClosePauseMenu();
        }
    }
    
    public void OnInventoryCancel(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.ShouldBlockInput(context)) return;
        
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        Debug.Log("Closing Inventory");

        if (_noteIsActivated)
        {
            _noteIsActivated = false;
            _ui.ToggleNoteContents(_noteIsActivated);
        }
        
        _ui.PausePanel.SetActive(false);
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
