using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public TMP_Text dialogueTxt;
    public GameObject dialogueBox;

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
    }

    public void InitiateDialogue(string dialogue)
    {
        StartCoroutine(ShowDialogue(dialogue));
    }

    private IEnumerator ShowDialogue(string dialogue)
    {
        dialogueTxt.text = dialogue;
        dialogueBox.SetActive(true);

        yield return new WaitForSeconds(6f);

        dialogueBox.SetActive(false);
    }
}
