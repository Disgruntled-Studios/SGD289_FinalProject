using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIControls : MonoBehaviour
{
    [SerializeField] private InventoryUI _ui;
    [SerializeField] private PlayerInventory _inventory;

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

        if (input.y > 0.1f)
        {
            _ui.Navigate(-1);
        }
        else if (input.y < -0.1f)
        {
            _ui.Navigate(1);
        }
    }

    // AKA USE ITEM
    public void OnInventorySubmit(InputAction.CallbackContext context)
    {
        if (!context.performed || !InputManager.Instance.IsInUI) return;

        var selectedItem = _ui.GetSelectedItem(_inventory.Items);
        if (selectedItem == null) return;

        if (GameManager.Instance.PlayerController.CurrentItemReceiver != null)
        {
            if (GameManager.Instance.PlayerController.CurrentItemReceiver.TryReceiveItem(_inventory, selectedItem))
            {
                Debug.Log($"Item: {selectedItem.itemName} used on receiver");
            }
            else
            {
                // Don't unpause? 
                Debug.Log($"Receiver did not accept item {selectedItem.itemName}");
            }
        }
        else
        {
            var dropPos = GameManager.Instance.Player.transform.position +
                          GameManager.Instance.Player.transform.forward;
            _inventory.DropItem(selectedItem, dropPos);
            Debug.Log($"Dropped item {selectedItem.itemName}");
        }

        _ui.RefreshUI(_inventory.Items);
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
        _ui.RefreshUI(_inventory.Items);
        _ui.gameObject.SetActive(true);
        InputManager.Instance.SwitchToUIInput();
    }

    private void RefreshInventoryUI()
    {
        _ui.RefreshUI(_inventory.Items);
    }
}
