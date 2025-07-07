using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.XR.Haptics;

public class MiraScript : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        if (anim == null)
        {
            anim = this.gameObject.GetComponent<Animator>();
        }
    }
    public void PlayDieAnim()
    {
        anim.SetTrigger("Death");
    }
}