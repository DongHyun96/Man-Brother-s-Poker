using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefab : MonoBehaviour
{
    public Text name;
    public Image knob;

    private Color green = new Color(24/255f, 255/255f, 8/255f);
    private Color red = new Color(245/255f, 78/255f ,81/255f);

    public void SetName_Dot(string n, bool isInvitable)
    {
        name.text = n;
        
        if(isInvitable)
        {
            knob.color = green;
        }
        else
        {
            knob.color = red;
        }

    }
}
