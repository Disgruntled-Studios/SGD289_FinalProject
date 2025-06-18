using UnityEngine;

public class FPSStartConsoleController : MonoBehaviour, IInteractable
{
    [SerializeField] private StartDoorController _sdc;
    
    private bool _hasStarted;
    
    public void Interact(Transform player)
    {
        if (_hasStarted) return;

        _hasStarted = true;
        _sdc.StartCountdown();
    }

    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        return;
    }
}
