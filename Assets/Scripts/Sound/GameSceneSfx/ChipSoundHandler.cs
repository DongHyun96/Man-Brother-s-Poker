using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipSoundHandler : MonoBehaviour
{
    public enum SoundType{
        BETTING
    }

    public void PlayChipSound(SoundType t)
    {
        // Check if there is available chip object
        if(transform.childCount > 0)
        {
            SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.CHIP, (int)t);
        }
    }
}
