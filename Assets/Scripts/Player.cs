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
    
    // Player's character
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Character
    {
        MALE, MALE_CAP, FEMALE, POLICE, ZOMBIE
    }
    public Character character;


    /********************************************************************************************************
    In game ref
    *********************************************************************************************************/
    [JsonConverter(typeof(StringEnumConverter))]
    public enum State{
        IDLE, SMALL, BIG, CHECK, BET, CALL, RAISE, ALLIN, FOLD
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
    public Player(string name, Character character)
    {
        this.name = name;
        this.character = character;
    }
    public Player(Character character)
    {
        this.character = character;
    }
    public Player(){}

}
