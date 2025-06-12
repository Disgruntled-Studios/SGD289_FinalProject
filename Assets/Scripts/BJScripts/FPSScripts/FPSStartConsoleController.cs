using UnityEngine;

public class FPSStartConsoleController : MonoBehaviour, IInteractable
{
    private bool _hasStarted;
    
    public void Interact(Transform player)
    {
        if (_hasStarted) return;

        _hasStarted = true;
        FPSManager.Instance.StartSimulation();
    }

    public void OnExit()
    {
        return;
    }
}
