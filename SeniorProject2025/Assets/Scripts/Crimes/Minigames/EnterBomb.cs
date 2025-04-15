using UnityEngine;

public class EnterBomb : MonoBehaviour
{
    private Minigames minigames;
    void Start()
    {
        minigames = FindObjectOfType<Minigames>();
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            minigames.cutWireGameStart();
        }
    }
}
