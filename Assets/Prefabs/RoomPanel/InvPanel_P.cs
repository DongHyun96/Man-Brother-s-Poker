using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InvPanel_P : MonoBehaviour
{
    public Text name;

    public GameObject button;
    
    public GameObject timer;
    public Image fill;
    private const float t = 10f;

    public bool isTimerActive{
        set{
            if(value == true)
            {
                button.SetActive(false);
                timer.SetActive(true);
                _isTimerActive = true;
            }
            else
            {
                button.SetActive(true);
                timer.SetActive(false);
                _isTimerActive = false;

                // Reset fill
                fill.fillAmount = 0f;
            }
        }
        get{
            return _isTimerActive;
        }
    }

    private bool _isTimerActive;

    public void InitContents(string name, bool invitable)
    {
        this.name.text = name;
        if(!invitable)
            button.SetActive(false);
    }

    public void onBtnPressed()
    {
        isTimerActive = true;

        // Send invitation to target player
        RoomMessage m = new RoomMessage(GameManager.thisPlayerRoom.id, RoomMessage.MessageType.INVITE,
        GameManager.thisPlayer.name, name.text);
        RoomMsgHandler.SendMessage(m);
    }

    private void Update() {
        if(isTimerActive)
        {
            
            if(fill.fillAmount < 1)
            {
                fill.fillAmount += Time.deltaTime * (1 / t);
            }
            else
            {
                isTimerActive = false;
            }
        }
    }
    
}
