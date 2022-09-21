using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Cinemachine;

public class EntCamController : MonoBehaviour
{
    private static EntCamController m_instance;
    public static EntCamController Instance
    {
        get
        {
            if(m_instance == null) {
                m_instance = FindObjectOfType<EntCamController>();
            }
            return m_instance;
        }
        private set{ m_instance = value; }
    }

    // Character select panel
    public GameObject characterSelectObj;    

    // Cinemachine cam, lobby pos, RoomPos
    public CinemachineVirtualCamera currentCam;
    public LobbyCamPos lobbyPos;
    public RoomCamPos roomPos;
    public GameCamPos gamePos;

    private const int LOBBY_PATH_POS = 2;
    private const int ROOM_PATH_POS = 5;
    private const int GAME_PATH_POS = 8;
    private const int BETWEEN_LR = 4;

    // Changing panel events
    public EntPanelController panelAnimController;
    public UnityEvent onValidSignUp;
    public UnityEvent onRoomToLobby;
    public UnityEvent onPlayingToLobby;

    public UnityEvent onLobbyToRoom;

    public IEnumerator LogInToLobbyRoutine()
    {
        // Camera animation routine
        var dolly = currentCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = LOBBY_PATH_POS;

        // Panel anim
        panelAnimController.UpdatePanel(EntPanelController.Panel.SIGN);

        while(!lobbyPos.isTriggered)
        {
            yield return null;
        }

        // Loose cinemachine damping little bit
        dolly.m_ZDamping = 1.5f;

        // Panel init and panel animtaion routine
        onValidSignUp.Invoke();
        panelAnimController.UpdatePanel(EntPanelController.Panel.LOBBY);

        string name = GameManager.thisPlayer.name;
        RoomMessage msg = new RoomMessage(Guid.NewGuid(), RoomMessage.MessageType.INIT, name);
        RoomMsgHandler.SendMessage(msg);
    }

    public IEnumerator LobbyToRoomRoutine()
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

    public IEnumerator RoomToLobbyRoutine()
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

    public IEnumerator RoomToRoomRoutine()
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

    public IEnumerator RoomToGameRoutine()
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

    public IEnumerator GameToLobbyRoutine()
    {
        // Turn off the CharacterSelect panel
        characterSelectObj.SetActive(false);

        var dolly = currentCam.GetCinemachineComponent<CinemachineTrackedDolly>();

        // Init damping to 0 and Place camera between Room pos and Lobby pos
        dolly.m_XDamping = 0f;
        dolly.m_YDamping = 0f;
        dolly.m_ZDamping = 0f;
        dolly.m_PathPosition = BETWEEN_LR;

        yield return new WaitForSeconds(0.25f);

        // Re-assign damping
        dolly.m_XDamping = 1f;
        dolly.m_YDamping = 1f;
        dolly.m_ZDamping = 1.5f;
        
        // Cam animation routine
        dolly.m_PathPosition = LOBBY_PATH_POS;

        while(!lobbyPos.isTriggered)
        {
            yield return null;
        }

        // Panel init and panel animation routine
        panelAnimController.UpdatePanel(EntPanelController.Panel.LOBBY);
        onPlayingToLobby.Invoke();
    }
}
