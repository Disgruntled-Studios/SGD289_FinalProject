using UnityEngine;
using System.Collections.Generic;

public class CometStart : MonoBehaviour
{

    [SerializeField]
    private Transform topRightStartPos;
    private Transform botRightStartPos;

    private List<GameObject> Comets = new List<GameObject>();

    //when player enters trigger. Comets start generating
    //should comets appear at top of level and aim at player or just off screen of the camera from the right. Second one for now.
    //I think I will try to find where off screen is.

    //start a coroutine or timer system where it instantiates comets one or two at a time, every minTime-maxTime seconds using random x value within bounds that are children of the camera.
    //Use object pooling so that comets will disappear when they reach left side of start screen OR use range bounds somehow.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Instantiate()
        }
    }
}
