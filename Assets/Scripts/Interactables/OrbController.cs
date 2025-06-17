using UnityEngine;

public class OrbController : MonoBehaviour, IInteractable
{
    public void Interact(Transform player)
    {
        Debug.Log("Talking to orb");
    }

    public void OnExit()
    {
        return;
    }
}
