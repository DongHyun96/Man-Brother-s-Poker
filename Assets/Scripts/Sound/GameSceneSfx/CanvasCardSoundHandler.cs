using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCardSoundHandler : MonoBehaviour
{
    public void PlayCardSound(int idx)
    {
        SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.CANVAS_CARD, idx);
    }
}
