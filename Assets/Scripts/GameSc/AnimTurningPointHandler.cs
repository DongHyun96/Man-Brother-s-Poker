using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Get anim turning point event on animation and notify it to GameSceneUpdater to move to next step */
public class AnimTurningPointHandler : MonoBehaviour
{
    private bool isTurningPointPassed = false;

    public void OnAnimTurningPoint()
    {
        isTurningPointPassed = true;
    }

    /* Always set this start method if using turningPointHandler on animation */
    public void OnAnimStart()
    {
        isTurningPointPassed = false;
    }

    public bool IsTurningPointPassed
    {
        set
        {
            isTurningPointPassed = false;
        }
        get
        {
            if(!isTurningPointPassed)
            {
                return false;
            }
            else
            {
                isTurningPointPassed = false;
                return true;
            }

        }
    }
}
