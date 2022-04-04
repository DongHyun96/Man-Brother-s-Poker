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
                    // When everyone enters the gameScene
                    if(message.table.registerCount == GameManager.thisPlayerRoom.players.Count)
                    {
                        // Refresh iterPos from server (Received random start position from server)
                        GameManager.gameTable.iterPos = message.table.iterPos;

                        // Very first Game start
                        print("Game Start");
                    }
                    return;
                
                default:
                    print("No Matching message type or type is null");
                    break;
            }

            switch(message.action)
            {
                case Player.State.CHECK:
                    break;
                case Player.State.BET:
                    break;
                case Player.State.CALL:
                    break;
                case Player.State.RAISE:
                    break;
                case Player.State.FOLD:
                    break;
                case Player.State.ALLIN:
                    break;
                default:
                    print("No matching action type or type is null");
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

        // REGISTER the gameTable and this player to server
        GameMessage msg = new GameMessage(GameManager.thisPlayerRoom.id, GameManager.thisPlayer.name, GameManager.gameTable);
        SendMessage(msg);
        
    }

    


}
