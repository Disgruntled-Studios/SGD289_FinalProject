using UnityEngine;

public class NameScript : MonoBehaviour
{
    [SerializeField] private float waitDuration;
    private float waitTimer;
    [SerializeField] private float timeOnScreen = 2f;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private GameObject name;

    private bool onScreen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (anim == null)
        {
            anim = name.gameObject.GetComponent<Animator>();
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
                FadeOutText();
                print("fading out");
            }
        }
        
    }

    public void Visible()
    {
        //anim.SetTrigger("Invisible");
    }

    public void FadeInText()
    {
        name.SetActive(true);
        anim.SetTrigger("FadeInTrigger");
        waitDuration = Time.deltaTime + timeOnScreen;
        onScreen = true;
    }


    public void FadeOutText()
    {
        anim.SetTrigger("FadeOutTrigger");
    }

    public void EnemyStop()
    {
        CreditsEnemy ce = GameObject.Find("Enemy1").GetComponent<CreditsEnemy>();
        ce.StopMovement();
    }




}
