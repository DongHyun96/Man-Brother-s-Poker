using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

public class Player
{
    public string name;

    // Whether player is available to get invitation
    public bool invitable = true;
    
    /********************************************************************************************************
    In game ref
    *********************************************************************************************************/
    [JsonConverter(typeof(StringEnumConverter))]
    public enum State{
        [EnumMember(Value = "IDLE")]
        IDLE
    }
    public State state;

    public int totalChips;
    public int betChips;

    public List<Card> cards;
    /********************************************************************************************************/

    public Player(string name)
    {
        this.name = name;
    }
    public Player(){}

}
