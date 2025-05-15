using UnityEngine;

public class Minigames : MonoBehaviour
{
    // Minigames
    public GameObject writeTicketGame;
    public WireCut cutWireGame;

    void Start()
    {
        cutWireGame.GetComponent<WireCut>();
    }

    public void WriteTicketGameStart()
    {
        writeTicketGame.SetActive(true);
    }

    public void cutWireGameStart()
    {
        cutWireGame.StartMinigame();
    }
}
