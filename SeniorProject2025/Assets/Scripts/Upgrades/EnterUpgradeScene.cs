using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterUpgradeScene : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text upgradeName;
    public GameObject pressEToUpgrade;

    [Header("Values")]
    public string nameOfUpgrade;
    public string gameScene;

    [Header("Booleans")]
    public bool isInGameTrigger = false;

    void Start()
    {
        upgradeName.text = nameOfUpgrade;
        pressEToUpgrade.SetActive(false);
    }

    void Update()
    {
        if (isInGameTrigger && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(gameScene);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("Player"))
        {
            pressEToUpgrade.SetActive(true);
            isInGameTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == ("Player"))
        {
            pressEToUpgrade.SetActive(false);
            isInGameTrigger = false;
        }
    }
}