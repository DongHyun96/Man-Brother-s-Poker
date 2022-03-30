using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;
using System;

public class GameMsgHandler : MonoBehaviour
{
    private static GameMsgHandler instance;

    private static WebSocket websocket = new WebSocket(UrlBase.addressBase + "gameEcho");

    public static GameMsgHandler GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<GameMsgHandler>();
        }
        return instance;
    }

    public static void SendMessage(GameMessage message)
    {
        Debug.Log("Sending game message: " + JsonConvert.SerializeObject(message));
        websocket.Send(JsonConvert.SerializeObject(message)); // Convert to Json(string) form and send
    }

    private static void ReceiveMessage(object s, MessageEventArgs e)
    {
        try
        {
            GameMessage message = JsonConvert.DeserializeObject<GameMessage>(e.Data);

            print("Game Message received --> " + message.type);

            switch(message.type)
            {
                case GameMessage.MessageType.REGISTER:
                    // Check if the room's players count == Current GameTable players count
                    
                    break;
                
                default:
                    print("No Matching message type or type is null");
                    break;
            }

        }
        catch(Exception exc)
        {
            Debug.Log("Exception occured while Receiving message from GameMsgHandler: /n" + exc);
        }
    }

    private void Start()
    {
        websocket.Connect();

        websocket.OnMessage += ReceiveMessage;

        // Send REGISTER message here
    }


}
