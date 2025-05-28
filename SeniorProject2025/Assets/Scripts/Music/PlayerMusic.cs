using UnityEngine;

public class PlayerMusic : MonoBehaviour
{
    public MusicManager musicManager;

    public bool isInSuburbs = false;
    public bool isInRich = false;
    public bool isInHood = false;
    public bool isInIndustrial = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "suburbsTrigger" && isInSuburbs == false)
        {
            musicManager.PlayMusic(musicManager.suburbs, "Hannara");
            musicManager.PlayAmbiance(musicManager.suburbsSFX);
            isInSuburbs = true;
        }
        else if (other.gameObject.tag == "richTrigger" && isInRich == false)
        {
            musicManager.PlayMusic(musicManager.rich, "Morgatelis");
            musicManager.PlayAmbiance(musicManager.richSFX);
            isInRich = true;
        }
        else if (other.gameObject.tag == "hoodTrigger" && isInHood == false)
        {
            musicManager.PlayMusic(musicManager.hood, "DelDedria");
            musicManager.PlayAmbiance(musicManager.hoodSFX);
            isInHood = true;
        }
        else if (other.gameObject.tag == "industrialTrigger" && isInIndustrial == false)
        {
            musicManager.PlayMusic(musicManager.industrial, "Colezaria");
            musicManager.PlayAmbiance(musicManager.industrialSFX);
            isInIndustrial = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "suburbsTrigger" && isInSuburbs == true)
        {
            isInSuburbs = false;
        }
        else if (other.gameObject.tag == "richTrigger" && isInRich == true)
        {
            isInRich = false;
        }
        else if (other.gameObject.tag == "hoodTrigger" && isInHood == true)
        {
            isInHood = false;
        }
        else if (other.gameObject.tag == "industrialTrigger" && isInIndustrial == true)
        {
            isInIndustrial = false;
        }
    }
}
