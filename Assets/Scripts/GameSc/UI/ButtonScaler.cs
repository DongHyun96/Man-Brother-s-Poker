using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float BUTTON_SCALE = 1.2f;

    // Override
    public void OnPointerEnter(PointerEventData data)
    {
        transform.localScale = Vector3.one * BUTTON_SCALE;
    }


    // Override
    public void OnPointerExit(PointerEventData data)
    {
        transform.localScale = Vector3.one;
    }
}
