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

    //Timer:
    [SerializeField]
    private int timer;
    [SerializeField]
    private TMP_Text timeText;
    private float timeCounter;
    [SerializeField]
    private float resetTime = 0;


    //invincibility time
    [SerializeField]
    private float iTime;
    private bool invincible = false;


    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject gameOverPanel;



    [Header("TextFields")]
    [SerializeField]
    private TMP_Text nameText;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text coinsText;
    [SerializeField]
    private GameObject gameOverText;
    [SerializeField]
    private TMP_Text livesText;

    private bool coinCollecting = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lives = startLives;
        livesText.text = "Lives: " + lives;
        score = 0;
        CalculateScore(0);
        coins = 0;
        CalculateCoins(0);
        gameOverPanel.SetActive(false);
        gameOverText.SetActive(false);
    }

    void Update()
    {
        //timer
        timeCounter = (timeCounter + Time.deltaTime)-resetTime;
        //timeCounter = timer - timeCounter;

        //print(timeCounter);
        UpdateTimer();

    }

    private void UpdateTimer()
    {
        int time = Mathf.FloorToInt(timeCounter);
        time = timer - time;
        string timeString = time.ToString();

        //print("timeCounter is" +timeCounter);
        //print("time.delta time is " + Time.deltaTime);

/*
        if (timeString.Length < 3)
        {
            string zero = "0";
            while (timeString.Length < 3)
            {
                timeString = zero + timeString;
            }
            //print("time is " + timeString);
        }
*/

        timeText.text = timeString;

    }

    public void CalculateScore(int value)
    {
        score = score + value;

        if(score > 999999)
        {
            score = 999999;
        }

        string displayScore = score.ToString();

        while(displayScore.Length < 6)
        {
            string zero = "0";
            displayScore = zero + displayScore;
           // print("displayScore is " + displayScore);
        }

        scoreText.text = displayScore;
    }

    public void CalculateCoins(int value)
    {
        if (coinCollecting == false)
        {
            coinCollecting = true;
            print("coins is" + coins + "and value is " + value);
            coins = coins + value;

            if (coins > 99)
            {
                coins = 99;
            }

            string displayCoins = coins.ToString();

            if (displayCoins.Length < 2)
            {
                string zero = "0";
                displayCoins = zero + displayCoins;
                //print("displayCoins " + displayCoins);
            }

            coinsText.text = "Coins     " + displayCoins;
            StartCoroutine(Wait());

            //print("displayed coin count. Coins = " + displayCoins);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        coinCollecting = false;
    }


    public void HandleDamage()
    {
        lives--;
        print("player lives is " + lives);
        CheckGameOver();
        //sound effect or flinch, etc.
    }

    public void BecomeInvincible(float time)
    {
        if(invincible)
        {
            //make player flash.
            //Make it so enemy doesn't harm player
        }
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
            BecomeInvincible(iTime);
        }
    }

    IEnumerator GameOver()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(5);

        gameOverPanel.SetActive(true);
        //play sound effect.

    }

    public void ResetGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
        print("Quit button works");
    }

    public void PlayAgainButton()
    {
        ResetGame();
    }







}
