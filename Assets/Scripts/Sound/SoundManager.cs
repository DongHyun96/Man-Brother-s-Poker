using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/* Controls Volume & mute */
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioMixer masterMixer;

    private void Awake() {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static SoundManager GetInstance()
    {
        return instance;
    }

    public void SetBgmLvl(float bgmLvl)
    {
        masterMixer.SetFloat("bgmVolume", bgmLvl);
    }

    public void SetSfxLvl(float sfxLvl)
    {
        masterMixer.SetFloat("sfxVolume", sfxLvl);
    }
    
}
