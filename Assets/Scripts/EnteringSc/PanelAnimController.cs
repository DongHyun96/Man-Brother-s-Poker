using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PanelAnimController : MonoBehaviour
{
    public Animator anim_signUp;
    public Animator anim_Lobby;
    public Animator anim_Room;
    public Animator anim_Inviting;
    public Animator anim_Menu;

    public enum Panel{
        SIGN, LOBBY, ROOM, INVITE, MENU
    }

    /**
    * Toggle the panel
    */
    public void UpdatePanel(int panel)
    {
        Panel p = (Panel)panel;
        switch(p)
        {
            case Panel.SIGN:
                toggleAnimation(anim_signUp);
                break;
            case Panel.LOBBY:
                toggleAnimation(anim_Lobby);
                break;
            case Panel.ROOM:
                toggleAnimation(anim_Room);
                break;
            case Panel.INVITE:
                toggleAnimation(anim_Inviting);
                break;
            case Panel.MENU:
                toggleAnimation(anim_Menu);
                break;
            default:
                break;
        }
    }
    
    private void toggleAnimation(Animator a)
    {
        if (a.GetCurrentAnimatorStateInfo(0).IsName("Out") || a.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            a.SetTrigger("Show");
        else
            a.SetTrigger("Hide");
    }

    private void Update() {
        // Toggling Menu panel 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggleAnimation(anim_Menu);
        }
    }
}
