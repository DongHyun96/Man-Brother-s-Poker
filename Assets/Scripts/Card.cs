using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

public class Card
{
    [JsonConverter(typeof(StringEnumConverter))]
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

    public override string ToString()
    {
        return $"Suit = {suit}, num = {num}";
    }
}
