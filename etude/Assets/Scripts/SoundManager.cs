using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
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

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject obj = new GameObject(sfxName + "Sound");
        AudioSource audioSource = obj.AddComponent<AudioSource>();
        audioSource.volume = .3f;
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(obj, clip.length);
    }
}
