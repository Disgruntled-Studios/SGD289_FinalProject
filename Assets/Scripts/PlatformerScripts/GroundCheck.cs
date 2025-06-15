using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool canJump = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            print("canJump = true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = false;
            print("canJump = false");
        }
    }
}
