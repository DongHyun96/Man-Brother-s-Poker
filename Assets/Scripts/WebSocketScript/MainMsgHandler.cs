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

    public static WebSocket webSocket = new WebSocket(UrlBase.addressBase + "mainEcho");



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
        Debug.Log("Sending some message: " + JsonConvert.SerializeObject(message));
        webSocket.Send(JsonConvert.SerializeObject(message)); // Convert to Json(string) form and send
    }



    private void ReceiveMessage(object sender, MessageEventArgs e)
    {
        Debug.Log("Message received: " + e.Data);
        try{
            MainMessage message = JsonConvert.DeserializeObject<MainMessage>(e.Data); // Received message

            Debug.Log(message.type);

            switch (message.type)
            {
                // Valid Sign up received. Add player to the Game
                case MainMessage.MessageType.SIGNUP:

                    if(GameManager.thisPlayer == null) // Sign up checked by me(this player)
                    {
                        Debug.Log("Valid Sign up");
                        
                        GameManager.thisPlayer = new Player(message.name);
                        GameManager.state = GameManager.State.LOBBY;
                        print(GameManager.state);
                    }
                    else
                    {
                        GameManager.allOthers.Add(message.name, new Player(message.name));
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
                    break;

                case MainMessage.MessageType.REMOVE:
                    GameManager.allOthers.Remove(message.name);
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
