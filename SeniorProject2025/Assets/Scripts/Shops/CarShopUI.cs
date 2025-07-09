using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class CarShopUI : MonoBehaviour
{
    public enum CarRequirementType
    {
        None,
        PlayBlackjack,
        CompleteTier1Crimes,
        DefeatRangedOrcs,  
        DriveDistance
    }

    [Header("Car Settings")]
    public GameObject[] purchaseableCars;
    public GameObject carItemPrefab;
    public Transform carListContainer;
    public int[] carPrices;
    public CarRequirementType[] carRequirements;

    [Header("Default Car Settings")]
    public int defaultCarIndex = 0;

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

        // Ensure default car is unlocked
        if (defaultCarIndex >= 0 && defaultCarIndex < purchaseableCars.Length)
        {
            string defaultCarKey = "Car_Purchased_" + purchaseableCars[defaultCarIndex].name;
            PlayerPrefs.SetInt(defaultCarKey, 1);
        }

        string currentlySelectedCar = PlayerPrefs.GetString("SelectedCar", "");
        if (string.IsNullOrEmpty(currentlySelectedCar))
        {
            currentlySelectedCar = purchaseableCars[0].name;
            PlayerPrefs.SetString("SelectedCar", currentlySelectedCar);
        }

        GameObject defaultCar = purchaseableCars[defaultCarIndex];

        for (int i = 0; i < purchaseableCars.Length; i++)
        {
            GameObject car = purchaseableCars[i];
            int carPrice = carPrices.Length > i ? carPrices[i] : 100;
            CarRequirementType requirement = carRequirements.Length > i ? carRequirements[i] : CarRequirementType.None;

            GameObject item = Instantiate(carItemPrefab, carListContainer);

            TMP_Text carNameText = item.transform.Find("CarNameText").GetComponent<TMP_Text>();
            TMP_Text carPriceText = item.transform.Find("CarPriceText").GetComponent<TMP_Text>();
            Button purchaseButton = item.transform.Find("PurchaseButton").GetComponent<Button>();
            Button selectButton = item.transform.Find("SelectButton").GetComponent<Button>();

            string carKey = "Car_Purchased_" + car.name;
            bool isPurchased = PlayerPrefs.GetInt(carKey, 0) == 1 || car.name == purchaseableCars[defaultCarIndex].name;
            bool isSelected = (currentlySelectedCar == car.name);
            if (isSelected) defaultCar = car;

            carNameText.text = car.name;

            int handsPlayedBJ = PlayerPrefs.GetInt("HandsPlayedBJ", 0);
            int tier1Completed = PlayerPrefs.GetInt("Tier1CrimesCompleted", 0);
            int orcsDefeated = PlayerPrefs.GetInt("RangedOrcsDefeated", 0);
            float driveDistance = PlayerPrefs.GetFloat("TotalDriveDistance", 0f);

            if (isPurchased)
            {
                carPriceText.gameObject.SetActive(false);
                purchaseButton.gameObject.SetActive(false);
            }
            else
            {
                carPriceText.gameObject.SetActive(true);
                bool canUnlock = false;

                switch (requirement)
                {
                    case CarRequirementType.PlayBlackjack:
                        if (handsPlayedBJ < 100)
                        {
                            carPriceText.text = $"Play {100 - handsPlayedBJ} more blackjack hands to unlock";
                        }
                        else
                        {
                            carPriceText.text = $"Price: {carPrice}";
                            canUnlock = true;
                        }
                        break;

                    case CarRequirementType.CompleteTier1Crimes:
                        if (tier1Completed < 25)
                        {
                            carPriceText.text = $"Complete {25 - tier1Completed} more Tier 1 Crimes to unlock";
                        }
                        else
                        {
                            carPriceText.text = $"Price: {carPrice}";
                            canUnlock = true;
                        }
                        break;

                    case CarRequirementType.DefeatRangedOrcs:
                        if (orcsDefeated < 10)
                        {
                            carPriceText.text = $"Defeat {10 - orcsDefeated} more ranged orcs to unlock";
                        }
                        else
                        {
                            carPriceText.text = $"Price: {carPrice}";
                            canUnlock = true;
                        }
                        break;

                    case CarRequirementType.DriveDistance:
                        if (driveDistance < 5000f)
                        {
                            carPriceText.text = $"Drive {Mathf.CeilToInt(5000f - driveDistance)} more meters to unlock";
                        }
                        else
                        {
                            carPriceText.text = $"Price: {carPrice}";
                            canUnlock = true;
                        }
                        break;

                    default:
                        carPriceText.text = $"Price: {carPrice}";
                        canUnlock = true;
                        break;
                }

                bool canAfford = playerData.credits >= carPrice;
                purchaseButton.interactable = canUnlock && canAfford;
                UpdatePurchaseButtonColor(purchaseButton, canUnlock && canAfford);

                purchaseButton.gameObject.SetActive(true);
            }

            selectButton.gameObject.SetActive(isPurchased);
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

            GameObject currentCar = car;

            purchaseButton.onClick.AddListener(() =>
            {
                if (!purchaseButton.interactable) return;

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

            EventTrigger trigger = item.GetComponent<EventTrigger>() ?? item.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entryEnter.callback.AddListener((data) => { DisplayCar(currentCar); });

            EventTrigger.Entry entryExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entryExit.callback.AddListener((data) =>
            {
                string selectedName = PlayerPrefs.GetString("SelectedCar", "");
                GameObject fallback = purchaseableCars.FirstOrDefault(c => c.name == selectedName);
                if (fallback != null) DisplayCar(fallback);
            });

            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);
        }

        DisplayCar(defaultCar);
    }

    private void Update()
    {
        creditsAMT.text = "Credits: " + playerData.credits;

        int handsPlayedBJ = PlayerPrefs.GetInt("HandsPlayedBJ", 0);
        int tier1Completed = PlayerPrefs.GetInt("Tier1CrimesCompleted", 0);
        int orcsDefeated = PlayerPrefs.GetInt("RangedOrcsDefeated", 0);
        float driveDistance = PlayerPrefs.GetFloat("TotalDriveDistance", 0f);

        foreach (Transform item in carListContainer)
        {
            Button purchaseButton = item.Find("PurchaseButton")?.GetComponent<Button>();
            TMP_Text carPriceText = item.Find("CarPriceText")?.GetComponent<TMP_Text>();
            TMP_Text carNameText = item.Find("CarNameText")?.GetComponent<TMP_Text>();

            if (purchaseButton == null || carPriceText == null || carNameText == null || !purchaseButton.gameObject.activeSelf)
                continue;

            string carName = carNameText.text;
            int index = System.Array.FindIndex(purchaseableCars, c => c.name == carName);
            if (index < 0) continue;

            int price = carPrices.Length > index ? carPrices[index] : 100;
            CarRequirementType requirement = carRequirements.Length > index ? carRequirements[index] : CarRequirementType.None;

            bool canUnlock = false;

            switch (requirement)
            {
                case CarRequirementType.PlayBlackjack:
                    if (handsPlayedBJ < 100)
                        carPriceText.text = $"Play {100 - handsPlayedBJ} more blackjack hands to unlock";
                    else
                    {
                        carPriceText.text = $"Price: {price}";
                        canUnlock = true;
                    }
                    break;

                case CarRequirementType.CompleteTier1Crimes:
                    if (tier1Completed < 25)
                        carPriceText.text = $"Complete {25 - tier1Completed} more Tier 1 Crimes to unlock";
                    else
                    {
                        carPriceText.text = $"Price: {price}";
                        canUnlock = true;
                    }
                    break;

                case CarRequirementType.DefeatRangedOrcs:
                    if (orcsDefeated < 10)
                        carPriceText.text = $"Defeat {10 - orcsDefeated} more ranged orcs to unlock";
                    else
                    {
                        carPriceText.text = $"Price: {price}";
                        canUnlock = true;
                    }
                    break;

                case CarRequirementType.DriveDistance:
                    if (driveDistance < 5000f)
                        carPriceText.text = $"Drive {Mathf.CeilToInt(5000f - driveDistance)} more meters to unlock";
                    else
                    {
                        carPriceText.text = $"Price: {price}";
                        canUnlock = true;
                    }
                    break;

                default:
                    carPriceText.text = $"Price: {price}";
                    canUnlock = true;
                    break;
            }

            bool canAfford = playerData.credits >= price;
            purchaseButton.interactable = canUnlock && canAfford;
            UpdatePurchaseButtonColor(purchaseButton, canUnlock && canAfford);
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

            // Treat default car (index 0) as purchased always
            bool isDefaultCar = carName == purchaseableCars[0].name;
            bool isPurchased = PlayerPrefs.GetInt("Car_Purchased_" + carName, 0) == 1 || isDefaultCar;
            bool isSelected = (selectedCarName == carName);

            if (!isPurchased)
            {
                selectButton.gameObject.SetActive(false);
            }
            else
            {
                selectButton.gameObject.SetActive(true);

                if (isSelected)
                {
                    // Gray out and disable interaction like all other selected cars
                    selectButton.interactable = false;
                    buttonText.text = "Selected";

                    ColorBlock colors = selectButton.colors;
                    colors.normalColor = Color.gray;
                    colors.highlightedColor = Color.gray;
                    colors.pressedColor = Color.gray;
                    selectButton.colors = colors;
                }
                else
                {
                    selectButton.interactable = true;
                    buttonText.text = "Select";

                    ColorBlock colors = selectButton.colors;
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
        LoadingScreenManager.Instance.LoadSceneWithLoadingScreen("MainScene");
    }
}
