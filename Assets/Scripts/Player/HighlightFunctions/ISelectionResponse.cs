//////////////////////////////////////////////
//Assignment/Lab/Project: Portal
//Name: Nathaniel Lester
//Section: SGD.235.4173
//Instructor: Ven Lewis
//Date: 3/12/25
/////////////////////////////////////////////
using UnityEngine;

internal interface ISelectionResponse
{
    void OnDeselect(Transform selection);
    void OnSelect(Transform selection);
}
