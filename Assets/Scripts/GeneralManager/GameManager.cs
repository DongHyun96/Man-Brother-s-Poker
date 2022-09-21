using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static Player thisPlayer;
    
    private static GameManager instance;

    // For faster search, Using dictionary instead of list, (name, Player) pair
    public static Dictionary<string, Player> allOthers = new Dictionary<string, Player>();

    // RoomDict (Guid, Room) pair
    public static Dictionary<Guid, Room> rooms = new Dictionary<Guid, Room>();

    // This player's current room
    public static Room thisPlayerRoom;
    
    // This player's game table
    public static GameTable gameTable;

    // Ent scene index
    private const int ENT_SCENE_IDX = 1;

    public enum State{
        LOGIN, LOBBY, ROOM, PLAYING
    }
    
    /*
    public delegate void ValidSignUp(PanelAnimController.Panel p, PanelAnimController.Status s);
    public event ValidSignUp onValidSignUp;
    */
    public State state{

        get => m_state;

        set{

            EntCamController camController = EntCamController.Instance;

            switch(value)
            {
                case State.LOBBY:
                    if(state == State.PLAYING) // PLAYING -> LOBBY
                    {                   
                        /* thisPlayer.invitable = true; */
                        StartCoroutine(PlayingToLobbyRoutine());
                    }
                    else if(state == State.ROOM) // ROOM -> LOBBY
                    {
                        StartCoroutine(camController.RoomToLobbyRoutine());
                    }
                    else if(state == State.LOGIN) // SIGNUP -> LOBBY
                    {
                        StartCoroutine(camController.LogInToLobbyRoutine());
                    }

                    break;
                case State.ROOM:
                    if(state == State.PLAYING) // PLAYING -> ROOM (not supported)
                    {
                        throw new NotImplementedException();
                    }
                    else if(state == State.ROOM) // ROOM -> ROOM
                    {
                        StartCoroutine(camController.RoomToRoomRoutine());
                    }
                    else // LOBBY -> ROOM
                    {
                        StartCoroutine(camController.LobbyToRoomRoutine());
                    }
                    break;
                case State.PLAYING: // ROOM -> PLAYING

                    // Init thisPlayertable
                    gameTable = new GameTable(thisPlayerRoom.id, thisPlayerRoom.players,
                     thisPlayerRoom.mode, thisPlayerRoom.buyIn);
                    
                    StartCoroutine(camController.RoomToGameRoutine()); // Changing scene and cinemachine cam.
                    break;
            }
            m_state = value;
        }
    }

    private static State m_state = State.LOGIN;

    void Awake()
    {
        // DontDestroyOnLoad(this.gameObject);
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    private IEnumerator PlayingToLobbyRoutine()
    {
        // Load scene
        LoadingSceneController.LoadScene("EnteringScene");

        // Wait until Ent scene fully loaded
        while(SceneManager.GetActiveScene().buildIndex != ENT_SCENE_IDX)
        {
            yield return null;
        }

        StartCoroutine(EntCamController.Instance.GameToLobbyRoutine());
    }

}
