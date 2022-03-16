using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;
using System;

public class RoomMsgHandler : MonoBehaviour
{
    private static RoomMsgHandler instance;

    private static WebSocket webSocket = new WebSocket(UrlBase.addressBase + "roomEcho");

    private void Awake() {
        webSocket.Connect();
        DontDestroyOnLoad(this.gameObject);
    }
    
    public static RoomMsgHandler GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<RoomMsgHandler>();

            if(instance == null)
            {
                GameObject container = new GameObject("RoomMsgHandler");
                instance = container.AddComponent<RoomMsgHandler>();
            }
        }
        return instance;
    }

    public static void SendMessage(RoomMessage message)
    {
        Debug.Log("Sending room message: " + JsonConvert.SerializeObject(message));
        webSocket.Send(JsonConvert.SerializeObject(message)); // Convert to Json(string) form and send
    }

    private void ReceiveMessage(object s, MessageEventArgs e)
    {
        try
        {
            RoomMessage message = JsonConvert.DeserializeObject<RoomMessage>(e.Data);

            print("Room message received --> " + message.type);

            switch(message.type)
            {
                case RoomMessage.MessageType.REGISTER:
            
                    // Register room to GameManager
                    GameManager.rooms.Add(message.id, message.room);

                    UnityMainThread.wkr.AddJob(() => {
                        // Update RoomPanel if currently LOBBY state
                        if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                        {
                            EnteringSceneUpdater.GetInstance().onLobbyRoomsUpdate.Invoke();
                        }
                    });
                    break;
                
                case RoomMessage.MessageType.ENTER:
                    
                    UnityMainThread.wkr.AddJob(() => {
                    // Add player to the room (if the player is not me) / just change the room to received room
                    if(!message.sender.Equals(GameManager.thisPlayer.name))
                    {
                        GameManager.rooms[message.id] = message.room;

                        if(GameManager.GetInstance().state == GameManager.State.LOBBY) // If LOBBY state, update roomPanel
                        {
                            EnteringSceneUpdater.GetInstance().onLobbyRoomsUpdate.Invoke();
                            return;
                        }

                        if(GameManager.thisPlayerRoom.id == message.id) // If the room is currently this player's room
                        {
                            GameManager.thisPlayerRoom = message.room;
                            EnteringSceneUpdater.GetInstance().UpdatePlayerInRoom(message.sender, RoomPanel.UpdateType.ENTER_ROOM);
                        }
                        
                    }
                    });

                    break;
                case RoomMessage.MessageType.LEAVE:
                    UnityMainThread.wkr.AddJob(() => {
                        GameManager.rooms[message.id] = message.room;

                        if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                        {
                            EnteringSceneUpdater.GetInstance().onLobbyRoomsUpdate.Invoke();
                            return;
                        }
                        if(GameManager.thisPlayerRoom.id == message.id)
                        {
                            GameManager.thisPlayerRoom = message.room;
                            EnteringSceneUpdater.GetInstance().UpdatePlayerInRoom(message.sender, RoomPanel.UpdateType.LEAVE_ROOM);
                        }
                    });
                    break;
                case RoomMessage.MessageType.INVITE: // Show Invitation panel
                    UnityMainThread.wkr.AddJob(() => {
                        print(message.id);
                        print(message.sender);
                        EnteringSceneUpdater.GetInstance().ReceiveInvitation(message.id, message.sender);
                    });
                    break;
                case RoomMessage.MessageType.UPDATE: // Update room features(No player info change involved)
                    UnityMainThread.wkr.AddJob(() => {
                        // Update table
                        GameManager.rooms[message.id] = message.room;

                        if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                        {
                            EnteringSceneUpdater.GetInstance().onLobbyRoomsUpdate.Invoke();
                            return;
                        }
                        if(GameManager.thisPlayerRoom.id == message.id) // Current room feature changed
                        {
                            GameManager.thisPlayerRoom = message.room;
                            EnteringSceneUpdater.GetInstance().onRoomUpdate.Invoke();
                        }
                    });
                    break;
                case RoomMessage.MessageType.REMOVE_ROOM:
                    UnityMainThread.wkr.AddJob(() => {
                        // Remove room from the table
                        GameManager.rooms.Remove(message.id);

                        if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                        {
                            EnteringSceneUpdater.GetInstance().onLobbyRoomsUpdate.Invoke();
                        }

                    });
                    break;
                case RoomMessage.MessageType.GET: // Get room table
                    // Init room tables
                    UnityMainThread.wkr.AddJob(() => {
                        Dictionary<Guid, Room> receivedRooms = message.roomMap;
                        GameManager.rooms = receivedRooms;
                        if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                            EnteringSceneUpdater.GetInstance().onLobbyRoomsUpdate.Invoke();
                    });
                    break;
                default:
                    break;
                
            }
        }
        catch(Exception exc)
        {
            Debug.Log("Exception occured while Receiving message from RoomMsgHandler: /n" + exc);
        }
    }

    private void Start() {
        webSocket.OnMessage += ReceiveMessage;
    }
    
}
