using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableToggler : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;

    public void SetInteractable(bool isInteractable)
    {
        foreach(Button b in buttons)
        {
            b.interactable = isInteractable;
        }
    }    
}
