using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Get anim turning point event on animation and notify it to GameSceneUpdater to move to next step */
public class AnimTurningPointHandler : MonoBehaviour
{
    private bool isTurningPointPassed = false;

    public void OnAnimTurningPoint()
    {
        print("OnAnimTurningPointEntered");
        isTurningPointPassed = true;
    }

    /* Always set this start method if using turningPointHandler on animation */
    public void OnAnimStart()
    {
        print("OnAnimStart Entered");

        isTurningPointPassed = false;
    }

    public bool IsTurningPointPassed
    {
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
