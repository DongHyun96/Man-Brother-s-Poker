using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

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

    /// Changing scene(panel) events
    public UnityEvent onValidSignUp;
    public UnityEvent onRoomToLobby;
    public UnityEvent onPlayingToLobby;

    public UnityEvent onLobbyToRoom;
    public UnityEvent onPlayingToRoom;

    public UnityEvent onPlayingState;
    

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
            switch(value)
            {
                case State.LOBBY:
                    if(state == State.PLAYING) // PLAYING -> LOBBY : Change invitable to true and notify to other players
                    {                   
                        thisPlayer.invitable = true;
                    }
                    else if(state == State.ROOM)
                    {
                        onRoomToLobby.Invoke();
                    }
                    else if(state == State.LOGIN) // SIGNUP -> LOBBY (initialize Room session)
                    {
                        onValidSignUp.Invoke();
                        
                        RoomMessage message = new RoomMessage(RoomMessage.MessageType.INIT, thisPlayer.name);
                        RoomMsgHandler.SendMessage(message);
                    }

                    break;
                case State.ROOM:
                    if(state == State.PLAYING) // PLAYING -> ROOM : Change invitable to true and notify to other players
                    {
                        thisPlayer.invitable = true;
                    }
                    else if(state == State.ROOM) // ROOM -> ROOM
                    {

                    }
                    else // LOBBY -> ROOM
                    {
                        // Initialize room features here ( room UI )
                        onLobbyToRoom.Invoke();
                    }
                    break;
                case State.PLAYING:
                    break;
            }
            m_state = value;
        }
    }

    private static State m_state = State.LOGIN;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static GameManager GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<GameManager>();

            if (instance == null)
            {
                GameObject container = new GameObject("GameManager");
                instance = container.AddComponent<GameManager>();
            }
        }
        return instance;
    }



}
