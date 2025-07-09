using UnityEngine;

// This script is only to identify the gun pickup automatically, think of it as an enhanced tag
public class PlayerGun : MonoBehaviour
{
    public bool CanGrab { get; set; }

    public void EnableInteraction()
    {
        CanGrab = true;
    }
}
