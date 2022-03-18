using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class EnteringSceneUpdater : MonoBehaviour
{
    private static EnteringSceneUpdater instance;

    // Lobby Updaters
    public UnityEvent onLobbyPlayersUpdate;
    public UnityEvent onLobbyRoomsUpdate;
    public delegate void LobbyChatUpdate(string name, string chat);
    public event LobbyChatUpdate onLobbyChatUpdate;

    // Room Updaters
    public UnityEvent onRoomUpdate; //--> UpdateRoomFeatures() use this event only for room features (not player)
    public delegate void RoomPlayerUpdate(string name, RoomPanel.UpdateType type);
    public event RoomPlayerUpdate onRoomPlayerUpdate;

    // Invitation updater
    public delegate void ReceivingInvitation(Guid id, string sender);
    public event ReceivingInvitation onReceiveInv;

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

    public void ReceiveInvitation(Guid id, string sender)
    {
        onReceiveInv(id, sender);
    }

    public void UpdatePlayerInRoom(string name, RoomPanel.UpdateType type)
    {
        onRoomPlayerUpdate(name, type);
    }

}
