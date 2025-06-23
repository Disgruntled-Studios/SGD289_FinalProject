using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("BJ_Hub");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
