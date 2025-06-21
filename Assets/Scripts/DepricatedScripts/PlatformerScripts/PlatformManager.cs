using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlatformManager : MonoBehaviour
{

    //current life count
    [SerializeField]
    public int lives = 3;

    [SerializeField]
    private int score = 0;

    [SerializeField]
    private int coins = 0; //when coins reach coinGoal int, player gets a new life.

    [SerializeField]
    private int coinGoal = 10;

    [SerializeField]
    private float afterDeathTime = 5f;


    [Header("EnemyPoints")]
    public int littleAlienScore = 100;
    public int FlyingAlienScore = 150;
    public int TargetingShipScore = 300;
    //Electric ship can only be dodged and so is worth no points.



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
    private bool lifeCalc = false;

    PlatformingCollisions pCollisions;

    [SerializeField]
    private float gameOverWait;


    [Header("HubTransition")]
    [SerializeField] private string _sceneSwitchName;
    [SerializeField] private string _cameraSwitchId;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisplayLives();
        CalculateScore(score);
        CalculateCoins(0);
        gameOverPanel.SetActive(false);
        gameOverText.SetActive(false);

        if (!player)
        {
            player = GameManager.Instance.Player;
        }
        
        pCollisions = GameObject.Find("Player").GetComponent<PlatformingCollisions>();

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

            //print("coins is" + coins + "and value is " + value);
            coins = coins + value;

            //if player reaches coin goal, coins reset and they get a new life.
            if (coins > (coinGoal-1))
            {
                lives++;
                DisplayLives();
                coins = 0;
            }

            string displayCoins = coins.ToString();

            if (displayCoins.Length < 2)
            {
                string zero = "0";
                displayCoins = zero + displayCoins;
                //print("displayCoins " + displayCoins);
            }

            coinsText.text = "Coins     " + displayCoins;
            StartCoroutine(Wait(0.1f, "coin"));

            //print("displayed coin count. Coins = " + displayCoins);
        }
    }

    IEnumerator Wait(float waitTime, string type)
    {
        yield return new WaitForSeconds(waitTime);

        if (type == "coin")
        {
            coinCollecting = false;
        }
        else if (type == "life")
        {
            lifeCalc = false;
        }
    }



    public void HandleDamage()
    {
        //makes sure player doesn't lose multiple lives at once.
        if (lifeCalc == false && (pCollisions.invincible ==false))
        {
            lifeCalc = true;
            lives--;
            if(lives<0)
            {
                lives = 0;
            }
            DisplayLives();
            print("player lives is " + lives);
            CheckGameOver();
            //sound effect or flinch, etc
            StartCoroutine(Wait(0.1f, "life"));
        }
    }

    public void DisplayLives()
    {
        livesText.text = "Lives: " + lives;
    }


    public void CheckGameOver()
    {
        if(lives < 1)
        {
            //HandleGameOverScreen();
            print("player is dead");
            gameOverText.SetActive(true);

            PlatformingCollisions pc = GameObject.Find("Player").GetComponent<PlatformingCollisions>();
            if (pc.hasShip)
            {
                pc.ExitShip();
                print("exited ship");
            }

            StartCoroutine("GameOver");
        }
        else
        {
            //player becomes invincible for a few seconds and flashes. Code that in player collisions.
            pCollisions.BecomeInvincibleDamage();
        }
    }

    IEnumerator GameOver()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(gameOverWait);

        try
        {
            ReturnToHub();
        }
        catch
        {
            ResetGame();
        }

        //gameOverPanel.SetActive(true);
        //play sound effect.
    }

    public void ReturnToHub()
    {
        PlatformingCollisions pc = GameObject.Find("Player").GetComponent<PlatformingCollisions>();
        if (pc.hasShip)
        {
            pc.ExitShip();
            print("exited ship");
        }

        if (player.transform.localScale.z < 1)
        {
            var scale = player.transform.localScale;
            scale.z = 1;
            player.transform.localScale = scale;
        }
        
        TransitionManager.Instance.TransitionToScene(_sceneSwitchName, _cameraSwitchId);
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
