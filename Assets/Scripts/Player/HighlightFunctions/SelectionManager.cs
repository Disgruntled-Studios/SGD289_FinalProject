//////////////////////////////////////////////
//Assignment/Lab/Project: Portal
//Name: Nathaniel Lester
//Section: SGD.235.4173
//Instructor: Ven Lewis
//Date: 3/11/25
/////////////////////////////////////////////
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;
    [Tooltip("The Tag that is on the objects we want to Highlight.")]
    [SerializeField] private PlayerController playerRef;

    private HighlightSelectionResponse _selectionResponse;

    private Transform _selection;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _selectionResponse = GetComponent<HighlightSelectionResponse>();
        if (playerRef == null)
        {
            playerRef = FindAnyObjectByType<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Deselection Response
        if (_selection != null)
        {
            //Debug.Log(_selection + " set on Deselection");
            _selectionResponse.OnDeselect(_selection);
        }

        _selection = null;
        if (playerRef.currentHighlightedObj != null)
        {
            _selection = playerRef.currentHighlightedObj;
        }

         //Selection Response
        if (_selection != null)
        {
            //Debug.Log(_selection + " set on selection");
            _selectionResponse.OnSelect(_selection);
        }
    }

    public void SetSelection(GameObject selected)
    {
        if (selected.GetComponent<PickupItem>() != null)
        {
            //Debug.Log(selected.name + " has been selected");
            _selection = selected.transform;
        }
        else if (selected.GetComponentInParent<PickupItem>() != null)
        {
            _selection = selected.transform.parent.transform;
        }
        _selectionResponse.OnSelect(_selection);
    }

    public void WipeSelection()
    {
        if (_selection != null)
        {
            _selectionResponse.OnDeselect(_selection);
            _selection = null;
        }
    }
}