using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Cinemachine;

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

    /// Cinemachine cam & lobbyPos, RoomPos
    public CinemachineVirtualCamera currentCam;
    public LobbyCamPos lobbyPos;
    public RoomCamPos roomPos;
    public GameCamPos gamePos;

    private const int LOBBY_PATH_POS = 2;
    private const int ROOM_PATH_POS = 5;
    private const int GAME_PATH_POS = 8;

    /// Changing scene(panel) events
    public EntPanelController panelAnimController;
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
                    else if(state == State.ROOM) // ROOM -> LOBBY
                    {
                        StartCoroutine(RoomToLobbyRoutine());
                    }
                    else if(state == State.LOGIN) // SIGNUP -> LOBBY
                    {
                        StartCoroutine(LogInToLobbyRoutine());
                    }

                    break;
                case State.ROOM:
                    if(state == State.PLAYING) // PLAYING -> ROOM : Change invitable to true and notify to other players
                    {
                        thisPlayer.invitable = true;
                    }
                    else if(state == State.ROOM) // ROOM -> ROOM
                    {
                        StartCoroutine(RoomToRoomRoutine());
                    }
                    else // LOBBY -> ROOM
                    {
                        StartCoroutine(LobbyToRoomRoutine());
                    }
                    break;
                case State.PLAYING: // ROOM -> PLAYING
                    StartCoroutine(RoomToGameRoutine());
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

    private IEnumerator LogInToLobbyRoutine()
    {
        // camera animation routine
        var dolly = currentCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = LOBBY_PATH_POS;

        // Panel Anim
        panelAnimController.UpdatePanel(EntPanelController.Panel.SIGN);

        while(!lobbyPos.isTriggered)
        {
            yield return null;
        }

        // Loose cinemachine damping little bit
        dolly.m_ZDamping = 1.5f;

        // Panel init and panel animation routine
        onValidSignUp.Invoke();
        panelAnimController.UpdatePanel(EntPanelController.Panel.LOBBY);

        RoomMessage msg = new RoomMessage(Guid.NewGuid(), RoomMessage.MessageType.INIT, thisPlayer.name);
        RoomMsgHandler.SendMessage(msg);
    }

    private IEnumerator LobbyToRoomRoutine()
    {
        // Cam animation routine
        var dolly = currentCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = ROOM_PATH_POS;

        // Panel anim
        panelAnimController.UpdatePanel(EntPanelController.Panel.LOBBY);

        while(!roomPos.isTriggered)
        {
            yield return null;
        }

        // Panel init and panel animation routine
        panelAnimController.UpdatePanel(EntPanelController.Panel.ROOM);
        onLobbyToRoom.Invoke();
    }

    private IEnumerator RoomToLobbyRoutine()
    {
        // Cam animation routine
        var dolly = currentCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = LOBBY_PATH_POS;

        // Panel anim
        panelAnimController.UpdatePanel(EntPanelController.Panel.ROOM);

        while(!lobbyPos.isTriggered)
        {
            yield return null;
        }

        // Panel init and panel animation routine
        panelAnimController.UpdatePanel(EntPanelController.Panel.LOBBY);
        onRoomToLobby.Invoke();
    }

    private IEnumerator RoomToRoomRoutine()
    {
        // Cam animation routine
        var dolly = currentCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = LOBBY_PATH_POS;

        // Panel anim
        panelAnimController.UpdatePanel(EntPanelController.Panel.ROOM);

        while(!lobbyPos.isTriggered)
        {
            yield return null;
        }

        // Cam animation again (back to room again)
        dolly.m_PathPosition = ROOM_PATH_POS;
        
        while(!roomPos.isTriggered)
        {
            yield return null;
        }

        // Panel init and return to room panel
        onLobbyToRoom.Invoke();
        panelAnimController.UpdatePanel(EntPanelController.Panel.ROOM);
    }

    private IEnumerator RoomToGameRoutine()
    {
        // Cam animation routine
        var dolly = currentCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = GAME_PATH_POS;

        // Panel anim
        panelAnimController.UpdatePanel(EntPanelController.Panel.ROOM);

        while(!gamePos.isTriggered)
        {
            yield return null;
        }
        
        // Change scene to gameScene
        LoadingSceneController.LoadScene("GameScene");
    }

}
