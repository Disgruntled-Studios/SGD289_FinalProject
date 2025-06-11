using TMPro;
using UnityEngine;

public class PuzzleUI_Manager : MonoBehaviour
{
    public static PuzzleUI_Manager Instance { get; private set; }

    public TMP_Text tileMoveInstructions;
    public TMP_Text tileRotateInstructions;
    public TMP_Text puzzleInstructions;

    public GameObject puzzlePanelRef;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void TogglePuzzlePanel()
    {
        if (puzzlePanelRef.activeInHierarchy)
        {
            puzzlePanelRef.SetActive(false);
        }
        else
        {
            puzzlePanelRef.SetActive(true);
        }
    }

    public void ChangeControlsTxt(string newTextForMove, string newTextForRotate)
    {
        tileMoveInstructions.text = newTextForMove;
        tileRotateInstructions.text = newTextForRotate;
    }

    public void ChangePuzzleInstructionTxt(string newTxt)
    {
        puzzleInstructions.text = newTxt;
    }
}
