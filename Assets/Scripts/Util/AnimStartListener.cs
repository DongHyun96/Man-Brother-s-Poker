using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimStartListener : MonoBehaviour
{
    public UnityEvent onStart;
    
    public void OnAnimStart()
    {
        onStart.Invoke();
    }
}
