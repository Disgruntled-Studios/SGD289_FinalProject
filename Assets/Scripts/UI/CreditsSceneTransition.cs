using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsSceneTransition : MonoBehaviour
{
    public void EndCredits()
    {
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
