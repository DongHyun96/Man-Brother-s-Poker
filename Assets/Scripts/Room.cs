using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

public class Room
{
    public string host;
    public List<Player> players = new List<Player>();
    public GameTable gameTable;

    // Rooms property
    public string title;
    public string password;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BuyIn{
        [EnumMember(Value = "ONE")]
        ONE, 
        [EnumMember(Value = "FIVE")]
        FIVE,
        [EnumMember(Value = "TEN")]
        TEN,
        [EnumMember(Value = "TWENTY")]
        TWENTY,
        [EnumMember(Value = "FIFTY")]
        FIFTY,
        [EnumMember(Value = "HUNDRED")]
        HUNDRED
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Mode{
        [EnumMember(Value = "CHICKEN")]
        CHICKEN,
        [EnumMember(Value = "LASTMAN")]
        LASTMAN,
        [EnumMember(Value = "HEADS")]
        HEADS
    }

    public BuyIn buyIn;
    public Mode mode;

    public bool isPlaying = false;

    public Room(){}
    public Room(string host, string title, string password, BuyIn b, Mode m)
    {
        this.host = host;
        this.title = title;
        this.password = password;
        this.buyIn = b;
        this.mode = m;
    }

    
}
