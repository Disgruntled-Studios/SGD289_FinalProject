using UnityEngine;

// Interface for interactable objects
// Have any interactable object script inherit from this
// See: MirrorController

public interface IInteractable
{
    void Interact(Transform player);
    void OnExit();
}
