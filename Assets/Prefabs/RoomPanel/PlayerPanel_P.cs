using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel_P : MonoBehaviour
{
    public Text name;

    public void Init(string n)
    {
        name.text = n;
    }
}
