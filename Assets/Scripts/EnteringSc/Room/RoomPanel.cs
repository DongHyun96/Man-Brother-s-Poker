using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomPanel : MonoBehaviour
{
    private const int MAXIMUM_NUM_OF_INV = 20;
    public Guid id;

    public Text title;

    public Text hostName; 

    public Toggle[] buyIn;
    public Toggle[] mode;


    public GameObject playerPanel;
    public GameObject invitingPanel;

    public GameObject playerPanel_host;   // Prefab
    public GameObject playerPanel_player; // Prefab

    public GameObject invPanel_player;    // Prefab

    public Button playBtn;
    public Button inviteBtn;
    public Button settingBtn;

    public InputField titleField;
    public InputField passwordField;

    // Prefabs Container
    private List<GameObject> playerPanel_prefabs = new List<GameObject>();
    private List<GameObject> invPanel_prefabs = new List<GameObject>();

    // For handling player updating
    public enum UpdateType{
        ENTER_GAME, LEAVE_GAME, ENTER_ROOM, LEAVE_ROOM
    }

    private void Awake() {
        EnteringSceneUpdater.GetInstance().onRoomPlayerUpdate += UpdatePlayerToPanels;
    }

    /**
    * Only use this when entering the room
    */
    public void InitRoomPanel()
    {
        // Double leaving routine here to make sure init works perfectly.
        LeavingRoutine();

        // Init id
        id = GameManager.thisPlayerRoom.id;
        
        Room room = GameManager.thisPlayerRoom;

        // Init room Features
        UpdateRoomFeatures();

        // Init host player
        hostName.text = room.host;
        
        // Check if thisPlayer is the host
        if(room.host.Equals(GameManager.thisPlayer.name))
        {
            EnableHostFunctions();
        }

        // Init playerPanel
        foreach(Player p in room.players)
        {
            if(p.name.Equals(room.host))
                continue;

            AddPlayerPanelPrefabs(p.name);
        }

        // Init InvitingPanel
        foreach(KeyValuePair<string, Player> pair in GameManager.allOthers)
        {
            if (invPanel_prefabs.Count < MAXIMUM_NUM_OF_INV && pair.Value.invitable && !isIntheRoom(pair.Value))
            {
                AddInvPanelPrefabs(pair.Value.name);
            }
            else
            {
                break;
            }
        }
        foreach(KeyValuePair<string, Player> pair in GameManager.allOthers)
        {
            if (invPanel_prefabs.Count < MAXIMUM_NUM_OF_INV && !pair.Value.invitable)
            {
                AddInvPanelPrefabs(pair.Value.name);
            }
            else
            {
                break;
            }
        }
    }

    /**
    * Update playerPanel and inviting panel based on target player
    */
    public void UpdatePlayerToPanels(string name, UpdateType type) 
    {

        switch(type)
        {
            case UpdateType.ENTER_ROOM:
            {
                // Add to playerPanel
                AddPlayerPanelPrefabs(name);

                // Remove from inv panel
                RemoveInvPanelPrefabs(name);
            }
            break;
            case UpdateType.LEAVE_ROOM:
            {
                // Init host player
                hostName.text = GameManager.thisPlayerRoom.host;

                // If thisPlayer becomes host
                if(GameManager.thisPlayerRoom.host.Equals(GameManager.thisPlayer.name))
                {
                    EnableHostFunctions();
                    hostName.text = GameManager.thisPlayer.name;
                    RemovePlayerPanelPrefabs(GameManager.thisPlayer.name); // Remove thisPlayer in normal playerPanel
                }

                // Remove from playerPanel
                RemovePlayerPanelPrefabs(name);

                // Add to invPanel
                if(invPanel_prefabs.Count < MAXIMUM_NUM_OF_INV)
                    AddInvPanelPrefabs(name);
            }
            break;
            case UpdateType.ENTER_GAME:
            {
                // update invPanel
                if(invPanel_prefabs.Count < MAXIMUM_NUM_OF_INV)
                    AddInvPanelPrefabs(name);
            }
            break;
            case UpdateType.LEAVE_GAME:
            {
                RemoveInvPanelPrefabs(name);
            }
            break;
        }
    }
    /*
    *  Update room features(Title and toggles)
    */
    public void UpdateRoomFeatures()
    {
        Room room = GameManager.thisPlayerRoom;

        title.text = room.title;

        // Init toggles
        buyIn[(int)room.buyIn].isOn = true;
        mode[(int)room.mode].isOn = true;
    }

    public void OnUpdate() // When host changes features in the room
    {
        if(GameManager.thisPlayerRoom.host.Equals(GameManager.thisPlayer.name))
        {
            // Change the room to updated features
            Room room = GameManager.thisPlayerRoom;
            
            // Update the mode and buyin
            for(int i = 0; i < buyIn.Length; i++)
            {
                if(buyIn[i].isOn)
                {
                    room.buyIn = (Room.BuyIn)i;
                    break;
                }
            }
            for(int i = 0; i < mode.Length; i++)
            {
                if(mode[i].isOn)
                {
                    room.mode = (Room.Mode)i;
                    break;
                }
            }
            
            // Send UPDATE message to the server
            RoomMessage msg = new RoomMessage(id, RoomMessage.MessageType.UPDATE, room);
            RoomMsgHandler.SendMessage(msg);
        }
    }
    public void onSettingAccept()
    {
        Room room = GameManager.thisPlayerRoom;

        // Update title
        if(!String.IsNullOrEmpty(titleField.text))
        {
            room.title = titleField.text;
        }
      
        room.password = passwordField.text;
        
        
        // Send UPDATE message to server
        RoomMessage msg = new RoomMessage(id, RoomMessage.MessageType.UPDATE, room);
        RoomMsgHandler.SendMessage(msg);

    }
    public void OnPlay()
    {
        // Game play start
    }

    public void onExit()
    {
        // Send LEAVE message to server
        RoomMessage msg = new RoomMessage(id, RoomMessage.MessageType.LEAVE, GameManager.thisPlayer.name);
        RoomMsgHandler.SendMessage(msg);

        // Empty the room panel fields and GameManager.thisRoom
        LeavingRoutine();

        // Init GameManager.thisRoom
        GameManager.thisPlayerRoom = null;

        // Change state to LOBBY
        GameManager.GetInstance().state = GameManager.State.LOBBY;

    }

    /*****************************************************************************************************************/
    /*                                               PRIVATE FUNCTIONS
    /*****************************************************************************************************************/
    private void EnableHostFunctions()
    {
        playBtn.interactable = true;
        inviteBtn.interactable = true;
        settingBtn.interactable = true;

        // Change toggles to interactive
        foreach(Toggle t in buyIn)
        {
            t.interactable = true;
        }
        foreach(Toggle t in mode)
        {
            t.interactable = true;
        }
    }


    private bool isIntheRoom(Player p)
    {
        foreach(Player player in GameManager.thisPlayerRoom.players)
        {
            if(player.name.Equals(p.name))
                return true;
        }
        return false;

    }


    private void LeavingRoutine()
    {
        // Empty all the prefabs
        foreach(GameObject o in playerPanel_prefabs)
        {
            Destroy(o);
        }
        foreach(GameObject o in invPanel_prefabs)
        {
            Destroy(o);
        }
        playerPanel_prefabs.Clear();
        invPanel_prefabs.Clear();

        // Init fields
        id = Guid.Empty;
        title.text = String.Empty;

        playBtn.interactable = false;
        inviteBtn.interactable = false;
        settingBtn.interactable = false;

        foreach(Toggle t in buyIn)
        {
            t.interactable = false;
        }
        foreach(Toggle t in mode)
        {
            t.interactable = false;
        }
        
    }


    private void AddPlayerPanelPrefabs(string name)
    {
        GameObject prefab = Instantiate(playerPanel_player, playerPanel.transform);
        prefab.GetComponent<PlayerPanel_P>().Init(name);
        playerPanel_prefabs.Add(prefab);
    }


    private void RemovePlayerPanelPrefabs(string name)
    {
        print(name);
        foreach(GameObject p in playerPanel_prefabs)
        {
            print(p.GetComponent<PlayerPanel_P>().name);
            if(p.GetComponent<PlayerPanel_P>().name.text.Equals(name))
            {
                playerPanel_prefabs.Remove(p);
                Destroy(p);
                return;
            }
        }
    }


    private void AddInvPanelPrefabs(string name)
    {
        GameObject prefab = Instantiate(invPanel_player, invitingPanel.transform);
        prefab.GetComponent<InvPanel_P>().InitContents(name, GameManager.allOthers[name].invitable);
        invPanel_prefabs.Add(prefab);
    }

    private void RemoveInvPanelPrefabs(string name)
    {
        foreach(GameObject p in invPanel_prefabs)
        {
            if(p.GetComponent<InvPanel_P>().name.text.Equals(name))
            {
                invPanel_prefabs.Remove(p);
                Destroy(p);
                return;
            }
        }
    }

    /*****************************************************************************************************************/
    /*                                               Inviting panel toggler
    /*****************************************************************************************************************/
    public void ToggleInvPanel()
    {
        EntPanelController.GetInstance().UpdatePanel(EntPanelController.Panel.INVITE);
    }

}

