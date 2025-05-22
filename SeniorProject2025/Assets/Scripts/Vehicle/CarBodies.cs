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
        string selectedCarName = PlayerPrefs.GetString("SelectedCar", "");

        foreach (var car in cars)
        {
            if (car != null)
                car.SetActive(false);
        }

        GameObject selectedCar = null;
        foreach (var car in cars)
        {
            if (car != null && car.name == selectedCarName)
            {
                selectedCar = car;
                break;
            }
        }

        if (selectedCar != null)
        {
            selectedCar.SetActive(true);
        }
        else if (defaultCar != null)
        {
            defaultCar.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No car selected and no default car assigned!");
        }

        foreach (var wheel in wheels)
        {
                if (selectedCarName == "Hanna-R7")
                {
                    wheel.SetActive(false);
                }
                
            }
        
    }
}
