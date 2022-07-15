using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds all sfx in the scene
public abstract class SfxHolder : MonoBehaviour
{
    private static SfxHolder instance;

    protected AudioSource audioSource;

    /* Sound clip type */

    /* All the sound clips */
    [SerializeField] private List<AudioClip> buttonClips;

    protected virtual void Awake() {
        if(instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static SfxHolder GetInstance()
    {
        return instance;
    }

    public void PlayButtonSound(int idx)
    {
        audioSource.PlayOneShot(buttonClips[idx]);
    }

    public abstract void PlaySfx(Enum e, int idx);

}


