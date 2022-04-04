using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDownPanel : MonoBehaviour
{
    public Image UpperLeft;
    public Image UpperRight;
    public Image left;
    public Image right;

    public void InitShowDown(List<Card> cards)
    {
        // Init Upper
        UpperLeft.sprite = CardSprite.GetInstance().GetSprite(cards[0]);
        UpperRight.sprite = CardSprite.GetInstance().GetSprite(cards[1]);

        // Init left
        left.sprite = CardSprite.GetInstance().GetSprite(cards[0]);

        // Init right
        right.sprite = CardSprite.GetInstance().GetSprite(cards[1]);
    }
}
