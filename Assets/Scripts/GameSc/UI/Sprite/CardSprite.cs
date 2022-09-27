using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardSprite : MonoBehaviour
{
    
    [SerializeField] private List<Sprite> clubs;
    [SerializeField] private List<Sprite> diamonds;
    [SerializeField] private List<Sprite> hearts;
    [SerializeField] private List<Sprite> spades;
    [SerializeField] private Sprite back;
    
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


    public Sprite GetBack()
    {
        return back;
    }

    
}
