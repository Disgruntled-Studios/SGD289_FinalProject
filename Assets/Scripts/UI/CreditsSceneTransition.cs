using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsSceneTransition : MonoBehaviour
{
    public void EndCredits()
    {
        SceneManager.LoadScene(0);
    }

}
