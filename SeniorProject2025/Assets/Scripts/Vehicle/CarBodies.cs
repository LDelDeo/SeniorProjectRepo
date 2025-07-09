using UnityEngine;

public class CarBodies : MonoBehaviour
{
    [Header("Cars Available in Gameplay")]
    public GameObject[] cars;

    [Header("Default Car (Used if none selected or found)")]
    public GameObject defaultCar;

    [Header("Wheels")]
    public GameObject[] wheels;

    private void Start()
    {
        // Load selected car name
        string selectedCarName = PlayerPrefs.GetString("SelectedCar", "");

        // Deactivate all cars
        foreach (var car in cars)
        {
            if (car != null)
                car.SetActive(false);
        }

        // Find selected car
        GameObject selectedCar = null;
        foreach (var car in cars)
        {
            if (car != null && car.name == selectedCarName)
            {
                selectedCar = car;
                break;
            }
        }

        // If no selected car or not found, use default
        if (selectedCar == null)
        {
            selectedCar = defaultCar;

            if (selectedCar != null)
            {
                PlayerPrefs.SetString("SelectedCar", selectedCar.name); // Ensure fallback is saved
                Debug.Log("Fallback to default car: " + selectedCar.name);
            }
            else
            {
                Debug.LogWarning("No car selected and no default car assigned!");
            }
        }

        // Activate the selected or default car
        if (selectedCar != null)
        {
            selectedCar.SetActive(true);
        }

        // Handle wheels visibility (example condition for Hanna-R7)
        foreach (var wheel in wheels)
        {
            if (wheel != null)
            {
                // Customize this logic for each car as needed
                wheel.SetActive(selectedCar.name != "Hanna-R7");
            }
        }
    }
}
