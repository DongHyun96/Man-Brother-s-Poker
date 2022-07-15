using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InputFieldEnterHandler : MonoBehaviour
{
    public UnityEvent onEnter; 

    private InputField inputField;
    private bool wasFocused;

    private void Awake() 
    {
        inputField = gameObject.GetComponent<InputField>();
    }

    private void Update() 
    {
        if(wasFocused && Input.GetKeyDown(KeyCode.Return))
        {
            onEnter.Invoke();
        }

        wasFocused = inputField.isFocused;
    }


}

