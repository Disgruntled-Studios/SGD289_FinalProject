using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{

    [SerializeField]
    private Transform eShipStartPos;
    [SerializeField]
    private Transform tShipStartPos;

    [SerializeField]
    private int maxShips = 2;

    //private List<GameObject> Ships = new List<GameObject>();

    [SerializeField]
    private int shipCount = 0;

    [SerializeField]
    private GameObject eShip;

    [SerializeField]
    private GameObject tShip;

    [SerializeField]
    private float timeBetweenShips;

    [SerializeField]
    private bool randomShips = true;

    [SerializeField]
    private bool testingTargettingShip = true;



    //if time, turn this into a timer.

    //start a coroutine or timer system where it instantiates comets one or two at a time, every minTime-maxTime seconds using random x value within bounds that are children of the camera.
    //Use object pooling so that comets will disappear when they reach left side of start screen OR use range bounds somehow.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //0.1 for start time to prevent potential issues with 0.
            //consider changing later
            InvokeRepeating("SpawnShip", 0.1f, timeBetweenShips);
        }
    }

    private void SpawnShip()
    {
        //if there are not max number of ships already on the field.
        if (shipCount < maxShips)
        {
            shipCount++;
            //rolls to find ship type

            int type = Random.Range(0, 1 + 1);

            if (randomShips)
            {
                return;
            }
            else if (testingTargettingShip)
            {
                type = 1;
            }
            else if (testingTargettingShip == false)
            {
                type = 0;
            }

            if (type == 0) //electric ship
            {
                Instantiate(eShip, eShipStartPos.position, Quaternion.identity);
            }
            else if (type == 1) //targeting ship
            {
                Instantiate(tShip, tShipStartPos.position, Quaternion.identity);
            }
        }

    }

    public void DecreaseShipCount()
    {
        shipCount--;
        if(shipCount <0)
        {
            shipCount = 0;
        }
    }


}