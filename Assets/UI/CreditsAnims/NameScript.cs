using UnityEngine;

public class NameScript : MonoBehaviour
{
    [SerializeField] private float waitDuration;
    private float waitTimer;
    [SerializeField] private float timeOnScreen = 2f;

    [SerializeField]
    private Animator anim;

    private bool OnScreen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (anim == null)
        {
            anim = this.gameObject.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        waitTimer += Time.deltaTime;
        if(waitTimer > waitDuration)
        {
            print("waitTimer is larger than waitDuration");
            if (OnScreen == false)
            {
                FadeInText();
                waitDuration = Time.deltaTime + timeOnScreen;
                print("fading in");
                OnScreen = true;
            }
            else
            {
                FadeOutText();
                print("fading out");
            }
        }
    }

    public void FadeInText()
    {
        anim.SetTrigger("FadeInTrigger");
    }

    public void FadeOutText()
    {
        anim.SetTrigger("FadeOutTrigger");
    }




}
