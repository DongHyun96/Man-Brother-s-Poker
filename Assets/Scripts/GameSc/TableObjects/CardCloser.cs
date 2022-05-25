using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCloser : MonoBehaviour
{
    /* Reset the card transform to origin position for the next proper card flip */
    public void OnGatherToDeck()
    {
        transform.rotation = Quaternion.identity;

        /* Set flip layer to idle */
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetTrigger("rotationIdle");
    }
}
