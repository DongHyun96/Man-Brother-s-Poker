using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Invitation : MonoBehaviour
{
    private Guid id;
    public Text sender;
    
    public Image fill;
    private const float t = 10f;

    void Awake() { // Subscribe to EnteringSceneUpdater delegate event
        EnteringSceneUpdater.GetInstance().onReceiveInv += Init;
        this.gameObject.SetActive(false);
    }

    public void Init(Guid id, string sender)
    {
        this.id = id;
        this.sender.text = sender;
        fill.fillAmount = 1f;
        this.gameObject.SetActive(true);
    }

    public void OnAccept()
    {
        // check if the ENTER is valid
        if(GameManager.rooms[id].players.Count >= 6 || GameManager.rooms[id].isPlaying)
        {
            this.gameObject.SetActive(false);
            return;
        }

        // Add me to GameManager.rooms and init gameManager.thisRoom
        Room targetRoom = GameManager.rooms[id];
        targetRoom.players.Add(GameManager.thisPlayer);
        GameManager.thisPlayerRoom = targetRoom;

        // Send ENTER message to server
        RoomMessage m = new RoomMessage(id, RoomMessage.MessageType.ENTER, GameManager.thisPlayer.name);
        RoomMsgHandler.SendMessage(m);

        // Change the current state
        GameManager.GetInstance().state = GameManager.State.ROOM;

        // Reset panel
        Reset();

    }

    public void onDeny()
    {
        Reset();
    }

    private void Update() {

        if(this.gameObject.activeSelf)
        {
            if(fill.fillAmount > 0)
            {
                fill.fillAmount -= Time.deltaTime * (1/t);
            }
            else // End of timer(pretend to be rejected)
            {
                Reset();
            }
        }

    }

    private void Reset()
    {
        // Reset fields
        fill.fillAmount = 1f;
        sender.text = "";
        id = Guid.Empty;
        this.gameObject.SetActive(false);
    }
}
