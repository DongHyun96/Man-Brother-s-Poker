using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomButtonPrefab : MonoBehaviour
{
    public Text title;
    public Image dot;
    public Text num;
    public Text buyIn;

    public Text chicken;
    public Text lastMan;
    public Text heads;

    public Text OX;

    private Color greenDot = new Color(24/255f, 255/255f, 8/255f);
    private Color redDot = new Color(245/255f, 78/255f ,81/255f);

    private Color modeRed = new Color(243/255f, 86/255f, 29/255f);
    private Color modeGrey = new Color(149/255f, 149/255f, 149/255f);

    public Guid id;

    public void SetContents(Guid id)
    {
        this.id = id;

        Room room = GameManager.rooms[id];

        this.title.text = room.title;

        // Check if the room is available to enter
        if(room.mode == Room.Mode.HEADS)
        {
            dot.color = (!room.isPlaying && room.players.Count < 2) ? greenDot : redDot;
            this.gameObject.GetComponent<Button>().interactable = (dot.color == redDot) ? false : true;
        }
        else
        {
            dot.color = (!room.isPlaying && room.players.Count < 6) ? greenDot : redDot;
            this.gameObject.GetComponent<Button>().interactable = (dot.color == redDot) ? false : true;
        }

        num.text = (room.mode == Room.Mode.HEADS) ? room.players.Count + "/" + "2" : room.players.Count + "/" + "6";

        switch(room.buyIn)
        {
            case Room.BuyIn.ONE:
                buyIn.text = "1k";
                break;
            case Room.BuyIn.FIVE:
                buyIn.text = "5k";
                break;
            case Room.BuyIn.TEN:
                buyIn.text = "10k";
                break;
            case Room.BuyIn.TWENTY:
                buyIn.text = "20k";
                break;
            case Room.BuyIn.FIFTY:
                buyIn.text = "50k";
                break;
            case Room.BuyIn.HUNDRED:
                buyIn.text = "100k";
                break;
            default:
                break;
        }

        switch(room.mode)
        {
            case Room.Mode.CHICKEN:
                chicken.color = modeRed;
                break;
            case Room.Mode.LASTMAN:
                lastMan.color = modeRed;
                break;
            case Room.Mode.HEADS:
                heads.color = modeRed;
                break;
            default:
             break;
        }

        if(String.IsNullOrEmpty(room.password))
        {
            OX.text = "X";
        }
        else
        {
            OX.text = "O";
        }


    }

    public void onBtnPressed()
    {
        // check if the ENTER is valid
        /*
        if (GameManager.rooms[id].players.Count >= 6 || GameManager.rooms[id].isPlaying)
        {
            return;
        }
        */
        //if(GameManager.rooms[id])

        // Check if the room has required password
        if(!String.IsNullOrEmpty(GameManager.rooms[id].password))
        {
            PasswordPanel.Instance.OpenPanel(id);
            return;
        }
        
        // Add me to GameManager.rooms and init gameManager.thisRoom
        Room targetRoom = GameManager.rooms[id];
        targetRoom.players.Add(GameManager.thisPlayer);
        GameManager.thisPlayerRoom = targetRoom;

        // Send ENTER message to server
        RoomMessage message = new RoomMessage(id, RoomMessage.MessageType.ENTER, GameManager.thisPlayer.name);
        RoomMsgHandler.SendMessage(message);

        // Change the current state
        GameManager.GetInstance().state = GameManager.State.ROOM;

    }


}
