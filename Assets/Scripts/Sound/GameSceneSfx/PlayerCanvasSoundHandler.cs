using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvasSoundHandler : MonoBehaviour
{
    // 0 - in, 1 - out
    public void PlayTabPanelSound(int idx)
    {
        SfxHolder.GetInstance().PlaySfx(GameSfxHolder.SoundType.P_CANVAS, idx);
    }
}
