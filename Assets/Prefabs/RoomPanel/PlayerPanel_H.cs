using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel_H : MonoBehaviour
{
    public Text name;

    public void Init(string name)
    {
        this.name.text = name;
    }
}
