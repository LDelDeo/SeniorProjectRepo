using UnityEngine;

public class ColorLoop : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float colorChangeSpeed = 1f;

    private bool goingToBlue = true;
    private float t = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        // Update t based on direction
        t += (goingToBlue ? 1 : -1) * colorChangeSpeed * Time.deltaTime;
        t = Mathf.Clamp01(t);

        // Lerp between red and blue
        spriteRenderer.color = Color.Lerp(Color.red, Color.blue, t);

        // Reverse direction when reaching ends
        if (t >= 1f) goingToBlue = false;
        else if (t <= 0f) goingToBlue = true;
    }
}
