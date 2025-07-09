using System;
using System.Collections;
using UnityEngine;

public class DoorPressureGame : MonoBehaviour, IInteractable
{
    public float doorPressure;
    public float doorPressureReleaseVal = 10;
    public float doorPressureIncreaseVal = 10;
    public float maxPressure = 100;
    public bool hasTimerStarted;
    public bool isReleasingPressure;
    public bool isSinglePress;

    public float escapeTimer;
    public float escapeStartTime;

    public GameObject doorRef;
    public Transform highlightedObj;
    public Transform valveRotationStart;
    public Transform valveRotationEnd;
    public Transform doorStartPos;
    public Transform doorEndPos;
    public Transform doorEndTimerPos;

    public EnemyBehavior[] enemies;

    public SoundComponent soundComponent;
    public string pressureSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!soundComponent)
        {
            soundComponent = GetComponent<SoundComponent>();
        }
        hasTimerStarted = false;
    }

    // Update is called once per frame    
    void Update()
    {
        // if (!hasTimerStarted)
        // {
        //     float pressurePercentage = doorPressure / maxPressure;
        //     doorRef.transform.position = Vector3.Lerp(doorStartPos.position, doorEndPos.position, pressurePercentage);
        //     highlightedObj.transform.rotation = Quaternion.Lerp(valveRotationStart.rotation, valveRotationEnd.rotation, pressurePercentage);
        //     //Debug.Log(pressurePercentage);
        // }
    }

    private IEnumerator SlowlyReleasePressure()
    {
        isReleasingPressure = true;
        while (isReleasingPressure)
        {
            doorPressure -= doorPressureReleaseVal * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        if (doorPressure < 0)
        {
            doorPressure = 0;
        }
        
    }

    public void Interact(Transform player, PlayerInventory inventory)
    {
        if (isSinglePress)
        {
            doorPressure = maxPressure;
            StartCoroutine(EscapeSequence());
            isReleasingPressure = false;
            GameManager.Instance.Player.GetComponent<PlayerController>().currentHighlightedObj = null;
            highlightedObj.transform.rotation = valveRotationEnd.rotation;
            return;
        }

        if (hasTimerStarted) return;

        if (!isReleasingPressure) StartCoroutine(SlowlyReleasePressure());

        if (doorPressure + doorPressureIncreaseVal >= maxPressure)
        {
            doorPressure = maxPressure;
            StartCoroutine(EscapeSequence());
            isReleasingPressure = false;
            GameManager.Instance.Player.GetComponent<PlayerController>().currentHighlightedObj = null;
        }
        else
        {
            doorPressure += doorPressureIncreaseVal;
        }
    }


    private IEnumerator EscapeSequence()
    {
        Debug.Log("Escape Sequence Initiated");
        hasTimerStarted = true;
        escapeTimer = escapeStartTime;
        soundComponent.PlaySFX(pressureSFX);
        foreach (EnemyBehavior enemy in enemies)
        {

            Debug.Log(enemy + " is in the list");
            if (enemy.currentState == EnemyBehavior.BehaviorState.Resting)
            {
                enemy.WakeEnemy();
                Debug.Log(enemy + " has been woken up");
            }
        }

        float escapeLerp;
        while (hasTimerStarted)
        {
            while (UIManager.Instance.IsGamePaused) yield return null;

            escapeTimer -= 1;

            escapeLerp = escapeTimer / escapeStartTime;

            //Debug.Log(escapeLerp);

            if (escapeTimer <= 0)
            {
                Debug.Log("Setting door to start pos");
                escapeTimer = 0;
                hasTimerStarted = false;
                doorRef.transform.position = doorStartPos.position;
            }
            else
            {
                Debug.Log("Handling door movement");
                doorRef.transform.position = Vector3.Lerp(doorEndTimerPos.position, doorEndPos.position, escapeLerp);
            }
            
            yield return new WaitForSeconds(1f);            
        }
    }

    public void OnEnter()
    {
        return;
    }

    public void OnExit()
    {
        return;
    }

}
