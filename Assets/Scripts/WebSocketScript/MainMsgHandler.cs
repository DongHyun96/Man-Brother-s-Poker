using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;

public class MainMsgHandler : MonoBehaviour
{
    private static MainMsgHandler instance;

    private static WebSocket webSocket = new WebSocket(UrlBase.addressBase + "mainEcho");



    void Awake()
    {
        webSocket.Connect();
        DontDestroyOnLoad(this.gameObject);
    }



    public static MainMsgHandler GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<MainMsgHandler>();

            if (instance == null)
            {
                GameObject container = new GameObject("MainMsgHandler");
                instance = container.AddComponent<MainMsgHandler>();
            }
        }
        return instance;
    }



    public static void SendMessage(MainMessage message)
    {
        Debug.Log("Sending main message: " + JsonConvert.SerializeObject(message));
        webSocket.Send(JsonConvert.SerializeObject(message)); // Convert to Json(string) form and send
    }



    private void ReceiveMessage(object sender, MessageEventArgs e)
    {
        try{
            MainMessage message = JsonConvert.DeserializeObject<MainMessage>(e.Data); // Received message

            Debug.Log("Main Message received --> " + message.type);

            switch (message.type)
            {
                // Valid Sign up received. Add player to the Game
                case MainMessage.MessageType.SIGNUP:
                    
                    if(String.IsNullOrEmpty(GameManager.thisPlayer.name)) // Sign up checked by me(this player)
                    {
                        Debug.Log("Valid Sign up");
                        
                        // Set thisPlayer's name & change state to LOBBY
                        UnityMainThread.wkr.AddJob(() => {
                            GameManager.thisPlayer.name = message.name;

                            // Init allOthers
                            Dictionary<string, Player> receivedPlayers = message.playerMap;

                            receivedPlayers.Remove(GameManager.thisPlayer.name);  // Remove self
                            GameManager.allOthers = receivedPlayers;
  
                            GameManager.GetInstance().state = GameManager.State.LOBBY;
                        });
                    }
                    else // Valid Sign up by other.
                    {
                        UnityMainThread.wkr.AddJob(() => {
                            
                            GameManager.allOthers.Add(message.name, new Player(message.name, message.character));

                            // Check whether current state is LOBBY and then update players panel
                            if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                            {
                                EnteringSceneUpdater.GetInstance().onLobbyPlayersUpdate.Invoke();
                            }
                            else if(GameManager.GetInstance().state == GameManager.State.ROOM) // Update inviting panel
                            {
                                EnteringSceneUpdater.GetInstance().UpdatePlayerInRoom(message.name, RoomPanel.UpdateType.INV_ADD);
                            }
                        });
                    }
                    break;

                // Invalid Sign up
                case MainMessage.MessageType.DENIED:
                    // Show warning text in main thread
                    UnityMainThread.wkr.AddJob(() => {
                        SignUpPanel.warning.SetActive(true);
                    });
                    break;

                case MainMessage.MessageType.CHAT:
                    UnityMainThread.wkr.AddJob(() => {
                        // check LOBBY state
                        if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                        {
                            EnteringSceneUpdater.GetInstance().UpdateChat(message.name, message.msg);
                        }
                    });
                    break;

                case MainMessage.MessageType.REMOVE:
                    UnityMainThread.wkr.AddJob(() => {
                        GameManager.allOthers.Remove(message.name);

                        if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                        {
                            EnteringSceneUpdater.GetInstance().onLobbyPlayersUpdate.Invoke();
                        }
                        else if(GameManager.GetInstance().state == GameManager.State.ROOM)
                        {
                            // Update inviting panel in Room Panel
                            EnteringSceneUpdater.GetInstance().UpdatePlayerInRoom(message.name, RoomPanel.UpdateType.INV_REMOVE);
                        }
                    });
                    break;

                case MainMessage.MessageType.UPDATE: // Update player's invitable status

                    if(!message.name.Equals(GameManager.thisPlayer.name))
                    {
                        UnityMainThread.wkr.AddJob(() => {
                    
                            GameManager.allOthers[message.name].invitable = message.invitable;

                            if(GameManager.GetInstance().state == GameManager.State.LOBBY)
                            {
                                EnteringSceneUpdater.GetInstance().onLobbyPlayersUpdate.Invoke();
                            }
                            else if(GameManager.GetInstance().state == GameManager.State.ROOM)
                            {
                                if(message.invitable)
                                    EnteringSceneUpdater.GetInstance().UpdatePlayerInRoom(message.name, RoomPanel.UpdateType.INV_ADD);
                                else
                                    EnteringSceneUpdater.GetInstance().UpdatePlayerInRoom(message.name, RoomPanel.UpdateType.INV_REMOVE);
                            }
                        });
                    }
                    
                    break;
                case MainMessage.MessageType.GET:
                    // Refresh GameManager's allOthers
                    UnityMainThread.wkr.AddJob(() => {
                        Dictionary<string, Player> receivedPlayers = message.playerMap;

                        receivedPlayers.Remove(GameManager.thisPlayer.name);  // Remove self
                        GameManager.allOthers = receivedPlayers;

                        EnteringSceneUpdater.GetInstance().onLobbyPlayersUpdate.Invoke();
                    });
                    break;
                default:

                    break;
            }
        } catch(Exception exc) {
            Debug.Log("Exception occured while Receiving message from MainMsgHandler: /n" + exc);
        }   
    }



    void Start() {
        // Listener
        webSocket.OnMessage += ReceiveMessage;
    }

}
