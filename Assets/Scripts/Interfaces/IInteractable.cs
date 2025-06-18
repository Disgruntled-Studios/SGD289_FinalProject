using UnityEngine;

// Interface for interactable objects
// Have any interactable object script inherit from this
// See: MirrorController

public interface IInteractable
{
    void Interact(Transform player); // Called when player presses interact in the trigger zone
    void OnEnter(); // Called when player enters the trigger zone
    void OnExit(); // Automatically calls when player leaves trigger zone
}
