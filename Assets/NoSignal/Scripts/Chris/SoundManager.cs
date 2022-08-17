using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioClip[] BgClips;

    private AudioSource audioSource;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
    }

    private AudioClip GetClip()
    {
        return BgClips[Random.Range(0, BgClips.Length)];    
    }   


    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = GetClip();
            audioSource.Play();
        }

    }

    public void MuteSound()
    {
        audioSource.volume = 0;
    }

    public void PlaySound()
    {
        audioSource.volume = 1;
    }
}
