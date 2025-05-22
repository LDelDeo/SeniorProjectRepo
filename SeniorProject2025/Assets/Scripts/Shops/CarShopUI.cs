using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class CarShopUI : MonoBehaviour
{
    [Header("Car Settings")]
    public GameObject[] purchaseableCars;
    public GameObject carItemPrefab;
    public Transform carListContainer;
    public int[] carPrices;

    [Header("UI Elements")]
    public TMP_Text creditsAMT;

    [Header("Script Grabs")]
    public PlayerData playerData;

    [Header("Display Settings")]
    public Transform displayAnchor;

    private GameObject currentDisplayCar;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        string currentlySelectedCar = PlayerPrefs.GetString("SelectedCar", "");
        GameObject defaultCar = purchaseableCars[0];

        for (int i = 0; i < purchaseableCars.Length; i++)
        {
            GameObject car = purchaseableCars[i];
            int carPrice = carPrices.Length > i ? carPrices[i] : 100;

            GameObject item = Instantiate(carItemPrefab, carListContainer);

            TMP_Text carNameText = item.transform.Find("CarNameText").GetComponent<TMP_Text>();
            TMP_Text carPriceText = item.transform.Find("CarPriceText").GetComponent<TMP_Text>();
            Button purchaseButton = item.transform.Find("PurchaseButton").GetComponent<Button>();
            Button selectButton = item.transform.Find("SelectButton").GetComponent<Button>();

            string carKey = "Car_Purchased_" + car.name;
            bool isPurchased = PlayerPrefs.GetInt(carKey, 0) == 1;
            bool isSelected = (currentlySelectedCar == car.name);
            if (isSelected) defaultCar = car;

            carNameText.text = car.name;

            if (isPurchased)
            {
                carPriceText.gameObject.SetActive(false);
                purchaseButton.gameObject.SetActive(false);
            }
            else
            {
                carPriceText.gameObject.SetActive(true);
                carPriceText.text = "Price: " + carPrice;
                purchaseButton.gameObject.SetActive(true);
            }

            if (!isPurchased)
            {
                selectButton.gameObject.SetActive(false);
            }
            else
            {
                selectButton.gameObject.SetActive(true);
            }

            GameObject currentCar = car;

            purchaseButton.onClick.AddListener(() =>
            {
                if (playerData.credits >= carPrice)
                {
                    playerData.SpendCredits(carPrice);
                    PlayerPrefs.SetInt(carKey, 1);
                    purchaseButton.gameObject.SetActive(false);
                    carPriceText.gameObject.SetActive(false);
                    selectButton.gameObject.SetActive(true);
                    selectButton.interactable = true;
                    UpdatePurchaseButtonColor(purchaseButton, false);
                    Debug.Log("Purchased: " + currentCar.name);
                }
                else
                {
                    Debug.Log("Not enough credits to purchase: " + currentCar.name);
                }
            });

            selectButton.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("SelectedCar", currentCar.name);
                Debug.Log("Selected: " + currentCar.name);

                RefreshSelectButtons();
                DisplayCar(currentCar);
            });

            purchaseButton.interactable = !isPurchased;
            selectButton.interactable = isPurchased && !isSelected;
            selectButton.GetComponentInChildren<TMP_Text>().text = isSelected ? "Selected" : "Select";

            if (isSelected)
            {
                var colors = selectButton.colors;
                colors.normalColor = Color.gray;
                colors.highlightedColor = Color.gray;
                colors.pressedColor = Color.gray;
                selectButton.colors = colors;
            }

            EventTrigger trigger = item.GetComponent<EventTrigger>();
            if (trigger == null) trigger = item.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { DisplayCar(currentCar); });

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) =>
            {
                string selectedName = PlayerPrefs.GetString("SelectedCar", "");
                GameObject fallback = purchaseableCars.FirstOrDefault(c => c.name == selectedName);
                if (fallback != null) DisplayCar(fallback);
            });

            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);
        }

        // Show selected/default car on start
        DisplayCar(defaultCar);
    }

    private void Update()
    {
        creditsAMT.text = "Credits: " + playerData.credits;

        // Also update purchase button colors live based on credits (optional)
        foreach (Transform item in carListContainer)
        {
            Button purchaseButton = item.Find("PurchaseButton")?.GetComponent<Button>();
            TMP_Text carPriceText = item.Find("CarPriceText")?.GetComponent<TMP_Text>();
            TMP_Text carNameText = item.Find("CarNameText")?.GetComponent<TMP_Text>();

            if (purchaseButton != null && carPriceText != null && carNameText != null && purchaseButton.gameObject.activeSelf)
            {
                string carName = carNameText.text;
                int price = 100; // default price
                int index = System.Array.FindIndex(purchaseableCars, c => c.name == carName);
                if (index >= 0 && carPrices.Length > index) price = carPrices[index];

                UpdatePurchaseButtonColor(purchaseButton, playerData.credits >= price);
            }
        }
    }

    private void DisplayCar(GameObject carPrefab)
    {
        if (currentDisplayCar != null)
            Destroy(currentDisplayCar);

        currentDisplayCar = Instantiate(carPrefab, displayAnchor.position, displayAnchor.rotation, displayAnchor);
        currentDisplayCar.transform.localPosition = Vector3.zero;
        currentDisplayCar.transform.localRotation = Quaternion.identity;
    }

    private void RefreshSelectButtons()
    {
        string selectedCarName = PlayerPrefs.GetString("SelectedCar", "");
        foreach (Transform item in carListContainer)
        {
            Button selectButton = item.Find("SelectButton").GetComponent<Button>();
            TMP_Text buttonText = selectButton.GetComponentInChildren<TMP_Text>();
            TMP_Text carNameText = item.Find("CarNameText").GetComponent<TMP_Text>();
            string carName = carNameText.text;
            bool isSelected = (selectedCarName == carName);
            bool isPurchased = PlayerPrefs.GetInt("Car_Purchased_" + carName, 0) == 1;

            if (!isPurchased)
            {
                selectButton.gameObject.SetActive(false);
            }
            else
            {
                selectButton.gameObject.SetActive(true);
                if (isSelected)
                {
                    selectButton.interactable = false;
                    buttonText.text = "Selected";

                    var colors = selectButton.colors;
                    colors.normalColor = Color.gray;
                    colors.highlightedColor = Color.gray;
                    colors.pressedColor = Color.gray;
                    selectButton.colors = colors;
                }
                else
                {
                    selectButton.interactable = true;
                    buttonText.text = "Select";

                    var colors = selectButton.colors;
                    colors.normalColor = Color.white;
                    colors.highlightedColor = new Color(0.78f, 0.78f, 0.78f);
                    colors.pressedColor = new Color(0.58f, 0.58f, 0.58f);
                    selectButton.colors = colors;
                }
            }
        }
    }

    private void UpdatePurchaseButtonColor(Button purchaseButton, bool canAfford)
    {
        ColorBlock colors = purchaseButton.colors;
        colors.normalColor = canAfford ? Color.green : Color.red;
        colors.highlightedColor = canAfford ? new Color(0.6f, 1f, 0.6f) : new Color(1f, 0.6f, 0.6f);
        purchaseButton.colors = colors;
    }

    public void ExitGarage()
    {
        SceneManager.LoadScene("MainScene");
    }
}
