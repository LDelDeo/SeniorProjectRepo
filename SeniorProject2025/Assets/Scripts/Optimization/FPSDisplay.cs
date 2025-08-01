using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TMP_Text fpsDisplayText;
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsDisplayText.text = "FPS " + Mathf.Ceil(fps).ToString();
    }
}
