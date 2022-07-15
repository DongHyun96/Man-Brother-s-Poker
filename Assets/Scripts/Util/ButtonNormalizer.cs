using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* Set unity button to normal state after click */
public class ButtonNormalizer : MonoBehaviour
{
    public static void NormanlizeButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
