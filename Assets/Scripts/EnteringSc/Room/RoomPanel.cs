using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomPanel : MonoBehaviour
{
    public Guid id;

    public Text title;

    //public ToggleGroup buyIn;
    //public ToggleGroup mode;

    public Toggle[] buyIn;
    public Toggle[] mode;


    public GameObject playerPanel;
    public GameObject invitingPanel;

    public GameObject playerPanel_host;   // Prefab
    public GameObject playerPanel_player; // Prefab

    public GameObject invPanel_player;    // Prefab

    public Button playBtn;
    public Button inviteBtn;

    public void InitRoomPanel()
    {
        // Init prefabs
        foreach(Transform child in playerPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in invitingPanel.transform)
        {
            Destroy(child.gameObject);
        }

        Room room = GameManager.thisPlayerRoom;

        title.text = room.title;
        
        // Init Toggles
        buyIn[(int)room.buyIn].isOn = true;
        mode[(int)room.mode].isOn = true;

        // Init host player
        GameObject host = Instantiate(playerPanel_host, playerPanel.transform);

        // Check if thisPlayer is the host
        if(room.host.Equals(GameManager.thisPlayer.name))
        {

            playBtn.interactable = true;
            inviteBtn.interactable = true;

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

        // Init playerPanel
        foreach(Player p in room.players)
        {
            if(p.name.Equals(room.host))
                continue;
            GameObject prefab = Instantiate(playerPanel_player, playerPanel.transform);
            prefab.GetComponent<PlayerPanel_P>().Init(p.name);
        }

        // Init InvitingPanel
        int count = 0;
        foreach(KeyValuePair<string, Player> pair in GameManager.allOthers)
        {
            if (count < 20 && pair.Value.invitable)
            {
                GameObject prefab = Instantiate(invPanel_player, invitingPanel.transform);
                prefab.GetComponent<InvPanel_P>().InitContents(pair.Value.name, pair.Value.invitable);
            }
            else
            {
                break;
            }
            count++;
        }
        foreach(KeyValuePair<string, Player> pair in GameManager.allOthers)
        {
            if (count < 20 && !pair.Value.invitable)
            {
                GameObject prefab = Instantiate(invPanel_player, invitingPanel.transform);
                prefab.GetComponent<InvPanel_P>().InitContents(pair.Value.name, pair.Value.invitable);
            }
            else
            {
                break;
            }
            count++;
        }
    }

    public void onPlay()
    {

    }

    public void onSetting()
    {

    }

}

