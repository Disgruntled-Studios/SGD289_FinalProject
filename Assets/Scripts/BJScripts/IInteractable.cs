using UnityEngine;

// Interface for interactable objects
// Have any interactable object script inherit from this
// See: MirrorController

public interface IInteractable
{
    void Interact(Transform player); // Called when player presses interact in the trigger zone
    void OnExit(); // Automatically calls when player leaves trigger zone
}
