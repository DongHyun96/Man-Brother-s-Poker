using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MenuFunctions
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            toggleAnimation();
        }
    }

    public override void OnExitToLobby()
    {
        // Disconnect Game socket
        GameMsgHandler.DisconnectSocket();

        // Change the scene by using GameManager
        GameManager.GetInstance().state = GameManager.State.LOBBY;
    }

    private void toggleAnimation()
    {
        Animator a = GetComponent<Animator>();

        if (a.GetCurrentAnimatorStateInfo(0).IsName("Out") || a.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            a.SetTrigger("Show");
        else
            a.SetTrigger("Hide");
    }

    public override void OnCredits()
    {
        throw new System.NotImplementedException();
    }
}
