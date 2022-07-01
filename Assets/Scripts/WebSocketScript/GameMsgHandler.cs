using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;
using System;

public class GameMsgHandler : MonoBehaviour
{
    private static WebSocket websocket = new WebSocket(UrlBase.addressBase + "gameEcho");

    public static void DisconnectSocket()
    {
        try
        {
            if(websocket == null)
            {
                return;
            }
            if(websocket.IsAlive)
            {
                websocket.OnMessage -= ReceiveMessage;
                websocket.Close();
            }
        } catch(Exception e)
        {
            Debug.Log("From GameMsgHandler DisconnectSocket(): " + e.ToString());
        }
    }

    public static void SendMessage(GameMessage message)
    {
        Debug.Log("Sending game message: " + JsonConvert.SerializeObject(message));
        //websocket.Send(JsonConvert.SerializeObject(message, Formatting.Indented)); // Convert to Json(string) form and send
        websocket.Send(JsonConvert.SerializeObject(message)); // Convert to Json(string) form and send

    }

    public static void TossTable()
    {
        Debug.Log("Tossing gameTable --> " + GameManager.gameTable.stage);
    
        GameMessage message = new GameMessage(GameManager.thisPlayerRoom.id, GameMessage.MessageType.TOSS,
        GameManager.thisPlayer.name, GameManager.gameTable);

        try{
        //websocket.Send(JsonConvert.SerializeObject(message, Formatting.Indented));
            websocket.Send(JsonConvert.SerializeObject(message));
        }catch(Exception e)
        {
            print(e.ToString());
        }
        //websocket.Send(JsonConvert.SerializeObject(message, Formatting.Indented));
    }

    private static void ReceiveMessage(object s, MessageEventArgs e)
    {
        try
        {
            GameMessage message = JsonConvert.DeserializeObject<GameMessage>(e.Data);
           
            print("Game Message received --> " + message.type);

            switch(message.type)
            {
                case GameMessage.MessageType.DECK:
                    UnityMainThread.wkr.AddJob(() => 
                    {
                        /* Init deck here */
                        GameManager.gameTable.deck = message.deck;
                    });
                    break;
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
                            // Set SB_pos
                            GameManager.gameTable.SB_Pos = message.table.SB_Pos;

#if TEST
                            // Set characters
                            for(int i = 0; i < GameManager.gameTable.players.Count; i++)
                            {
                                GameManager.gameTable.players[i].character = message.table.players[i].character;
                            }
#endif

                            // Init game settings
                            GameSceneUpdater.GetInstance().InitSettings();
                            
                            // Init PREFLOP
                            GameManager.gameTable.stage = GameTable.Stage.PREFLOP;

                            // Very first Game start
                            print("Game Start");
                            GameSceneUpdater.GetInstance().StartGame();
                            GameSceneUpdater.GetInstance().isFirstGameStart = true;
                        });
                    }
                    break;
                case GameMessage.MessageType.TOSS:
                    UnityMainThread.wkr.AddJob(() => {

                        List<Card> deck = GameManager.gameTable.deck;
                        GameManager.gameTable = message.table; // Update gameTable
                        GameManager.gameTable.deck = deck; // Set deck again (because server - client doesn't share deck in TOSS msg)

                        Player targetPlayer = GameManager.gameTable.GetPlayerByName(message.sender);
                        GameSceneUpdater.GetInstance().UpdateGameScene(targetPlayer); //Update UI
                    });
                    break;
                case GameMessage.MessageType.SHOWDOWN:
                    UnityMainThread.wkr.AddJob(() => {
                        /* Show down cards */
                        GameSceneUpdater.GetInstance().ShowDownCardsByPlayer(message.sender, message.cardShowDown);
                    });
                    break;
                case GameMessage.MessageType.LEAVE:
                    UnityMainThread.wkr.AddJob(() => {
                        // When someone left the game
                        GameSceneUpdater.GetInstance().OnPlayerLeft(message.sender);
                    });
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
    private void Awake() 
    {
        websocket.Connect();
        websocket.OnMessage += ReceiveMessage;
    }

    private void Start()
    {
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
        StartCoroutine(DelayRegisterRoutine());

    }

    private IEnumerator DelayRegisterRoutine()
    {
        // Wait for random seconds
        System.Random rnd = new System.Random();
        float rand = rnd.Next(0, 7);
        yield return new WaitForSeconds(rand);
        
        GameMessage msg = new GameMessage(GameManager.thisPlayerRoom.id, GameMessage.MessageType.REGISTER,
         GameManager.thisPlayer.name, GameManager.gameTable);
        SendMessage(msg);
    }

}
