using Unity.VisualScripting;
using UnityEngine;

internal class HighlightSelectionResponse : MonoBehaviour, ISelectionResponse
{
    [SerializeField] public Material highlightedMaterial;
    private Material defaultMaterial;

    public void OnSelect(Transform selection)
    {
        if (selection.GetComponent<PowerPuzzleTile>()) return;
        
        //Creates a variable to hold the Renderer for the object being highlighted.
        var selectionRenderer = selection.GetComponent<MeshRenderer>();


        //in the case that the object doesn't have a renderer but a parent object does then set the reference to that parent's renderer.
        if (selectionRenderer == null && selection.GetComponentInParent<MeshRenderer>() != null)
        {
            selectionRenderer = selection.GetComponentInParent<MeshRenderer>();
        }
        //in the case that the object doesn't have a renderer but a child object does then set the reference to that child's renderer.
        else if (selectionRenderer == null && selection.GetComponentInChildren<MeshRenderer>() != null)
        {
            selectionRenderer = selection.GetComponentInChildren<MeshRenderer>();
        } 

        if (selectionRenderer != null)
        {
            //Saves the default material for deselection 
            defaultMaterial = selectionRenderer.material;
            //then create a list of materials to add the highlighted material 
            var highlightedMats = new Material[] { highlightedMaterial, selectionRenderer.material };
            //then sets the materials to the new list.
            selectionRenderer.materials = highlightedMats;
        }
    }

    public void OnDeselect(Transform selection)
    {
        //Creates a variable to hold the Renderer for the object being unhighlighted.
        var selectionRenderer = selection.GetComponent<MeshRenderer>();

        //in the case that the object doesn't have a renderer but a child object does then set the reference to that child's renderer.
        if (selectionRenderer == null && selection.GetComponentInChildren<MeshRenderer>() != null)
        {
            selectionRenderer = selection.GetComponentInChildren<MeshRenderer>();
        }
        //in the case that the object doesn't have a renderer but a parent object does then set the reference to that parent's renderer.
        else if (selectionRenderer == null && selection.GetComponentInParent<MeshRenderer>() != null)
        {
            selectionRenderer = selection.GetComponentInParent<MeshRenderer>();
        }

        if (selectionRenderer != null)
        {
            //Create a new list of materials that holds just the default material.
            var unhighlightedMats = new Material[] { defaultMaterial };
            //Apply the new list to the materials variable.
            selectionRenderer.materials = unhighlightedMats;
            //Debug.Log("Deselecting " + selection.name);
        }
    }
}
