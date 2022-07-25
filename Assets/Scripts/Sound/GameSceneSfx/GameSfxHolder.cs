using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSfxHolder : SfxHolder
{
    public enum SoundType{
        CARD, CHIP, ACTION, WINNING, CANVAS_CARD, P_CANVAS
    }

    // (Table object sounds) SHUFFLE, DRAW, SHOWDOWN, FOLD, GATHER TO DECK 
    [SerializeField] private List<AudioClip> cardClips;

    // (Table object sounds) BETTING, COLLECT_TO_POT
    [SerializeField] private List<AudioClip> chipClips;

    // Turn indicating sound(self), TakeActionSound
    [SerializeField] private List<AudioClip> actionClips;

    // (Winning sounds) pot winning sound, game winning sound
    [SerializeField] private List<AudioClip> winningClips;

    // (ScreenCanvas card sounds) cardInSound
    [SerializeField] private List<AudioClip> canvasCardClips;

    // (PlayerCanvas sounds) tabPanel in, tabPanel out
    [SerializeField] private List<AudioClip> playerCanvasClips;


    protected override void Awake()
    {
        base.Awake();

    }

    public override void PlaySfx(System.Enum e, int idx)
    {
        switch(e)
        {
            case SoundType.CARD:
                audioSource.PlayOneShot(cardClips[idx]);
                break;
            case SoundType.CHIP:
                audioSource.PlayOneShot(chipClips[idx]);
                break;
            case SoundType.ACTION:
                audioSource.PlayOneShot(actionClips[idx]);
                break;
            case SoundType.WINNING:
                audioSource.PlayOneShot(winningClips[idx]);
                break;
            case SoundType.CANVAS_CARD:
                audioSource.PlayOneShot(canvasCardClips[idx]);
                break;
            case SoundType.P_CANVAS:
                audioSource.PlayOneShot(playerCanvasClips[idx]);
                break;
        }
    }
}
