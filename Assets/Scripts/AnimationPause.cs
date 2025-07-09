using UnityEngine;

public class AnimationPause : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        anim.speed = 0;
    }

    [ContextMenu("Unpause")]
    public void Unpause()
    {
        anim.speed = 1;
    }
}
