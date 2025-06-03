using UnityEngine;

public class ObjectivesList : MonoBehaviour
{
    // Tier 1
    private GameObject illegalPark;
    public GameObject objectiveIP;

    private GameObject graffiti;
    public GameObject objectiveG;

    // Tier 2
    private GameObject assault;
    public GameObject objectiveA;
    private GameObject drugDeal;
    public GameObject objectiveDD;
    private GameObject vandalism;
    public GameObject objectiveV;

    // Tier 3
    private GameObject armedRobbery;
    public GameObject objectiveAR;
    private GameObject bombPlant;
    public GameObject objectiveBP;

    void Update()
    {
        AttachObjective(illegalPark, objectiveIP, "crimeOne");
        AttachObjective(graffiti, objectiveG, "crimeTwo");
        AttachObjective(assault, objectiveA, "crimeThree");
        AttachObjective(drugDeal, objectiveDD, "crimeFour");
        AttachObjective(vandalism, objectiveV, "crimeSeven");
        AttachObjective(armedRobbery, objectiveAR, "crimeFive");
        AttachObjective(bombPlant, objectiveBP, "crimeSix");
    }

    public void AttachObjective(GameObject crime, GameObject objectiveText, string crimeTag)
    {
        crime = GameObject.FindWithTag(crimeTag);
        if (crime == null)
        {
            objectiveText.SetActive(false);
        }
        else if (crime != null)
        {
            objectiveText.SetActive(true);
        }
    }

}