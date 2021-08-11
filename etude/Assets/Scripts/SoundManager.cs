using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private float sfxVolume;
    private bool isMute;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sfxVolume = 0.3f;
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject obj = new GameObject(sfxName + "Sound");
        AudioSource audioSource = obj.AddComponent<AudioSource>();
        if (isMute)
            audioSource.volume = 0;
        else
            audioSource.volume = sfxVolume;
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(obj, clip.length);
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
    }

    public void Mute(bool isMute)
    {
        this.isMute = isMute;
    }
}
