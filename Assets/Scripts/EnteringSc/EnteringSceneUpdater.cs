using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnteringSceneUpdater : MonoBehaviour
{
    public static EnteringSceneUpdater instance;

    public UnityEvent onLobbyPlayersUpdate;

    public UnityEvent onLobbyRoomsUpdate;

    public delegate void LobbyChatUpdate(string name, string chat);
    public event LobbyChatUpdate onLobbyChatUpdate;


    public static EnteringSceneUpdater GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<EnteringSceneUpdater>();
        }
        return instance; // It may be null if it calls in invalid scene.
    }

    public void InitLobby()
    {
        
        // Init allOthers and rooms by getting data from server & Invoke updaters
        MainMsgHandler.SendMessage(new MainMessage(MainMessage.MessageType.GET));
        RoomMsgHandler.SendMessage(new RoomMessage(RoomMessage.MessageType.GET));

        //onLobbyPlayersUpdate.Invoke(); --> Invoke these in each MsgHandler
        //onLobbyRoomsUpdate.Invoke();
        

    }

    public void UpdateChat(string name, string chat)
    {
        onLobbyChatUpdate(name, chat);
    }

}
