using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class CardSoundHandler : MonoBehaviour
{
    public enum SoundType{
        SHUFFLE, DRAW, SHOWDOWN, FOLD
    }

    public void PlayCardSound(SoundType st)
    {
        // Check if there is available card object
        if(transform.childCount > 0)
        {
            SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.CARD, (int)st);
        }
    }

}
