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
        websocket.Send(JsonConvert.SerializeObject(message, Formatting.Indented)); // Convert to Json(string) form and send
    }

    public static void TossTable()
    {
        Debug.Log("Tossing gameTable");

        GameMessage message = new GameMessage(GameManager.thisPlayerRoom.id, GameMessage.MessageType.TOSS,
        GameManager.thisPlayer.name, GameManager.gameTable);

        websocket.Send(JsonConvert.SerializeObject(message, Formatting.Indented));
    }

    private static void ReceiveMessage(object s, MessageEventArgs e)
    {
        try
        {
            GameMessage message = JsonConvert.DeserializeObject<GameMessage>(e.Data); // FATAL ERROR HERE
           
            print("Game Message received --> " + message.type);

            switch(message.type)
            {
                case GameMessage.MessageType.REGISTER:
#if TEST
                    if(GameManager.thisPlayer.name.Equals("InitialName"))
                    {
                        GameManager.thisPlayer.name = (message.table.registerCount - 1).ToString();
                    }
#endif
                    if(message.table.registerCount == GameManager.thisPlayerRoom.players.Count)
                    {   
                        UnityMainThread.wkr.AddJob(() => 
                        {
                            // Refresh iterPos from server (Received random start position from server)
                            GameManager.gameTable.iterPos = message.table.iterPos;
            
                            GameManager.gameTable.UTG = message.table.iterPos;

                            // Received shuffled deck and set it to table
                            GameManager.gameTable.deck = message.table.deck;

                            // Init game settings
                            GameSceneUpdater.GetInstance().InitSettings();
                            
                            // Init PREFLOP
                            GameManager.gameTable.stage = GameTable.Stage.PREFLOP;

                            // Very first Game start
                            print("Game Start");
                            GameSceneUpdater.GetInstance().StartRound();
                        });
                    }
                    break;
                case GameMessage.MessageType.TOSS:
                    UnityMainThread.wkr.AddJob(() => {

                        GameManager.gameTable = message.table; // Update gameTable

                        print("Current iterPos: " + message.table.iterPos);
                        Player targetPlayer = GameManager.gameTable.GetPlayerByName(message.sender);
                        GameSceneUpdater.GetInstance().UpdateGameScene(targetPlayer); //Update UI
                    });
                    break;
                case GameMessage.MessageType.LEAVE:
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
        

#if TEST
        Guid guid = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e");

        GameManager.thisPlayerRoom = new Room(guid, "0", "Title", "1234", Room.BuyIn.TWENTY, Room.Mode.CHICKEN);
        const int PLAYERCNT = 4;
        for (int i = 0; i < PLAYERCNT; i++)
        {
            GameManager.thisPlayerRoom.players.Add(new Player(i.ToString()));
        }

        GameManager.gameTable = new GameTable(guid, GameManager.thisPlayerRoom.players,
         GameManager.thisPlayerRoom.mode, GameManager.thisPlayerRoom.buyIn);
        
        GameManager.thisPlayer = new Player("InitialName");

        // Init UnityMainThread
        GameObject container = new GameObject("UnityMainThread");
        container.AddComponent<UnityMainThread>();
        
#endif

        // REGISTER the gameTable and this player to server
        GameMessage msg = new GameMessage(GameManager.thisPlayerRoom.id, GameMessage.MessageType.REGISTER,
         GameManager.thisPlayer.name, GameManager.gameTable);
        SendMessage(msg);

    }

}
