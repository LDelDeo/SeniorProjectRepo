using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [Header("Lighting Settings")]
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField, Range(0f, 1f)] private float TimeOfDay;
    [SerializeField] private float DayDuration = 300f; // 5 minutes for full day cycle

    private List<Light> streetLights = new List<Light>();

    /* 
        Cycle Times 
        0.00 - Midnight 
        0.25 - 6 AM (Sunrise) 
        0.50 - Noon 
        0.75 - 6 PM (Sunset) 
        1.00 - Midnight (Again/Resets) 
    */

    private void Start()
    {
        CacheStreetLights();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!Application.isPlaying)
            CacheStreetLights();
    }
#endif

    private void CacheStreetLights()
    {
        streetLights.Clear();

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("StreetLight");

        foreach (GameObject obj in taggedObjects)
        {
            Light light = obj.GetComponent<Light>();
            if (light != null && (light.type == LightType.Point || light.type == LightType.Spot))
            {
                streetLights.Add(light);
            }
        }
    }

    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime / DayDuration;
            TimeOfDay %= 1f;
        }

        UpdateLighting(TimeOfDay);
        UpdateStreetLights(TimeOfDay);
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation =
                Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }

    private void UpdateStreetLights(float timePercent)
    {
        // Daytime between 6 AM (0.25) and 6 PM (0.75)
        bool isDay = timePercent >= 0.25f && timePercent < 0.75f;

        foreach (Light light in streetLights)
        {
            if (light == null) continue;

            if (isDay)
            {
                light.intensity = 0f;
            }
            else
            {
                light.intensity = light.type == LightType.Point ? 2f : 100f;
            }
        }
    }

    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}
