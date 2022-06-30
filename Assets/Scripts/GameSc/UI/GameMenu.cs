using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public Animator menuAnim;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            toggleAnimation(menuAnim);
        }
    }

    public void OnExitToLobby()
    {
        // Disconnect Game socket
        GameMsgHandler.DisconnectSocket();

        // Change the scene by using GameManager
        GameManager.GetInstance().state = GameManager.State.LOBBY;
    }

    public void OnExitGame()
    {
        Application.Quit();
    }

    public void OnClose()
    {
        menuAnim.SetTrigger("Hide");
    }

    private void toggleAnimation(Animator a)
    {
        if (a.GetCurrentAnimatorStateInfo(0).IsName("Out") || a.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            a.SetTrigger("Show");
        else
            a.SetTrigger("Hide");
    }
}
