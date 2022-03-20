using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuPanel : MonoBehaviour
{
    public void OnX()
    {
        PanelAnimController.GetInstance().UpdatePanel(PanelAnimController.Panel.MENU);
    }

    public void OnOptions()
    {

    }

    public void OnCredits()
    {

    }

    public void OnExitGame()
    {
        Application.Quit();
    }
    
}
