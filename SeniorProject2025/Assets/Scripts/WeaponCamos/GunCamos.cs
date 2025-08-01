using UnityEngine;

public class GunCamos : MonoBehaviour
{
    [Header("Gun Camo Materials (Match by Name)")]
    public Material[] camoMaterials;

    [Header("Muzzle Flash Materials (Match by Name)")]
    public Material[] muzzleFlashMaterials;

    [Header("Muzzle Flash Default Material")]
    public Material defaultMuzzleFlashMaterial;

    [Header("Gun Mesh Renderers")]
    public MeshRenderer[] meshRenderers;

    [Header("Muzzle Flash Renderer")]
    public ParticleSystemRenderer muzzleFlashRenderer;

    private void Start()
    {
        string selectedCamoName = PlayerPrefs.GetString("SelectedCamo", "");

        Material selectedGunMaterial = FindMaterialByName(camoMaterials, selectedCamoName);
        bool applyGunCamo = selectedGunMaterial != null;

        Material selectedMuzzleFlashMaterial = FindMaterialByName(muzzleFlashMaterials, selectedCamoName);
        if (selectedMuzzleFlashMaterial == null)
        {
            selectedMuzzleFlashMaterial = defaultMuzzleFlashMaterial;
        }

        if (applyGunCamo)
        {
            foreach (var renderer in meshRenderers)
            {
                if (renderer != null)
                    renderer.material = selectedGunMaterial;
            }
        }

        if (muzzleFlashRenderer != null)
        {
            muzzleFlashRenderer.material = selectedMuzzleFlashMaterial;
        }
    }

    private Material FindMaterialByName(Material[] materials, string name)
    {
        foreach (var mat in materials)
        {
            if (mat != null && mat.name == name)
                return mat;
        }
        return null;
    }
}

