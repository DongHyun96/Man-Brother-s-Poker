using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // Play click sound
    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsBtnInteractable())
        {
            SfxHolder.GetInstance().PlayButtonSound(1);
        }
    }

    // Play hover sound
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(IsBtnInteractable())
        {
            SfxHolder.GetInstance().PlayButtonSound(0);
        }
    }

    private bool IsBtnInteractable()
    {
        // Button case
        if(gameObject.GetComponent<Button>() != null)
        {
            if(gameObject.GetComponent<Button>().IsInteractable())
            {
                return true;
            }
        }
        else if(gameObject.GetComponent<Toggle>() != null) // Toggle button case
        {
            if(gameObject.GetComponent<Toggle>().IsInteractable())
            {
                return true;
            }   
        }
        return false;
    }
}
