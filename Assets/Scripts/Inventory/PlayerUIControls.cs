using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIControls : MonoBehaviour
{
    [SerializeField] private InventoryUI _ui;
    [SerializeField] private PlayerInventory _inventory;

    public void OnInventoryNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

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

    public void OnInventorySubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        var selectedItem = _ui.GetSelectedItem(_inventory.Items);
        if (selectedItem != null)
        {
            _inventory.UseItem(selectedItem);
            _ui.RefreshUI(_inventory.Items);
        }
    }

    public void OnInventoryCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        _ui.gameObject.SetActive(false);
        InputManager.Instance.SwitchToDefaultInput();
    }

    public void OpenInventoryUI()
    {
        _ui.RefreshUI(_inventory.Items);
        _ui.gameObject.SetActive(true);
        InputManager.Instance.SwitchToUIInput();
    }
}
