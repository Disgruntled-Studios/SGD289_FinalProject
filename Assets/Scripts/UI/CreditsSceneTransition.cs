using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class CreditsSceneTransition : MonoBehaviour
{
    [SerializeField]
    private bool usingCreditVideo = true;
    [SerializeField]
    private float waitTime = 39f;
    private float baseTime = 0f; //holds current time
    private float targetTime; //holds target time
    private float resetTime = 0f; //any delay before credit video starts such as for a cinematic or win screen.


    void Start()
    {
        if(usingCreditVideo == true)
        {
            baseTime = (baseTime + Time.deltaTime) - resetTime;
            targetTime = baseTime + waitTime;
        }
    }

    //Added code to trigger end credits after a specified time if using video instead of an animation.
    void Update()
    {
        if (usingCreditVideo == true)
        {
            baseTime = (baseTime + Time.deltaTime) - resetTime;
            if (baseTime > targetTime)
            {
                EndCredits();
                print("returning to main menu");
            }
        }
    }
    public void EndCredits()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.DestroyUI();
        }
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
