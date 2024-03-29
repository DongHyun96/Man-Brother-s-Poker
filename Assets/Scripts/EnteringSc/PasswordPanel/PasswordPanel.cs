using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PasswordPanel : MonoBehaviour
{
    private static PasswordPanel m_instance;

    public static PasswordPanel Instance
    {
        get => m_instance;
        private set{ m_instance = value; }
    }
    private Guid id;

    [SerializeField] private InputField inputField;
    [SerializeField] private Text warning;

    private void Awake() {
        if(Instance == null)
        {
            Instance = this.GetComponent<PasswordPanel>();
        }
        this.gameObject.SetActive(false);
    }

    public void OnEnterBtnPressed()
    {
        // Check if password is valid
        if(!inputField.text.Equals(GameManager.rooms[id].password))
        {
            warning.gameObject.SetActive(true);
            return;
        }

        // Add me to gameManager.rooms and init gameManager.thisRoom
        Room targetRoom = GameManager.rooms[id];
        targetRoom.players.Add(GameManager.thisPlayer);
        GameManager.thisPlayerRoom = targetRoom;

        // Send ENTER message to server
        RoomMessage msg = new RoomMessage(id, RoomMessage.MessageType.ENTER, GameManager.thisPlayer.name);
        RoomMsgHandler.SendMessage(msg);

        // Change the current state
        GameManager.GetInstance().state = GameManager.State.ROOM;

        // Hide this Panel
        this.gameObject.SetActive(false);

    }

    public void OpenPanel(Guid id)
    {
        this.gameObject.SetActive(true);
        this.id = id;

        inputField.text = String.Empty;
    }
}
