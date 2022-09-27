using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntSfxHolder : SfxHolder
{
    public enum SoundType{
        DOOR
    }

    [SerializeField] private List<AudioClip> doorClips;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PlaySfx(Enum e, int idx)
    {
        switch(e)
        {
            case SoundType.DOOR:
                audioSource.PlayOneShot(doorClips[idx]);
                break;
        }
    }
}
