using UnityEngine;

public class Weather : MonoBehaviour
{
    [Header("Weather Conditions")]
    public ParticleSystem[] currentWeather;
    public AudioSource weatherAmbiance;
    public string weatherConditions = "";

    [Header("Grabs")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 10f, 0);
    public LightingManager lightingManager;

    [Header("Weather Cycle")]
    private float weatherChange = 150f; //2.5 Minutes
    private float currentTimer = 0f;

    // Weather States
    // [0] = Rain + Cloudy
    // [1] = Clear/Dusty
    // [2] = Cloudy

    void Start()
    {
        int switchWeather = Random.Range(0, currentWeather.Length);
            if (switchWeather == 0) { Rain(); }
            if (switchWeather == 1) { Clear(); }
            if (switchWeather == 2) { Cloudy(); }
    }

    void LateUpdate()
    {
        if (weatherConditions == "Raining" || weatherConditions == "Clear")
        {
            Vector3 followPosition = player.position + offset;

            foreach (ParticleSystem weatherSystem in currentWeather)
            {
                weatherSystem.transform.position = followPosition;
            } 
        }
    }

    public void Update()
    {
        //Fire Flies & Moths at Night
        FliesAndMoths();

        //Random Weather Changes
        WeatherCycle();
    }

    private void FliesAndMoths()
    {
        GameObject[] fireFlies = GameObject.FindGameObjectsWithTag("FireFly");
        GameObject[] flies = GameObject.FindGameObjectsWithTag("Flies");

        if (lightingManager.TimeOfDay > 0.75 || lightingManager.TimeOfDay < 0.25) //After Sunset, Before Sunrise
        {
            foreach (GameObject fireFly in fireFlies)
            {
                fireFly.GetComponent<ParticleSystem>().Play();
            }

            foreach (GameObject fly in flies)
            {
                fly.GetComponent<ParticleSystem>().Play();
            }
        }
        else
        {
            foreach (GameObject fireFly in fireFlies)
            {
                fireFly.GetComponent<ParticleSystem>().Stop();
            }

            foreach (GameObject fly in flies)
            {
                fly.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    private void WeatherCycle()
    {
        currentTimer += Time.deltaTime;

        if (currentTimer >= weatherChange)
        {
            currentTimer = 0f;
            int switchWeather = Random.Range(0, currentWeather.Length);
            if (switchWeather == 0) { Rain(); }
            if (switchWeather == 1) { Clear(); }
            if (switchWeather == 2) { Cloudy(); }
        }
    }

    private void Rain()
    {
        currentWeather[0].Play();
        currentWeather[1].Stop();
        currentWeather[2].Stop();

        weatherAmbiance.Play();
        weatherConditions = "Raining";
    }

    private void Clear()
    {
        currentWeather[0].Stop();
        currentWeather[1].Play();
        currentWeather[2].Stop();

        weatherAmbiance.Stop();
        weatherConditions = "Clear";
    }

    private void Cloudy()
    {
        currentWeather[0].Stop();
        currentWeather[1].Stop();
        currentWeather[2].Play();

        weatherAmbiance.Stop();
        weatherConditions = "Cloudy";
    }

}
