using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardSprite : MonoBehaviour
{
    
    public List<Sprite> clubs;
    public List<Sprite> diamonds;
    public List<Sprite> hearts;
    public List<Sprite> spades;
    public Sprite back;
    
    private static CardSprite instance;

    public static CardSprite GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<CardSprite>();   
        }
        return instance;
    }
    public Sprite GetSprite(Card c)
    {

        switch(c.suit)
        {
            case Card.Suit.CLUB:
                return clubs[c.num];
            case Card.Suit.DIAMOND:
                return diamonds[c.num];
            case Card.Suit.HEART:
                return hearts[c.num];
            case Card.Suit.SPADE:
                return spades[c.num];
            default:
                return null;
        }
    }

    
}
