using UnityEngine;
using UnityEngine.InputSystem;

public class UIControls : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;
    
    private UIManager _ui;
    private UIAudioController _uiAudio;
    
    private PlayerInput Input => InputManager.Instance.PlayerInput;
    
    private void Start()
    {
        _ui = UIManager.Instance;
        _uiAudio = _ui.UIAudioController;
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
        if (!ShouldProcessInput(context)) return;

        var input = context.ReadValue<Vector2>();
        _ui.GetActivePanelController()?.HandleNavigation(input);
    }
    
    // AKA USE ITEM
    public void OnInventorySubmit(InputAction.CallbackContext context)
    {
        if (!ShouldProcessInput(context, isSubmit: true)) return;
        
        var intro = UIManager.Instance.IntroController;
        if (intro && intro.ShouldHandleSubmit())
        {
            intro.OnContinuePressed();
            return;
        }
        
        _ui.GetActivePanelController()?.HandleSubmit();
    }
    
    public void OnInventoryCancel(InputAction.CallbackContext context)
    {
        if (!ShouldProcessInput(context)) return;
        
        _ui.GetActivePanelController()?.HandleCancel();
    }
    
    public void OnNextPanel(InputAction.CallbackContext context)
    {
        if (!ShouldProcessInput(context)) return;

        _ui.NavigatePanel(1);
    }
    
    public void OnPreviousPanel(InputAction.CallbackContext context)
    {
        if (!ShouldProcessInput(context)) return;

        _ui.NavigatePanel(-1);
    }

    private bool ShouldProcessInput(InputAction.CallbackContext context, bool isSubmit = false)
    {
        if (!context.performed) return false;

        var intro = UIManager.Instance.IntroController;
        if (isSubmit && intro && intro.ShouldHandleSubmit())
        {
            return true;
        }
        
        if (InputManager.Instance.ShouldBlockInput(context)) return false;
        if (!_ui || !InputManager.Instance.IsInUI) return false;

        return true;
    }
}
