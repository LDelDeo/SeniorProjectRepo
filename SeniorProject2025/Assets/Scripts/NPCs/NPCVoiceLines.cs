using UnityEngine;

public class NPCVoiceLines : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource voiceSource;
    public AudioClip[] voiceClips;

    //Random Intervals
    private float playVoice = 8f; //Every 8 Seconds Switches and Plays a New Voice
    private float timer = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= playVoice)
        {
            PlayVoice();
        }
    }

    private void PlayVoice()
    {
        int randomNumber = Random.Range(0, voiceClips.Length);
        voiceSource.clip = voiceClips[randomNumber];
        voiceSource.Play();
    }
}
