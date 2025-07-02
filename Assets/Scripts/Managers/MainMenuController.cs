using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string _firstLevelName = "L1PowerPlant";

    public void StartGame()
    {
        SceneManager.LoadScene(_firstLevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
