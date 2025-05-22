using UnityEngine;

public class GunCamos : MonoBehaviour
{
    [Header("Camo Materials (Match by Name)")]
    public Material[] camoMaterials;

    [Header("Default Material (Used if none selected or found)")]
    public Material defaultMaterial;

    [Header("Gun Mesh Renderers to Apply Camo")]
    public MeshRenderer[] meshRenderers;

    private void Start()
    {
        string selectedCamoName = PlayerPrefs.GetString("SelectedCamo", "");

        Material selectedMaterial = null;

        // Find the material by name
        foreach (var mat in camoMaterials)
        {
            if (mat != null && mat.name == selectedCamoName)
            {
                selectedMaterial = mat;
                break;
            }
        }

        // Fallback to default if nothing matched
        if (selectedMaterial == null)
        {
            selectedMaterial = defaultMaterial;
            if (selectedMaterial == null)
            {
                Debug.LogWarning("No selected camo and no default material assigned!");
                return;
            }
        }

        // Apply material to all renderers
        foreach (var renderer in meshRenderers)
        {
            if (renderer != null)
            {
                renderer.material = selectedMaterial;
            }
        }
    }
}
