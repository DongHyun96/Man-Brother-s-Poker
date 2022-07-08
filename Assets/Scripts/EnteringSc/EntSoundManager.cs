using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntSoundManager : SoundManager
{
    // 예시
    public enum SfxType{
        CARD, TABLE, PLAYER
    }

    public override void PlaySfx(Enum e, int idx)
    {
        switch(e)
        {

        }
    }

    protected override void Start() {
        base.Start();
    }

    
}
