using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource src;
    public AudioClip SfxPop;

    public void PlayPop()
    {
        src.clip = SfxPop;
        src.Play();
    }
}
