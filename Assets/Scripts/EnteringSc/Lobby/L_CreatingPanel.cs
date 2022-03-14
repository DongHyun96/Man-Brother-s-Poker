using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class L_CreatingPanel : MonoBehaviour
{
    public InputField title;
    public InputField password;

    public ToggleGroup buyIn;
    public ToggleGroup mode;

    public GameObject warning;

    private void Start() {
        Toggle t = mode.ActiveToggles().FirstOrDefault();
        print(t.name);
    }

    public void OnAccept()
    {
        if(title.text.Length < 1) // Check if user wrote title properly.
        {
            warning.SetActive(true);
            return;
        }

        Toggle bToggle = buyIn.ActiveToggles().FirstOrDefault();
        Toggle mToggle = mode.ActiveToggles().FirstOrDefault();

        Room.BuyIn b = GetBuyInFromToggleName(bToggle);
        Room.Mode m = GetModeFromToggleName(mToggle);

        // Set new room to GameManager
        Room newRoom = new Room(GameManager.thisPlayer.name, title.text, password.text, b, m);
        newRoom.players.Add(GameManager.thisPlayer);
        GameManager.thisPlayerRoom = newRoom;

        // Send message to server
        RoomMessage msg = new RoomMessage(RoomMessage.MessageType.REGISTER, GameManager.thisPlayer.name, newRoom);
        RoomMsgHandler.SendMessage(msg);

        // Change the GameManager state
        GameManager.GetInstance().state = GameManager.State.ROOM;
    }

    private Room.BuyIn GetBuyInFromToggleName(Toggle t)
    {
        switch(t.name)
        {
            case "1K":
                return Room.BuyIn.ONE;
            case "5K":
                return Room.BuyIn.FIVE;
            case "10K":
                return Room.BuyIn.TEN;
            case "20K":
                return Room.BuyIn.TWENTY;
            case "50K":
                return Room.BuyIn.FIFTY;
            case "100K":
                return Room.BuyIn.HUNDRED;
            default:
                return Room.BuyIn.ONE; // Default value
        }
    }

    
    private Room.Mode GetModeFromToggleName(Toggle t)
    {
        switch(t.name)
        {
            case "Chicken":
                return Room.Mode.CHICKEN;
            case "LastMan":
                return Room.Mode.LASTMAN;
            case "HeadsUp":
                return Room.Mode.HEADS;
            default:
                return Room.Mode.CHICKEN; // Default value
        }
    }


}
