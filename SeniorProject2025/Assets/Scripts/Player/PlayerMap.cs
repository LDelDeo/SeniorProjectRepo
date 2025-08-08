using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    [Header("Map")]
    public GameObject map;
    public GameObject miniMap;
    public bool isMapOpen;
    [Header("Canvas' / UI / HUD")]
    public GameObject playerHUD;
    public GameObject carHUD;
    public GameObject mapToggleText;
    public GameObject playerMapMarker;
    
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
            miniMap.SetActive(false);
            mapToggleText.SetActive(true);

            playerMapMarker.GetComponent<Animator>().enabled = true;
            playerMapMarker.transform.localScale = new Vector3(24, 24, 24);
        }
        else
        {
            map.SetActive(false);
            miniMap.SetActive(true);
            mapToggleText.SetActive(false);

            playerMapMarker.GetComponent<Animator>().enabled = false;
            playerMapMarker.GetComponent<SpriteRenderer>().color = Color.white;
            playerMapMarker.transform.localScale = new Vector3(12.4f, 12.4f, 12.4f);

            if (enterCarScript.isInCar)
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
