using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WireCut : MonoBehaviour
{
    public GameObject wireMinigameUI;
    public Button redWireButton;
    public Button blueWireButton;
    public Button yellowWireButton;
    public TMP_Text feedbackText;

    public bool doneCorrectly;

    private string correctWire;

    public void StartMinigame()
    {
        wireMinigameUI.SetActive(true);
        AssignRandomCorrectWire();

        feedbackText.text = $"Cut the <b><color={correctWire.ToLower()}>{correctWire}</color></b> wire!";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        redWireButton.onClick.AddListener(() => CutWire("Red"));
        blueWireButton.onClick.AddListener(() => CutWire("Blue"));
        yellowWireButton.onClick.AddListener(() => CutWire("Yellow"));
    }

    private void AssignRandomCorrectWire()
    {
        string[] wires = { "Red", "Blue", "Yellow" };
        correctWire = wires[Random.Range(0, wires.Length)];
        Debug.Log("Correct Wire: " + correctWire);
    }

    private void CutWire(string chosenWire)
    {
        if (chosenWire == correctWire)
        {
            feedbackText.text = "Success! You cut the right wire!";
            Invoke("CloseMinigame", 0.5f);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            doneCorrectly = true;
            //End Game Here
        }
        else
        {
            feedbackText.text = "Wrong Wire";

            doneCorrectly = false;
            // Player Death/Explosion
        }
        
    }

    private void CloseMinigame()
    {
        wireMinigameUI.SetActive(false);
        redWireButton.onClick.RemoveAllListeners();
        blueWireButton.onClick.RemoveAllListeners();
        yellowWireButton.onClick.RemoveAllListeners();
    }
}
