using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamPos : MonoBehaviour
{
    public bool isTriggered = false;

    private void OnTriggerEnter(Collider other) 
    {
        isTriggered = true;
    }

    private void OnTriggerExit(Collider other) 
    {
        isTriggered = false;
    }
}
