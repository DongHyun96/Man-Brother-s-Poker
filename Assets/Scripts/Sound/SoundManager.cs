using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/* Controls Volume & mute */
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioMixer masterMixer;

    public const float MAX = -12f;
    public const float MIN = -30f;

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

    private void Start() {
        // Init volumes
        float bgmVol = PlayerPrefs.GetFloat("bgmVol");
        float sfxVol = PlayerPrefs.GetFloat("sfxVol");

        bgmVol = bgmVol == 0 ? MAX : bgmVol;
        sfxVol = sfxVol == 0 ? MAX : sfxVol;

        // Set game Volumes
        SetBgmLvl(bgmVol);
        SetSfxLvl(sfxVol);
    }

    public static SoundManager GetInstance()
    {
        return instance;
    }

    public void SetBgmLvl(float bgmLvl)
    {
        if(bgmLvl == MIN)
        {
            masterMixer.SetFloat("bgmVolume", -80f); // Mute
            return;
        }
        masterMixer.SetFloat("bgmVolume", bgmLvl);
    }

    public void SetSfxLvl(float sfxLvl)
    {
        if(sfxLvl == MIN)
        {
            masterMixer.SetFloat("sfxVolume", -80f); // Mute
            return;
        }
        masterMixer.SetFloat("sfxVolume", sfxLvl);
    }

    public float GetBgmLvl()
    {
        float returnValue;
        masterMixer.GetFloat("bgmVolume",out returnValue);
        return returnValue;
    }

    public float GetSfxLvl()
    {
        float returnValue;
        masterMixer.GetFloat("sfxVolume",out returnValue);
        return returnValue;
    }

    // Save volume data
    private void OnApplicationQuit() {

        float bgmVol, sfxVol;
        masterMixer.GetFloat("bgmVolume", out bgmVol);
        masterMixer.GetFloat("sfxVolume", out sfxVol);

        // Set player data
        PlayerPrefs.SetFloat("bgmVol", bgmVol);
        PlayerPrefs.SetFloat("sfxVol", sfxVol);
    }
    
}
