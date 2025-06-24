using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControls : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;

    private UIManager _ui;

    private void Start()
    {
        _ui = UIManager.Instance;
    }
    
    private void OnEnable()
    {
        _inventory.OnInventoryChanged += RefreshInventoryUI;
    }

    private void OnDisable()
    {
        _inventory.OnInventoryChanged -= RefreshInventoryUI;
    }

    public void OnInventoryNavigate(InputAction.CallbackContext context)
    {
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
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        var selectedItem = _ui.GetSelectedInventoryItem(_inventory.Items);
        if (selectedItem == null) return;

        if (GameManager.Instance.PlayerController.CurrentItemReceiver != null)
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
        }
        else
        {
            _inventory.DropItem(selectedItem);
        }

        _ui.RefreshInventoryUI(_inventory.Items);
        GameManager.Instance.TogglePauseGame();
    }
    
    // public void OnInventoryCancel(InputAction.CallbackContext context)
    // {
    //     if (!context.performed || !InputManager.Instance.IsInUI) return;
    //
    //     _ui.gameObject.SetActive(false);
    //     InputManager.Instance.SwitchToDefaultInput();
    // }

    public void OpenInventoryUI()
    {
        _ui.RefreshInventoryUI(_inventory.Items);
        _ui.gameObject.SetActive(true);
        InputManager.Instance.SwitchToUIInput();
    }

    private void RefreshInventoryUI()
    {
        _ui.RefreshInventoryUI(_inventory.Items);
    }

    public void OnNextPanel(InputAction.CallbackContext context)
    {
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        _ui.NavigatePanel(1);
    }
    
    public void OnPreviousPanel(InputAction.CallbackContext context)
    {
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        _ui.NavigatePanel(-1);
    }
}
