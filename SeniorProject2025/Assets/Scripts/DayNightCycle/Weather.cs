using UnityEngine;

public class Weather : MonoBehaviour
{
    [Header("Weather Conditions")]
    public ParticleSystem[] currentWeather;

    [Header("Grabs")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 10f, 0);
    public LightingManager lightingManager;


    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 followPosition = player.position + offset;

            foreach (ParticleSystem weatherSystem in currentWeather)
            {
                if (weatherSystem != null)
                {
                    weatherSystem.transform.position = followPosition;
                }
            }
        }
    }

    public void Update()
    {
        if (lightingManager.TimeOfDay > 0.75 || lightingManager.TimeOfDay < 0.25) //After Sunset, Before Sunrise
        {
            currentWeather[0].Play();
            currentWeather[1].Stop();
        }
        else
        {
            currentWeather[0].Stop();
            currentWeather[1].Play();
        }
    }

}
