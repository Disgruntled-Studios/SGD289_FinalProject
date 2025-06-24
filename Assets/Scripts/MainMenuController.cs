using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("L1PowerPlant");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
