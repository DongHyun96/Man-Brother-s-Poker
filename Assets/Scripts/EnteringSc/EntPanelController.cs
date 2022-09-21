using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


public class EntPanelController : MonoBehaviour
{
    public Animator anim_door;
    public Animator anim_signUp;
    public Animator anim_Lobby;
    public Animator anim_Room;
    public Animator anim_Inviting;
    public Animator anim_Menu;

    private static EntPanelController m_instance;
    public static EntPanelController Instance
    {
        get
        {
            if(m_instance == null) {
                m_instance = FindObjectOfType<EntPanelController>();
            }
            return m_instance;
        }
    }

    public enum Panel{
        SIGN, LOBBY, ROOM, INVITE, MENU
    }

    /**
    * Toggle the panel
    */
    public void UpdatePanel(Panel p)
    {
        switch(p)
        {
            case Panel.SIGN:
                anim_door.SetTrigger("Open");
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
