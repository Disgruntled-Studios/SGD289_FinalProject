using UnityEngine;
using System.Collections.Generic;

public class CometStart : MonoBehaviour
{

    [SerializeField]
    private Transform topRightStartPos;
    [SerializeField]
    private Transform botRightStartPos;

    [SerializeField]
    private int cometMaxSpawnNum;

    private List<GameObject> Comets = new List<GameObject>();

    [SerializeField]
    private GameObject comet;

    [SerializeField]
    private float timeBetweenComets = 0.3f;

    //used for generating a random time in between spawns of comets. May add later if time using waitTime mechanic to be more specific with comet spawns.
    //[SerializeField]
    //private float maxTimeBetweenComets=20f;
    //private float minTimeBetweenComets = 10f;

    //when player enters trigger. Comets start generating
    //should comets appear at top of level and aim at player or just off screen of the camera from the right. Second one for now.
    //I think I will try to find where off screen is.

    //start a coroutine or timer system where it instantiates comets one or two at a time, every minTime-maxTime seconds using random x value within bounds that are children of the camera.
    //Use object pooling so that comets will disappear when they reach left side of start screen OR use range bounds somehow.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //0.1 for start time to prevent potential issues with 0.
            InvokeRepeating("SpawnComet", 0.1f, timeBetweenComets);
        }
    }

    private void SpawnComet()
    {
        float randY = Random.Range(botRightStartPos.position.y, topRightStartPos.position.y+1);

        Vector3 plotPoint = new Vector3(botRightStartPos.position.x, randY, botRightStartPos.position.z);

        Instantiate(comet, plotPoint, Quaternion.identity);
        //rolls how many comets can be spawned at the same time. chooses to spawn between 1 and 2 comets.
        //int cometNum = Random.Range(1, cometMaxSpawnNum + 1);


        //int randX = Random.Range()

        //Instantiate(comet, plotPoint, Quaternion.identity);

    }
}
