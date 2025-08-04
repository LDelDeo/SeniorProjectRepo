using UnityEngine;

public class Minigames : MonoBehaviour
{
    // Minigames
    public GameObject writeTicketGame;
    public WireCut cutWireGame;

    //Player's Rigidbody
    public Rigidbody playerRigidbody;

    void Start()
    {
        cutWireGame.GetComponent<WireCut>();
    }

    public void WriteTicketGameStart()
    {
        writeTicketGame.SetActive(true);
        playerRigidbody.linearVelocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }

    public void cutWireGameStart()
    {
        cutWireGame.StartMinigame();
        playerRigidbody.linearVelocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }
}
