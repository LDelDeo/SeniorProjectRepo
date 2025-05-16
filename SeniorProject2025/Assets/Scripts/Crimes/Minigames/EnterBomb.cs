using UnityEngine;

public class EnterBomb : MonoBehaviour
{
    private Minigames minigames;
    private EnterCarScript enterCarScript;

    void Update()
    {
        enterCarScript = FindFirstObjectByType<EnterCarScript>();

        if (minigames == null && enterCarScript.isInCar == false)
        {
            minigames = GameObject.Find("Minigames").GetComponent<Minigames>();
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            minigames.cutWireGameStart();
        }
    }
}
