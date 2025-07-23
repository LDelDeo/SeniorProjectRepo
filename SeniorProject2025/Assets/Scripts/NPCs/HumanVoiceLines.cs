using UnityEngine;

public class HumanVoiceLines : MonoBehaviour
{
    public AudioSource humanVoice;
    public AudioClip[] voiceClips;

    private void Start()
    {
        int randomNumber = Random.Range(0, voiceClips.Length);
        humanVoice.clip = voiceClips[randomNumber];
        humanVoice.Play();
    }
}
