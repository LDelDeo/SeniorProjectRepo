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
    public bool isInTrigger = false;

    void Start()
    {
        upgradeName.text = nameOfUpgrade;
        pressEToUpgrade.SetActive(false);
    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            SceneHelper.SaveAndLoadScene(gameScene);
            //LoadingScreenManager.Instance.LoadSceneWithLoadingScreen(gameScene);
        }
    }

  

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressEToUpgrade.SetActive(true);
            isInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressEToUpgrade.SetActive(false);
            isInTrigger = false;
        }
    }
}
