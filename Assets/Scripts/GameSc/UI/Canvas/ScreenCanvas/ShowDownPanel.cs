using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDownPanel : MonoBehaviour
{
    [SerializeField] private Timer timer;
    [SerializeField] private Image UpperLeft;
    [SerializeField] private Image UpperRight;
    [SerializeField] private Image left;
    [SerializeField] private Image right;

    public void InitShowDown(List<Card> cards)
    {
        // If cards are empty, return
        if(cards.Count < 2)
        {
            return;
        }
        
        // Init Upper
        UpperLeft.sprite = CardSprite.GetInstance().GetSprite(cards[0]);
        UpperRight.sprite = CardSprite.GetInstance().GetSprite(cards[1]);

        // Init left
        left.sprite = CardSprite.GetInstance().GetSprite(cards[0]);

        // Init right
        right.sprite = CardSprite.GetInstance().GetSprite(cards[1]);
    }

    private void OnEnable() 
    {
        timer.IsTimerActive = true;
    }

    private void OnDisable() 
    {
        // Turn off timer sfx
        timer.StopSfx();    
    }

    public void OnUpper()
    {
        List<bool> showDownBool = new List<bool>();
        showDownBool.Add(true);
        showDownBool.Add(true);

        ButtonRoutine(showDownBool);
    }

    public void OnLeft()
    {
        List<bool> showDownBool = new List<bool>();
        showDownBool.Add(true);
        showDownBool.Add(false);

        ButtonRoutine(showDownBool);
    }

    public void OnRight()
    {
        List<bool> showDownBool = new List<bool>();
        showDownBool.Add(false);
        showDownBool.Add(true);

        ButtonRoutine(showDownBool);
    }

    public void OnButtom()
    {
        List<bool> showDownBool = new List<bool>();
        showDownBool.Add(false);
        showDownBool.Add(false);

        ButtonRoutine(showDownBool);
    }

    private void ButtonRoutine(List<bool> showDownBool)
    {
        /* Send SHOWDOWN message to server */
        GameMessage message = new GameMessage(GameManager.thisPlayerRoom.id, GameMessage.MessageType.SHOWDOWN,
        GameManager.thisPlayer.name, showDownBool);
        GameMsgHandler.SendMessage(message);
        
        /* Disable showdownPanel */
        gameObject.SetActive(false);
    }
}
