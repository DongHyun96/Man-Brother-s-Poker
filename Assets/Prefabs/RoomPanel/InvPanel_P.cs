using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InvPanel_P : MonoBehaviour
{
    public Text name;

    public GameObject button;

    //public GameObject timer;

    public Image timer;
    public Image fill;

    public void InitContents(string name, bool invitable)
    {
        this.name.text = name;
        if(!invitable)
            button.SetActive(false);
    }

    public void onBtnPressed()
    {
        //fill.fillAmount
    }
    
}
