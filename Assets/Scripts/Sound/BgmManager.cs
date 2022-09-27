using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BgmManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgSound;
    [SerializeField] private List<AudioClip> bgList;

    private void Start() {

        // Play random bgm
        OnBgmFinished();
    }

    private void OnBgmFinished()
    {
        System.Random rand = new System.Random();
        int randIdx = rand.Next(bgList.Count);
        PlayBgm(randIdx);
    }

    private void PlayBgm(int idx)
    {
        bgSound.clip = bgList[idx];
        bgSound.volume = 1f;  // To do : erase this line
        bgSound.Play();

        // Invoke OnBgmFinished
        Invoke("OnBgmFinished", bgSound.clip.length);
    }
}
