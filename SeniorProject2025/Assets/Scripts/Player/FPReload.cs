using UnityEngine;

public class FPReload : MonoBehaviour
{
    public FPShooting fpShooting;

  
    public void eventReload()
    {
        fpShooting.RefillAmmo();
        
    }
}
