using UnityEngine;

public class GoblinGraffitiSprayPaint : MonoBehaviour
{
    public ParticleSystem sprayPaint;

    private void StartPainting()
    {
        sprayPaint.Play();
    }

    public void StopPainting()
    {
        sprayPaint.Stop();
    }
}
