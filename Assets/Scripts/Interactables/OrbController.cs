using UnityEngine;

public class OrbController : MonoBehaviour, IInteractable
{
    public void Interact(Transform player, PlayerInventory inventory)
    {
        Debug.Log("Talking to orb");
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
