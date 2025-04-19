using UnityEngine;
using TMPro;

public class WeaponTypeText : MonoBehaviour
{
    public FPShooting fPShooting;
    public TMP_Text weaponTypeText;

    public void ChangeWeapon()
    {
        switch (fPShooting.currentWeapon)
        {
            case FPShooting.WeaponType.Gun:
                weaponTypeText.text = "Lethal";
                break;
            case FPShooting.WeaponType.Melee:
                weaponTypeText.text = "Non Lethal";
                break;
            case FPShooting.WeaponType.None:
                weaponTypeText.text = "Unarmed";
                break;
        }
    }
}
