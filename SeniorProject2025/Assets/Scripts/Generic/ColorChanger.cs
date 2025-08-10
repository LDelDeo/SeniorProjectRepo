using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material[] possibleColors;
    public MeshRenderer objectToPaint;

    void Start()
    {
        int rand = Random.Range(0, possibleColors.Length);

        Material[] parts = objectToPaint.materials;
        parts[1] = possibleColors[rand];
        objectToPaint.materials = parts;
    }
}
