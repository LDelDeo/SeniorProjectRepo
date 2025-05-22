using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class CamoShopUI : MonoBehaviour
{
    [Header("Camo Settings")]
    public Material[] purchaseableCamoMaterials; // Use materials instead of GameObjects
    public GameObject camoItemPrefab;
    public Transform camoListContainer;
    public int[] camoPrices;

    [Header("UI Elements")]
    public TMP_Text creditsAMT;

    [Header("Script Grabs")]
    public PlayerData playerData;

    [Header("Showcase Gun")]
    public List<MeshRenderer> showcaseGunRenderers;

    private Material currentSelectedCamo;

    // Helper class to track purchase buttons and their data
    private class CamoItemData
    {
        public Button purchaseButton;
        public int price;
        public string camoKey;
        public bool isPurchased;
    }

    private List<CamoItemData> camoButtons = new List<CamoItemData>();

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        string firstCamoKey = "Camo_Purchased_" + purchaseableCamoMaterials[0].name;
        if (!PlayerPrefs.HasKey(firstCamoKey))
        {
            PlayerPrefs.SetInt(firstCamoKey, 1); // Unlock first camo
        }

        if (!PlayerPrefs.HasKey("SelectedCamo"))
        {
            PlayerPrefs.SetString("SelectedCamo", purchaseableCamoMaterials[0].name); // Select first camo
        }

        string selectedCamoName = PlayerPrefs.GetString("SelectedCamo", "");
        Material defaultCamo = purchaseableCamoMaterials[0];

        for (int i = 0; i < purchaseableCamoMaterials.Length; i++)
        {
            Material camoMat = purchaseableCamoMaterials[i];
            int camoPrice = camoPrices.Length > i ? camoPrices[i] : 100;

            GameObject item = Instantiate(camoItemPrefab, camoListContainer);

            TMP_Text nameText = item.transform.Find("CamoNameText").GetComponent<TMP_Text>();
            TMP_Text priceText = item.transform.Find("CamoPriceText").GetComponent<TMP_Text>();
            Button purchaseButton = item.transform.Find("PurchaseButton").GetComponent<Button>();
            Button selectButton = item.transform.Find("SelectButton").GetComponent<Button>();

            string camoKey = "Camo_Purchased_" + camoMat.name;
            bool isPurchased = PlayerPrefs.GetInt(camoKey, 0) == 1;
            bool isSelected = (selectedCamoName == camoMat.name);
            if (isSelected) defaultCamo = camoMat;

            nameText.text = camoMat.name;

            if (isPurchased)
            {
                priceText.gameObject.SetActive(false);
                purchaseButton.gameObject.SetActive(false);
            }
            else
            {
                priceText.gameObject.SetActive(true);
                priceText.text = "Price: " + camoPrice;
                purchaseButton.gameObject.SetActive(true);
            }

            selectButton.gameObject.SetActive(isPurchased);

            Material currentCamo = camoMat;

            purchaseButton.onClick.AddListener(() =>
            {
                if (playerData.credits >= camoPrice)
                {
                    playerData.SpendCredits(camoPrice);
                    PlayerPrefs.SetInt(camoKey, 1);
                    purchaseButton.gameObject.SetActive(false);
                    priceText.gameObject.SetActive(false);
                    selectButton.gameObject.SetActive(true);
                    selectButton.interactable = true;
                    //Debug.Log("Purchased: " + currentCamo.name);
                }
                else
                {
                    //Debug.Log("Not enough credits to purchase: " + currentCamo.name);
                }
            });

            selectButton.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("SelectedCamo", currentCamo.name);
                Debug.Log("Selected: " + currentCamo.name);
                currentSelectedCamo = currentCamo;
                RefreshSelectButtons();
                ApplyCamoToGun(currentCamo);
            });

            selectButton.interactable = isPurchased && !isSelected;
            selectButton.GetComponentInChildren<TMP_Text>().text = isSelected ? "Selected" : "Select";

            if (isSelected)
            {
                selectButton.interactable = false;
                SetButtonGray(selectButton);
                currentSelectedCamo = currentCamo;
            }

            EventTrigger trigger = item.GetComponent<EventTrigger>() ?? item.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { ApplyCamoToGun(currentCamo); });

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) =>
            {
                if (currentSelectedCamo != null)
                    ApplyCamoToGun(currentSelectedCamo);
            });

            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);

            // Track button and price
            camoButtons.Add(new CamoItemData
            {
                purchaseButton = purchaseButton,
                price = camoPrice,
                camoKey = camoKey,
                isPurchased = isPurchased
            });
        }

        ApplyCamoToGun(defaultCamo);
    }

    private void Update()
    {
        creditsAMT.text = "Credits: " + playerData.credits;

        foreach (var camoItem in camoButtons)
        {
            if (!camoItem.isPurchased && camoItem.purchaseButton != null)
            {
                bool canAfford = playerData.credits >= camoItem.price;
                camoItem.purchaseButton.interactable = canAfford;

                ColorBlock colors = camoItem.purchaseButton.colors;
                if (canAfford)
                {
                    colors.normalColor = Color.green;
                    colors.highlightedColor = new Color(0.5f, 1f, 0.5f);
                    colors.pressedColor = new Color(0.3f, 0.7f, 0.3f);
                }
                else
                {
                    colors.normalColor = Color.red;
                    colors.highlightedColor = new Color(1f, 0.5f, 0.5f);
                    colors.pressedColor = new Color(0.7f, 0.3f, 0.3f);
                }
                camoItem.purchaseButton.colors = colors;
            }
        }
    }

    private void ApplyCamoToGun(Material camoMaterial)
    {
        if (showcaseGunRenderers != null)
        {
            foreach (var renderer in showcaseGunRenderers)
            {
                if (renderer != null)
                    renderer.material = camoMaterial;
            }
        }
    }

    private void RefreshSelectButtons()
    {
        string selectedCamo = PlayerPrefs.GetString("SelectedCamo", "");

        foreach (Transform item in camoListContainer)
        {
            TMP_Text nameText = item.Find("CamoNameText").GetComponent<TMP_Text>();
            Button selectButton = item.Find("SelectButton").GetComponent<Button>();
            TMP_Text buttonText = selectButton.GetComponentInChildren<TMP_Text>();

            string camoName = nameText.text;
            bool isPurchased = PlayerPrefs.GetInt("Camo_Purchased_" + camoName, 0) == 1;
            bool isSelected = selectedCamo == camoName;

            selectButton.gameObject.SetActive(isPurchased);
            selectButton.interactable = isPurchased && !isSelected;
            buttonText.text = isSelected ? "Selected" : "Select";

            if (isSelected) SetButtonGray(selectButton);
            else ResetButtonColor(selectButton);
        }
    }

    private void SetButtonGray(Button btn)
    {
        var colors = btn.colors;
        colors.normalColor = Color.gray;
        colors.highlightedColor = Color.gray;
        colors.pressedColor = Color.gray;
        btn.colors = colors;
    }

    private void ResetButtonColor(Button btn)
    {
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.78f, 0.78f, 0.78f);
        colors.pressedColor = new Color(0.58f, 0.58f, 0.58f);
        btn.colors = colors;
    }

    public void ExitCamoShop()
    {
        SceneManager.LoadScene("MainScene");
    }
}
