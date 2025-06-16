using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool canJump = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            Debug.Log(canJump);
            print("canJump = true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = false;
            Debug.Log(canJump);
            print("canJump = false");
        }
    }
}
