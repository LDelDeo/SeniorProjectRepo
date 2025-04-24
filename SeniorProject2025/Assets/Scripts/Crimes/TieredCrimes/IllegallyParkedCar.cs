using UnityEngine;

public class IllegallyParkedCar : MonoBehaviour
{
    private Minigames minigames;

    void Update()
    {
        minigames = GameObject.Find("Minigames").GetComponent<Minigames>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            minigames.WriteTicketGameStart();
        }
    }
}
