using TMPro;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private TMP_Text _dialogText;

    [SerializeField] private string[] _dialogLines;
    
    public void Interact(Transform player)
    {
        _dialogText.gameObject.SetActive(true);
        _dialogText.text = _dialogLines[0];
    }

    public void OnExit()
    {
        _dialogText.gameObject.SetActive(false);
    }
}
