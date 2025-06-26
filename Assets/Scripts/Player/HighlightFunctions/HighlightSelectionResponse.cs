//////////////////////////////////////////////
//Assignment/Lab/Project: Portal
//Name: Nathaniel Lester
//Section: SGD.235.4173
//Instructor: Ven Lewis
//Date: 3/12/25
/////////////////////////////////////////////
using Unity.VisualScripting;
using UnityEngine;

internal class HighlightSelectionResponse : MonoBehaviour, ISelectionResponse
{
    [SerializeField] public Material highlightedMaterial;
    private Material defaultMaterial;
    private Material[] defaultMaterials;

    public void OnSelect(Transform selection)
    {
        //Creates a variable to hold the Renderer for the object being highlighted.
        var selectionRenderer = selection.GetComponent<Renderer>();

        //in the case that the object doesn't have a renderer but a child object does then set the reference to that child's renderer.
        if (selectionRenderer == null && selection.GetComponentInChildren<Renderer>() != null)
        {
            selectionRenderer = selection.GetComponentInChildren<Renderer>();
        }

        if (selectionRenderer != null)
        {
            //Saves the default material for deselection 
            defaultMaterials = selectionRenderer.materials;
            //then create a list of materials to add the highlighted material 
            var highlightedMats = new Material[] {highlightedMaterial};
            highlightedMats.AddRange(defaultMaterials);
            //then sets the materials to the new list.
            selectionRenderer.materials = highlightedMats;
        }
    }

    public void OnDeselect(Transform selection)
    {
        //Creates a variable to hold the Renderer for the object being unhighlighted.
        var selectionRenderer = selection.GetComponent<Renderer>();

        //in the case that the object doesn't have a renderer but a child object does then set the reference to that child's renderer.
        if (selectionRenderer == null && selection.GetComponentInChildren<Renderer>() != null)
        {
            selectionRenderer = selection.GetComponentInChildren<Renderer>();
        }

        if (selectionRenderer != null)
        {
            // //Create a new list of materials that holds just the default material.
            // var unhighlightedMats = new Material[] { defaultMaterial };
            //Apply the new list to the materials variable.
            selectionRenderer.materials = defaultMaterials;
            //Debug.Log("Deselecting " + selection.name);
        }
    }
}
