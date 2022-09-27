using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSoundHandler : MonoBehaviour
{
    public void PlayDoorSound(int idx)
    {
        SfxHolder.GetInstance().PlaySfx(EntSfxHolder.SoundType.DOOR, idx);
    }
}
