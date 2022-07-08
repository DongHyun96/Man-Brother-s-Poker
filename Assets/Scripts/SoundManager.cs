using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public abstract class SoundManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public AudioSource bgSound;
    public List<AudioClip> bgList;

    public AudioSource sfxSound;
    // public List<AudioClip> sfxList;

    private float prevBgmVol;
    private float prevSfxVol;

    public void SetBgmVolume(float bgmLvl)
    {
        masterMixer.SetFloat("bgmVolume", bgmLvl);
    }

    public void SetSfxVolume(float sfxLvl)
    {
        masterMixer.SetFloat("sfxVolume", sfxLvl);
    }

    public void ToggleBgm(bool isOn)
    {
        // float vol = isOn ? 
        throw new NotImplementedException();
    }

    public void ToggleSfx(bool isOn)
    {
        throw new NotImplementedException();
    }

    protected virtual void Start() {
        // Play random bgm
        System.Random rand = new System.Random();
        int randIdx = rand.Next(bgList.Count);
        PlayBgm(randIdx);
    }

    private void OnBgmFinished()
    {
        // Get random bgm and Keep playing
        System.Random rand = new System.Random();
        int randIdx = rand.Next(bgList.Count);
        PlayBgm(randIdx);
    }

    private void PlayBgm(int idx)
    {
        bgSound.clip = bgList[idx];
        bgSound.volume = 0.2f; // To do - erase this line
        bgSound.Play();

        // Invoke OnBgmFinished
        Invoke("OnBgmFinished", bgSound.clip.length);
    }


    public abstract void PlaySfx(Enum e, int idx);
}
