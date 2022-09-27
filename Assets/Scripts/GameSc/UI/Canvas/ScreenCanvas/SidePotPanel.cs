using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePotPanel : MonoBehaviour
{
    public Text name;
    
    public void ClearSidePot()
    {
        name.text = "";
    }
}
