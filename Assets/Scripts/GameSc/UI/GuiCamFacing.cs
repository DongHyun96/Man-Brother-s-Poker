using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCamFacing : MonoBehaviour
{
    public Transform mLookAt;

    private void Update() {

        if(mLookAt)
        {
            transform.LookAt(2 * transform.position - mLookAt.position);
        }
    }
}
