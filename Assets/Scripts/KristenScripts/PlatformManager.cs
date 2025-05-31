using System.Runtime.CompilerServices;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{

    [SerializeField]
    private int startLives = 3;

    [SerializeField]
    private int currentLives = 3;

    [SerializeField]
    private int score = 0;

    [SerializeField]
    private int coins = 0; //when coins reach coinGoal int, player gets a new life.

    [SerializeField]
    private int coinGoal = 20;


    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Transform playerStartPosition;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLives = startLives;
        score = 0;
        coins = 0;
    }

    public void CheckGameOver()
    {
        if(currentLives < 1)
        {
            //HandleGameOverScreen();
        }
        else
        {
            //UpdateUI();
            ResetGame();
        }
    }

    private void ResetGame()
    {
        //place player at start, activate all enemies within range--second part may be automatic once I get that coding working.
        

    }


}
