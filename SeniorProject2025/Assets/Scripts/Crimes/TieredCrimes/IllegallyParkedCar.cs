using UnityEngine;

public class IllegallyParkedCar : MonoBehaviour
{
    private Minigames minigames;
    private EnterCarScript enterCarScript;

    void Update()
    {
        enterCarScript = FindObjectOfType<EnterCarScript>();

        if (minigames == null && enterCarScript.isInCar == false)
        {
            minigames = GameObject.Find("Minigames").GetComponent<Minigames>();
        }
    }

    public void ParkedCarGone()
    {
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            minigames.WriteTicketGameStart();
        }
    }
}
