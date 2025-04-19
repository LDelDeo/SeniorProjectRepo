using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    [Header("Map")]
    public GameObject map;
    public bool isMapOpen;
    [Header("Canvas' / UI / HUD")]
    public GameObject playerHUD;
    public GameObject carHUD;
    
    [Header("Script Grabs")]
    public EnterCarScript enterCarScript;

    void Start()
    {
        map.SetActive(false);
    }
    void Update()
    {
        OpenMap();
    }

    public void OpenMap()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isMapOpen = !isMapOpen;
        }

        if (isMapOpen)
        {
            map.SetActive(true);
            carHUD.SetActive(false);
            playerHUD.SetActive(false);

        }
        else
        {
            map.SetActive(false);

            if(enterCarScript.isInCar)
            {
                carHUD.SetActive(true);
            }
            else
            {
                playerHUD.SetActive(true);
            }
        }
    }
}
