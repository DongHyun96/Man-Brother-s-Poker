using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public enum Suit{
        CLUB, DIAMOND, HEART, SPADE
    }

    public Suit suit;
    public int num; // 0: two, 1: three ... 12: Ace

    public Card() {}
    public Card(Suit suit, int num)
    {
        this.suit = suit;
        this.num = num;
    }
}
