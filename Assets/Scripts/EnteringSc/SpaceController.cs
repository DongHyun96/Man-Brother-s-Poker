using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpaceController : MonoBehaviour
{

    public InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField.onValueChanged.AddListener(delegate { RemoveSpaces(); });
    }

    void RemoveSpaces()
    {
        inputField.text = inputField.text.Replace(" ", "");
    }
}
