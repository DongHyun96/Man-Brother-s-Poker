using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LazyAction : MonoBehaviour
{
    private static LazyAction wkr;
    
    public static LazyAction GetWkr()
    {
        if(wkr == null)
        {
            GameObject container = new GameObject("LazyAction");
            container.AddComponent<LazyAction>();

            wkr = container.GetComponent<LazyAction>();
        }

        return wkr;
    }

    private IEnumerator LazyRoutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        action.Invoke();
    }

    public void Act(Action action, float delay)
    {
        StartCoroutine(LazyRoutine(action, delay));
    }
}
