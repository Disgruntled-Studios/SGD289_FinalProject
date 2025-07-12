using UnityEngine;

public class JobScript : MonoBehaviour
{
    [SerializeField] private float waitDuration;
    private float waitTimer;
    [SerializeField] private float timeOnScreen = 2f;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private GameObject job;

    private bool onScreen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (anim == null)
        {
            anim = job.gameObject.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        waitTimer += Time.deltaTime;

        if (onScreen == true)
        {
            if (waitTimer > waitDuration)
            {
                JobFadeOutText();
                print("fading out");
            }
        }
        
    }

    public void JobFadeInText()
    {
        job.SetActive(true);
        anim.SetTrigger("FadeInTrigger");
        waitDuration = Time.deltaTime + timeOnScreen;
        onScreen = true;
    }


    public void JobFadeOutText()
    {
        anim.SetTrigger("FadeOutTrigger");
    }





}
