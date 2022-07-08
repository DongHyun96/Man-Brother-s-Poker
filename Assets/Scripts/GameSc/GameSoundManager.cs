using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : SoundManager
{
    public enum SfxType{
        CARD, CHIP, PLAYER
    }

    public override void PlaySfx(System.Enum e, int idx)
    {

    }

    protected override void Start()
    {
        base.Start();
    }
}
