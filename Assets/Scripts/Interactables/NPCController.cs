using TMPro;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private TMP_Text _dialogText;

    [SerializeField] private string[] _dialogLines;
    
    public void Interact(Transform player, PlayerInventory inventory)
    {
        _dialogText.gameObject.SetActive(true);
        _dialogText.text = _dialogLines[0];
    }

    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        _dialogText.gameObject.SetActive(false);
    }
}
