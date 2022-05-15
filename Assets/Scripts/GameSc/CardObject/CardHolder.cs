using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> spades;
    [SerializeField] private List<GameObject> diamonds;
    [SerializeField] private List<GameObject> hearts;
    [SerializeField] private List<GameObject> clubs;


    private static CardHolder instance;

    public static CardHolder GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<CardHolder>();
        }
        return instance;
    }

    public GameObject GetCardPrefab(Card c)
    {
        if(c == null)
        {
            return null;
        }

        switch(c.suit)
        {
            case Card.Suit.SPADE:
                return spades[c.num];
            case Card.Suit.DIAMOND:
                return diamonds[c.num];
            case Card.Suit.HEART:
                return hearts[c.num];
            case Card.Suit.CLUB:
                return clubs[c.num];
            default:
                return null;
        }
    }
}
