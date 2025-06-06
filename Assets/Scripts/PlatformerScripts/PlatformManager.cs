using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlatformManager : MonoBehaviour
{
    //starting life count
    [SerializeField]
    private int startLives = 3;

    //current life count
    [SerializeField]
    public int lives = 3;

    [SerializeField]
    private int score = 0;

    [SerializeField]
    private int coins = 0; //when coins reach coinGoal int, player gets a new life.

    [SerializeField]
    private int coinGoal = 20;

    [SerializeField]
    private float afterDeathTime = 5f;


    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Transform playerStartPosition;

    [Header("Health Parameters")]
    [SerializeField]
    private TMP_Text nameText;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text coinText;
    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text timeCounter;
    [SerializeField]
    private GameObject gameOverText;

    [SerializeField]
    private GameObject gameOverPanel;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetGame();

    }

    private void ResetGame()
    {
        lives = startLives;
        score = 0;
        coins = 0;
        gameOverPanel.SetActive(false);
        gameOverText.SetActive(false);
    }

    public void HandleDamage()
    {
        lives--;
        print("player lives is " + lives);
        CheckGameOver();
        //sound effect or flinch, etc.
    }

    public void CheckGameOver()
    {
        if(lives < 1)
        {
            //HandleGameOverScreen();
            print("player is dead");
            gameOverText.SetActive(true);

            StartCoroutine("GameOver");
        }
        else
        {
            //player becomes invincible for a few seconds and flashes. Code that in player collisions.
        }
    }

    IEnumerator GameOver()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(5);

        gameOverPanel.SetActive(true);
        //play sound effect.

    }

    public void OnClickQuitButton()
    {
        Application.Quit();
        print("Quit button works");
    }

    public void PlayAgainButton()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }






}
