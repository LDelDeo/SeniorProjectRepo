using UnityEngine;

public class GoblinTrigger : MonoBehaviour
{
    public GoblinGraffitiEnemy goblinGraffitiEnemy;
    private void Start()
    {
        this.GetComponent<BoxCollider>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
            if (goblinGraffitiEnemy.hasBeenCaught) return; // Prevent showing prompt after already caught

            if (other.CompareTag("Player"))
            {
                Debug.Log("Player is in it");
                goblinGraffitiEnemy.canBeCuffed = true;

                if (goblinGraffitiEnemy.pressE != null)
                    goblinGraffitiEnemy.pressE.text = "Press [E] to Handcuff";
            }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player out");
            if (goblinGraffitiEnemy.pressE != null)
                goblinGraffitiEnemy. pressE.text = "";

            goblinGraffitiEnemy.canBeCuffed = false;
        }
    }
}
