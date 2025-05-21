using UnityEngine;
using System.Collections;


public class MedicalBill : MonoBehaviour
{
    public GameObject medicalBill;
    public PlayerData playerData;
    public Animator anim;
    public FPShooting fPShooting;
    public FPController fPController;
    public void PayFee()
    {
        playerData.SpendCredits(150);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        fPController.enabled = true;
        fPShooting.enabled = true;

        anim.SetBool("PlayAnim", true);
        StartCoroutine(ResetAnim()); 
    }

    public IEnumerator ResetAnim()
    {
        yield return new WaitForSeconds(0.05f);
        anim.SetBool("PlayAnim", false);
        medicalBill.SetActive(false);
    }
}
