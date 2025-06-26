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
    [SerializeField] private string selectableTag = "Interactable";
    [Tooltip("The Layer where the Objects we want to Highlight is on.")]
    [SerializeField] private LayerMask selectableLayer;
    [Tooltip("This is the reference to the player interact system.")]

    private ISelectionResponse _selectionResponse;

    private Transform _selection;

    void Awake()
    {
        _selectionResponse = GetComponent<ISelectionResponse>();
    }

    // Update is called once per frame
    void Update()
    {
        //Deselection Response
        if (_selection != null)
        {
            _selectionResponse.OnDeselect(_selection);
        }

        //Selection Response
        if (_selection != null)
        {
            _selectionResponse.OnSelect(_selection);
        }
    }

    public void SetSelection(GameObject selected)
    {
        if (selected.GetComponent<IInteractable>() != null)
        {
            _selection = selected.transform;
        }
    }
}