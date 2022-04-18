using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

[JsonConverter(typeof(StringEnumConverter))]
public enum Hand
{
    HIGHCARD, PAIR, TWOPAIR, THREE_OF_A_KIND,
    STRAIGHT, FLUSH, FULLHOUSE, FOUR_OF_A_KIND,
    STRAIGHT_FLUSH, ROYAL_FLUSH
}